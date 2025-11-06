using UnityEngine;
using System; // イベント(Action)を使うために必要

// KeyFlyToHUD スクリプトが同じプロジェクトに存在し、
// KeyBlockと同じGameObjectか子にアタッチされることを前提とする
// (あるいはAddComponentで追加されます)

public class KeyBlock : MonoBehaviour
{
    [SerializeField] private Vector2 targetSize = new Vector2(0.1f, 0.1f);
    [SerializeField] private AudioClip KeySE; // 鍵取得時の効果音

    [Header("鍵部品設定")]
    [Tooltip("このブロックが対応する鍵部品の番号 (0から始まるインデックス)")]
    public int keyPartIndex;

    [Header("エフェクト設定")]
    [Tooltip("Playerとの接触時にエフェクトを再生する")]
    [SerializeField] private bool playEffectOnHit = true;

    // 衝突エフェクト(クラス全体で共有)
    private static ParticleSystem hitEffectPrefab;
    private static ParticleSystem getEffectPrefab;
    private static bool isEffectLoaded = false;

    // UIに「この番号の部品が取られた」と通知するイベント
    public static event Action<int> OnKeyPartCollected;

    private const string KeyUIControllerName = "KeyUIController";


    protected void Awake()
    {
        SetSprite();
        LoadKeySE();
        PreloadHitEffect();
    }

    private void PreloadHitEffect()
    {
        if (!isEffectLoaded)
        {
            hitEffectPrefab = Resources.Load<ParticleSystem>("Effects/CFXR3 Hit Fire B (Air)");
            getEffectPrefab = Resources.Load<ParticleSystem>("Effects/Holy hit");
            isEffectLoaded = true;

            if (hitEffectPrefab == null)
            {
                Debug.LogWarning("取得エフェクトが見つかりません: Resources/Effects/CFXR3 Hit Fire B (Air)");
            }
        }
    }

    /// <summary>
    /// 鍵取得時の効果音を読み込む
    /// </summary>
    private void LoadKeySE()
    {
        // カスタムSEが設定されていない場合のみ自動ロード
        if (KeySE == null)
        {
            // Resources/Audio/SE/Key から読み込み
            KeySE = Resources.Load<AudioClip>("Audio/SE/Block/Key");

            if (KeySE == null)
            {
                Debug.LogWarning("鍵のSEが見つかりません: Resources/Audio/SE/BlockKey");
            }
        }
    }

    private void SetSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
        {
            Debug.LogWarning("SpriteRenderer または Sprite が設定されていません");
            return;
        }

        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(targetSize.x, targetSize.y);
        // BoxCollider2D のサイズも合わせる
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = sr.size;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            // 接触位置を取得 (この位置でエフェクトを生成する)
            Vector2 contactPoint = collision.contacts[0].point;

            // --- 取得ロジック (変更なし) ---
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey();

                // SEを再生
                if (KeySE != null && SEManager.Instance != null)
                {
                    SEManager.Instance.PlayOneShot(KeySE);
                }
            }

            // エフェクトを再生 (鍵が消える前に行う)
            PlayHitEffect(contactPoint);

            // UIに通知
            OnKeyPartCollected?.Invoke(keyPartIndex);


            // 1. 行先を取得
            KeyDestination hudTarget = KeyDestination.Instance;

            if (hudTarget != null)
            {
                Vector3 targetWorldPos = hudTarget.transform.position;

                // 2. 飛翔コンポーネントを取得/追加
                KeyFlyToHUD flyer = GetComponent<KeyFlyToHUD>();
                if (flyer == null)
                {
                    // 飛翔に必要な KeyFlyToHUD がなければ追加
                    flyer = gameObject.AddComponent<KeyFlyToHUD>();
                }

                // 3. 飛翔開始
                flyer.StartFlight(targetWorldPos);
                Debug.Log("KeyFlyToHUD.StartFlightを呼び出しました。");


                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

            }
            else
            {
                Debug.LogError("KeyUIControllerが見つかりません。即座に鍵を破壊します。");
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 衝突位置にエフェクトを生成
    /// </summary>
    private void PlayHitEffect(Vector2 position)
    {
        if (!playEffectOnHit) return;

        if (hitEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }

        if (getEffectPrefab != null)
        {
            ParticleSystem getEffect = Instantiate(getEffectPrefab, this.transform.position, Quaternion.identity);

            Destroy(getEffect.gameObject, getEffect.main.duration + getEffect.main.startLifetime.constantMax - 1);
        }
    }
}