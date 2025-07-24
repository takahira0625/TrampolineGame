using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trampoline : MonoBehaviour
{
    [Tooltip("跳ね上げる初速（m/s）")]
    public float jumpSpeed = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody2D rb = collision.collider.attachedRigidbody;
        if (rb == null) return;

        // 衝突点の平均法線を求める
        Vector2 normal = Vector2.zero;
        foreach (ContactPoint2D cp in collision.contacts)
        {
            normal += cp.normal;
        }
        normal.Normalize();

        // 速度 v を「接線成分 vt」と「法線成分 vn」に分解
        Vector2 v = rb.velocity;
        float vnMag = Vector2.Dot(v, normal);      // 法線方向成分の大きさ
        Vector2 vt = v - vnMag * normal;           // 接線方向成分

        // 新しい速度：接線成分 ＋ 法線方向を jumpSpeed に置換
        Vector2 newVelocity = vt + normal * jumpSpeed;
        rb.velocity = - newVelocity;
    }
}
