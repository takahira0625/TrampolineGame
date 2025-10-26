using UnityEngine;

public class SpeedReqBlock : BaseBlock
{
    [Header("速度要求")]
    [Tooltip("この速度以上でぶつからないと壊れない")]
    [SerializeField] private float requiredSpeed = 50f;
    [SerializeField] protected AudioClip customHitBudSound;

    private void PlayHitBudSound()
    {
        if (!playSoundOnHit) return;

        // キャッシュされた効果音を再生(瞬時に再生される)
        if (customHitBudSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(customHitBudSound);
        }
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // プレイヤーのRigidbodyを取得
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                float collisionSpeed = playerRb.velocity.magnitude;
                Vector2 contactPoint = collision.contacts[0].point;

                // 指定速度以上ならダメージ処理
                if (collisionSpeed >= requiredSpeed)
                {

                    // 効果音・エフェクト再生
                    PlayBreakSound();
                    PlayHitEffect(contactPoint);

                    // BaseBlockの耐久処理
                    TakeDamage(3);
                }
                else
                {
                    PlayHitBudSound();
                    PlayHitEffect(contactPoint);

                }
            }
        }
    }
    protected override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (health <= 0)
        {
            // 壊れた時の演出
            PlayBreakSound();
        }
    }
    [SerializeField] private AudioClip breakSound;
    private void PlayBreakSound()
    {
        if (breakSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(breakSound);
        }
    }
}
