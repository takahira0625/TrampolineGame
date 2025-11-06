using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // 必要キー数

    [Header("ゴール見た目設定")]
    [SerializeField] private Sprite lockedSprite;    // 鍵未取得時の見た目
    [SerializeField] private Sprite unlockedSprite; // 全取得後の見た目
    [SerializeField] private GameObject ShieldEffect; // 鍵を取得していない時にぶつかったときのエフェクト
    [SerializeField] private GameObject ShieldEffectHit; // 鍵を取得していない時にぶつかったときのエフェクト

    [Header("解放エフェクト設定")]
    [SerializeField] private ParticleSystem unlockEffectPrefab;
    [SerializeField] private Vector3 effectOffset = Vector3.zero;

    private SpriteRenderer spriteRenderer;
    private bool isUnlocked = false;
    private bool hasGoaled = false; // ゴール処理済みフラグを追加

    [Header("サウンド設定")]
    [SerializeField] private AudioClip goalSE;
    [SerializeField, Header("シールドヒット音")]
    private AudioClip shieldHitSE;

    //演出設定
    [Header("ゴール演出設定")]
    [SerializeField] private float animationDuration = 2.0f; // 演出の総時間（実時間）
    [SerializeField] private float initialRadius = 2.0f;     // ゴール開始時の旋回半径
    [SerializeField] private float rotationSpeed = 360f;     // 毎秒の回転角度

    protected override void Awake()
    {
        base.Awake();

        // シーン内の KeyBlock の数を取得して requiredKeys に設定
        KeyBlock[] keyBlocks = FindObjectsOfType<KeyBlock>();
        requiredKeys = keyBlocks.Length;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = lockedSprite; // 初期状態はロック見た目
    }
    private void OnEnable()
    {
        // PlayerInventory のイベント購読
        PlayerInventory.OnKeyCountChanged += HandleKeyCountChanged;
    }

    private void OnDisable()
    {
        //イベント購読解除（メモリリーク防止）
        PlayerInventory.OnKeyCountChanged -= HandleKeyCountChanged;
    }

    //鍵の数が変わったときに呼ばれる関数
    private void HandleKeyCountChanged(int currentKeyCount)
    {
        if (!isUnlocked && currentKeyCount >= requiredKeys)
        {
            isUnlocked = true;
            spriteRenderer.sprite = unlockedSprite;
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            SpawnUnlockEffect();

            //鍵が揃った瞬間のアニメーションはそのまま実行 (Time.timeScale=0.5f)
            StartCoroutine(PlayUnlockAnimation());
            Debug.Log("ゴールが開放されました！");
        }
    }

    private IEnumerator PlayUnlockAnimation()
    {
        // --- 時間をゆっくりにする ---
        Time.timeScale = 0.5f;

        Vector3 originalScale = transform.localScale;
        Vector3 smallScale = originalScale * 0.1f;

        float duration = 0.6f;
        float return_duration = 0.1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(originalScale, smallScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < return_duration)
        {
            elapsed += Time.unscaledDeltaTime;
            //補間割合の修正: durationではなくreturn_durationを使用
            transform.localScale = Vector3.Lerp(smallScale, originalScale, elapsed / return_duration);
            yield return null;
        }

        // --- 時間の流れを元に戻す ---
        Time.timeScale = 1f;
    }

    private void SpawnUnlockEffect()
    {
        if (unlockEffectPrefab == null) return;

        ParticleSystem effect = Instantiate(unlockEffectPrefab, transform);
        effect.transform.localPosition = effectOffset;
        effect.Play();
    }

    // ====================================================================
    // 衝突処理の修正と演出コルーチンの追加
    // ====================================================================

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //鍵が揃っており、まだゴールしていない場合
            if (GameManager.instance.TotalKeys >= requiredKeys && !hasGoaled)
            {
                hasGoaled = true; // ゴール処理済みフラグを立てる

                //通常のゴール処理の代わりに、演出コルーチンを開始
                StartCoroutine(StartGoalAnimation(collision.gameObject));

                // BGM停止、SE再生、GameManager.instance.Goal()はコルーチン内で処理します
            }
            else
            {
                // 鍵が揃っていない場合のシールド演出（変更なし）
                ContactPoint2D contact = collision.GetContact(0);
                if (ShieldEffect != null)
                {
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

                    GameObject effect = Instantiate(
                        ShieldEffect,
                        transform.position,
                        rotation
                    );

                    if (shieldHitSE != null && SEManager.Instance != null && !hasGoaled)
                    {
                        SEManager.Instance.PlayOneShot(shieldHitSE);
                    }
                    Destroy(effect, 2f);
                }
                if (ShieldEffectHit != null)
                {
                    GameObject effectHit = Instantiate(
                        ShieldEffectHit,
                        contact.point,
                        Quaternion.identity
                    );
                    Destroy(effectHit, 1.5f);
                }
            }
        }
    }

    /// <summary>
    /// ゴール時のボール吸い込みアニメーションを開始するコルーチン
    /// </summary>
    private IEnumerator StartGoalAnimation(GameObject player)
    {
        // 1. サウンドと時間の設定
        BGMManager.Instance.Stop();
        SEManager.Instance.PlayOneShot(goalSE);

        // ゴールとボール以外を完全に停止 (Time.timeScale = 0f;)
        Time.timeScale = 0f;

        // 2. ボールの物理演算を停止し、Transformで制御可能にする
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.isKinematic = true; // 物理演算を無効化
        }

        // 3. アニメーションの準備
        float elapsedRealTime = 0f;
        Vector3 goalCenter = transform.position;
        Vector3 initialDir = (player.transform.position - goalCenter).normalized; // ゴールからボールへの初期方向
        Vector3 initialScale = player.transform.localScale;

        // 4. アニメーションループ (実時間で実行)
        while (elapsedRealTime < animationDuration)
        {
            float t = elapsedRealTime / animationDuration;

            // a. 半径（吸い込み）: initialRadius から 0f まで線形補間
            float currentRadius = Mathf.Lerp(initialRadius, 0f, t);

            // b. 回転: 時間経過と共に角度を増やす
            float angle = rotationSpeed * elapsedRealTime;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // c. 位置計算: 回転 + 吸い込み
            Vector3 newOffset = rotation * initialDir * currentRadius;
            player.transform.position = goalCenter + newOffset;

            // d. スケール（消滅）: 1.0から0まで縮小
            player.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

            elapsedRealTime += Time.unscaledDeltaTime; // 停止時間中でも進行
            yield return null; // Time.timeScale=0 でも毎フレーム実行
        }

        // 5. 演出終了後の処理

        // ボールを完全に破棄/非表示
        Destroy(player);

        // 時間を元に戻す
        Time.timeScale = 1f;

        // 最終的なゴール処理を呼び出す
        GameManager.instance.Goal();

        Debug.Log("Goal! " + GameManager.instance.TotalKeys);
    }
}