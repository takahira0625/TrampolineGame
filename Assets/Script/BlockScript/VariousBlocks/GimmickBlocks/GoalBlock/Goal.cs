using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // 必要キー数
    protected override void Awake()
    {
        base.Awake();

        // シーン内の KeyBlock の数を取得して requiredKeys に設定
        KeyBlock[] keyBlocks = FindObjectsOfType<KeyBlock>();
        requiredKeys = keyBlocks.Length;

        SetSprite(parameter.GoalSprite);
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.KeyCount >= requiredKeys)
            {
                GameManager.instance.Goal();
                Debug.Log("Goal! " + inventory.KeyCount);
                SceneManager.LoadScene("ResultScene");
            }
            else
            {
                Debug.Log("キーの数が足りません");
            }
        }
    }
}
