using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // 必要キー数
    
    [Header("ゴール見た目設定")]
    [SerializeField] private Sprite lockedSprite;   // 鍵未取得時の見た目
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
        //ここに鍵が壊れる演出を作る

        // --- 縮む（0.3秒）---
        float duration = 0.6f;
        float return_duration = 0.1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(originalScale, smallScale, elapsed / duration);
            yield return null;
        }

        // --- 元に戻る（0.3秒）---
        elapsed = 0f;
        while (elapsed < return_duration)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(smallScale, originalScale, elapsed / duration);
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

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.TotalKeys >= requiredKeys && !hasGoaled)
            {
                hasGoaled = true; // ゴール処理済みフラグを立てる
                BGMManager.Instance.Stop();
                SEManager.Instance.PlayOneShot(goalSE);
                GameManager.instance.Goal();
                Debug.Log("Goal! " + GameManager.instance.TotalKeys);
            }
            else
            {
                ContactPoint2D contact = collision.GetContact(0);
                if (ShieldEffect != null)
                {
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

                    // ゴールの位置でエフェクトを生成
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
                    // 衝突位置でエフェクトを生成

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
}
