using UnityEngine;
using System; // イベント(Action)を使うために必要

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

    protected void Awake()
    {
        SetSprite();
        LoadKeySE();
        PreloadHitEffect();
    }

    /// <summary>
    /// エフェクトを事前にロード(一度のみ)
    /// </summary>
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
            // 接触位置を取得
            Vector2 contactPoint = collision.contacts[0].point;

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

            // エフェクトを再生
            PlayHitEffect(contactPoint);

            // UIに通知
            OnKeyPartCollected?.Invoke(keyPartIndex);

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 衝突位置にエフェクトを生成
    /// </summary>
    private void PlayHitEffect(Vector2 position)
    {
        if (!playEffectOnHit || hitEffectPrefab == null) return;

        // エフェクトを生成して再生
        //ParticleSystem effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        ParticleSystem getEffect = Instantiate(getEffectPrefab, this.transform.position, Quaternion.identity);
        // 自動削除
        //Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        Destroy(getEffect.gameObject, getEffect.main.duration + getEffect.main.startLifetime.constantMax);
    }
}
