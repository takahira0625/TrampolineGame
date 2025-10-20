using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    [SerializeField] private float minLength = 0.5f;
    [SerializeField] private float maxLength = 5f;
    private SpriteRenderer sr;
    private float originalSpriteWidth;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // スプライトの元のサイズ(スケール1の時)を保存
        originalSpriteWidth = sr.sprite.bounds.size.x;
    }

    public void UpdateArrow(Vector2 barPos, Vector2 ballPos, float ratio)
    {
        Vector2 dir = (barPos - ballPos).normalized;

        // 回転
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 長さ
        float newLength = Mathf.Lerp(minLength, maxLength, ratio);
        transform.localScale = new Vector3(newLength, 1, 1);

        // 根元（左端）をボールの中心に配置
        // スプライトの左端から中心までの距離 = 元のサイズの半分 × 現在のスケール
        float halfWidth = originalSpriteWidth / 2f * newLength;
        Vector2 offsetToRoot = -dir * halfWidth; // 方向の逆向きにオフセット

        transform.position = ballPos + offsetToRoot;
    }
}