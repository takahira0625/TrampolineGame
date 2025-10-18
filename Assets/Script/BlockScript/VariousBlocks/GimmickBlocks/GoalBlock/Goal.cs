using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // �K�v�L�[��
    protected override void Awake()
    {
        base.Awake();

        // �V�[������ KeyBlock �̐����擾���� requiredKeys �ɐݒ�
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
                Debug.Log("�L�[�̐�������܂���");
            }
        }
    }
}
