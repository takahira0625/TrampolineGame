using UnityEngine;

public class KeyBlock : GimmickBlock
{
    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
    }
    protected override void SetActiveState()
    {
        SetSprite(parameter.KeySprite);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // PlayerInventory �R���|�[�l���g��T���ăL�[�����Z
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey();
            }

            // ����������
            Destroy(gameObject);
        }
    }
}
