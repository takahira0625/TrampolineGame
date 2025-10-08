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

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision); // 親の共通処理（クールタイムなど）

        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey();
            }

            Destroy(gameObject);
        }
    }
}
