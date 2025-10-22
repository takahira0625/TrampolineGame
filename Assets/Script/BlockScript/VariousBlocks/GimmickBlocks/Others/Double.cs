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

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        Vector2 playerVelocity = playerRb != null ? playerRb.velocity : Vector2.zero;
        base.OnCollisionEnter2D(collision);
        // ���x�t���Ő���
        GameManager.instance.SpawnAdditionalPlayer(collision.transform, playerVelocity);

        Destroy(gameObject); // ��x�����g�p�\
    }
}
