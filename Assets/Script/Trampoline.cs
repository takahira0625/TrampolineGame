using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trampoline : MonoBehaviour
{
    [Tooltip("跳ね上げる初速（m/s）")]
    public float jumpSpeed = 10f;

    [Tooltip("生成後に自動破棄されるまでの秒数。0以下の場合は破棄しない")]
    public float lifeTime = 5f;

    // 予備：LineDrawerから与えられる基準法線（連絡が来ない/接触が0件のとき用）
    private Vector2 fallbackNormal = Vector2.up;

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

        // プレイヤー跳ね上げ処理（無効化）
        /*
        Vector2 n = GetAveragedContactNormal(collision);
        rb.velocity = n * jumpSpeed;
        Debug.Log($"Bounce ENTER: n={n}, v={rb.velocity}");
        */
    }

    // 連続接触時の再加速処理（無効化中）
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        // プレイヤー跳ね上げ再適用処理（無効化）
        /*
        Vector2 n = GetAveragedContactNormal(collision);
        if (Vector2.Dot(rb.velocity, n) < jumpSpeed * 0.5f)
        {
            rb.velocity = -n * jumpSpeed;
            Debug.Log($"Bounce STAY: n={n}, v={rb.velocity}");
        }
        */
    }

    private Vector2 GetAveragedContactNormal(Collision2D collision)
    {
        Vector2 n = Vector2.zero;
        int cnt = collision.contactCount;
        for (int i = 0; i < cnt; i++)
            n += collision.GetContact(i).normal;  // ← このオブジェクト（トランポリン）側の法線

        if (n == Vector2.zero) n = fallbackNormal; // 念のためのフォールバック
        return n.normalized;
    }
}
