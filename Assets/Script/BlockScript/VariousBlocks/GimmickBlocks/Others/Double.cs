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

        // 45����]�������x���v�Z
        Vector2 rotatedVelocity = Rotate45Degrees(playerVelocity);

        base.OnCollisionEnter2D(collision);

        // ��]�������x�Ő���
        GameManager.instance.SpawnAdditionalPlayer(collision.transform, rotatedVelocity);

        Destroy(gameObject);
    }

    /// <summary>
    /// �x�N�g����45����]�i�����v���j
    /// </summary>
    private Vector2 Rotate45Degrees(Vector2 velocity)
    {
        float angle = 45f * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        return new Vector2(
            velocity.x * cos - velocity.y * sin,
            velocity.x * sin + velocity.y * cos
        );
    }
}
