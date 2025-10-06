using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // 必要キー数
    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.GoalSprite);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.KeyCount >= requiredKeys)
            {
                SceneManager.LoadScene("ResultScene");
                Debug.Log("Goal!" + inventory.KeyCount);
            }
            else
            {
                Debug.Log("キーの数が足りません" );

            }
        }
    }
}
