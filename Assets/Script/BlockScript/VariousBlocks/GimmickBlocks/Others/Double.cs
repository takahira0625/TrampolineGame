using UnityEngine;

public class Double : GimmickBlock
{
    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
    }
    protected override void SetActiveState()
    {
        SetSprite(parameter.DoubleSprite);
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        base.OnCollisionEnter2D(collision);
        GameManager.instance.SpawnAdditionalPlayer(collision.transform);

        Destroy(gameObject); // 一度だけ使用可能
    }
}
