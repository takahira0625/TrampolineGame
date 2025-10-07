using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trampoline : MonoBehaviour
{
    [Tooltip("（任意）最低反発速度。0以下なら最低値なし")]
    public float minBounceSpeed = 0f;

    [Tooltip("生成後に自動破棄されるまでの秒数。0以下の場合は破棄しない")]
    public float lifeTime = 5f;

    // 予備：LineDrawerから与えられる基準法線（連絡が来ない/接触が0件のとき用）
    private Vector2 fallbackNormal = Vector2.up;

    // 直近衝突で用いた反発速度（継続接触での補正用）
    private float lastImpactSpeed = 0f;

    private void Start()
    {
        if (lifeTime > 0f)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    public void SetNormal(Vector2 normal)
    {
        fallbackNormal = normal.normalized;
        float angleZ = Vector2.SignedAngle(Vector2.up, fallbackNormal);
        Debug.Log("描画法線の角度: " + angleZ + "°");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        Vector2 n = GetAveragedContactNormal(collision);

        // 衝突時の相対速度（トランポリンが静止ならプレイヤーの衝突直前速度に相当）
        float impactSpeed = collision.relativeVelocity.magnitude;

        // 念のため相対速度が極小のときのフォールバック（例：Unity の内部解決後など）
        if (impactSpeed < 0.001f)
        {
            impactSpeed = rb.velocity.magnitude;
        }

        if (minBounceSpeed > 0f && impactSpeed < minBounceSpeed)
        {
            impactSpeed = minBounceSpeed;
        }

        lastImpactSpeed = impactSpeed;

        // 衝突速度と同じ大きさで法線方向へ再設定
        rb.velocity = -n * impactSpeed;

        Debug.Log($"Bounce ENTER: n={n}, impactSpeed={impactSpeed:F3}, v={rb.velocity}");
    }

    // 継続接触時：速度が著しく失われたら同等速度を再付与（任意）
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        if (lastImpactSpeed <= 0f) return;

        Vector2 n = GetAveragedContactNormal(collision);

        // 法線方向成分が当初反発速度の半分未満なら補正
        float along = Vector2.Dot(rb.velocity, n);
        if (along < lastImpactSpeed * 0.5f)
        {
            rb.velocity = -n * lastImpactSpeed;
            Debug.Log($"Bounce STAY (reapply): n={n}, restored={lastImpactSpeed:F3}");
        }
    }

    private Vector2 GetAveragedContactNormal(Collision2D collision)
    {
        Vector2 n = Vector2.zero;
        int cnt = collision.contactCount;
        for (int i = 0; i < cnt; i++)
            n += collision.GetContact(i).normal;

        if (n == Vector2.zero) n = fallbackNormal;
        return n.normalized;
    }
}
