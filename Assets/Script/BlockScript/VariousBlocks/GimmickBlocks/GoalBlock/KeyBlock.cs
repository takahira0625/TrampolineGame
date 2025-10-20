using UnityEngine;
using System; // イベント(Action)を使うために必要

public class KeyBlock : MonoBehaviour
{
    [SerializeField] private Vector2 targetSize = new Vector2(0.1f, 0.1f);
    [SerializeField] private AudioClip KeySE; // 鍵取得時の効果音

    [Header("鍵部品設定")]
    [Tooltip("このブロックが対応する鍵部品の番号 (0から始まるインデックス)")]
    public int keyPartIndex; // 追加：インスペクターで 0, 1, 2, 3 のどれかを設定

    // 追加：UIに「この番号の部品が取られた」と通知するイベント
    public static event Action<int> OnKeyPartCollected;

    protected void Awake()
    {
        SetSprite();
        LoadKeySE();
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

            // --- UI用の新しい通知（部品番号を通知）---
            OnKeyPartCollected?.Invoke(keyPartIndex); // UIに「keyPartIndex番が取られた」と通知

            Destroy(gameObject);
        }
    }
}
