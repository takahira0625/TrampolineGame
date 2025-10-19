using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // 必要キー数
    [Header("ゴール見た目設定")]
    [SerializeField] private Sprite lockedSprite;   // 鍵未取得時の見た目
    [SerializeField] private Sprite unlockedSprite; // 全取得後の見た目

    private SpriteRenderer spriteRenderer;
    private bool isUnlocked = false;

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
            Debug.Log("ゴールが開放されました！");
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.TotalKeys >= requiredKeys)
            {
                GameManager.instance.Goal();
                Debug.Log("Goal! " + GameManager.instance.TotalKeys);
            }
            else
            {
                Debug.Log("キーの数が足りません");
            }
        }
    }
}
