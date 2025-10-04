using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private int requiredKeys = 3; // 必要キー数

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.KeyCount >= requiredKeys)
            {
                Debug.Log("ゴール成功！");
                SceneManager.LoadScene("ResultScene");
            }
            else
            {
                Debug.Log("キーが足りません！");
            }
        }
    }
}
