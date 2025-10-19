using UnityEngine;

public class KeyBlock : MonoBehaviour
{
    [SerializeField] private Vector2 targetSize = new Vector2(0.1f, 0.1f);
    [SerializeField] private AudioClip KeySE; // 鍵取得時の効果音

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

            Destroy(gameObject);
        }
    }
}
