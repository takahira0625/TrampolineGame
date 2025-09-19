using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trampoline : MonoBehaviour
{
    [Tooltip("跳ね上げる初速（m/s）")]
    public float jumpSpeed = 10f;
    private Vector2 normalDirection;

    // LineDrawer.csから法線を設定
    public void SetNormal(Vector2 normal)
    {
        normalDirection = normal.normalized;

        // デバッグ表示
        float angleZ = Vector2.SignedAngle(Vector2.up, normalDirection);
        Debug.Log("トランポリンの角度: " + angleZ + "°");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        Debug.Log("衝突した相手: " + collision.gameObject.name);

        Rigidbody2D rb = collision.collider.attachedRigidbody;
        if (rb == null) return;

          SetVelocityInDirection(rb, normalDirection, jumpSpeed);

    }


    private void SetVelocityInDirection(Rigidbody2D rb, Vector2 direction, float speed)
    {
        Vector2 newVelocity = direction.normalized * speed;
        rb.velocity = newVelocity;
    }
}
