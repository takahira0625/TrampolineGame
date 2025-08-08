using UnityEngine;

//こっちは使っていません。

public class TrampolineScript : MonoBehaviour
{
    public float bounceForce = 20f; // 跳ね返す力の強さ

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("衝突検知！ 相手は: " + collision.gameObject.name);
        // 衝突した相手のRigidbody2Dを取得
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // 衝突した相手の現在の速度をリセットし、法線ベクトル方向（衝突面から垂直上向き）に力を加える
            rb.velocity = Vector2.zero;
            rb.AddForce(collision.contacts[0].normal * bounceForce, ForceMode2D.Impulse);
        }
    }
}