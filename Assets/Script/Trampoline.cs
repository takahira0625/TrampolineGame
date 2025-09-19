using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trampoline : MonoBehaviour
{
    [Tooltip("跳ね上げる初速（m/s）")]
    public float jumpSpeed = 10f;

    // 予備：LineDrawerから与えられる基準法線（連絡が来ない/接触が0件のとき用）
    private Vector2 fallbackNormal = Vector2.up;

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
        rb.velocity = n * jumpSpeed;

        Debug.Log($"Bounce ENTER: n={n}, v={rb.velocity}");
    }

    // 連続接触でも確実に発火させたい場合は有効化（必要なければ削除可）
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        // もし減速して面に押し付けられている等で再加速したいなら、条件付きで再適用
        Vector2 n = GetAveragedContactNormal(collision);
        // 面から離れる成分が小さいときだけ上書き
        if (Vector2.Dot(rb.velocity, n) < jumpSpeed * 0.5f)
        {
            rb.velocity = -n * jumpSpeed;
            Debug.Log($"Bounce STAY: n={n}, v={rb.velocity}");
        }
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
