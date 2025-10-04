using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private int requiredKeys = 3; // �K�v�L�[��

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.KeyCount >= requiredKeys)
            {
                Debug.Log("�S�[�������I");
                SceneManager.LoadScene("ResultScene");
            }
            else
            {
                Debug.Log("�L�[������܂���I");
            }
        }
    }
}
