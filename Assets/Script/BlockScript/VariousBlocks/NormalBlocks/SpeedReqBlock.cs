using UnityEngine;

public class SpeedReqBlock : BaseBlock
{
    [Header("速度要求設定")]
    [Tooltip("この速度以上でぶつからないと壊れない")]
    [SerializeField] private float requiredSpeed = 50f;

    [Header("スプライト設定")]
    [Tooltip("個別に見た目を変えたい場合に指定。未設定なら parameter のスプライトを使用します。")]
    [SerializeField] private Sprite customSprite; //個別スプライト指定

    [Header("効果音設定")]
    [SerializeField] protected AudioClip customHitBudSound;
    [SerializeField] private AudioClip breakSound;

    private void Awake()
    {
        base.Awake();

        // 個別スプライトが設定されていればそれを使用
        if (customSprite != null)
        {
            SetSprite(customSprite);
        }
        else
        {
            Debug.LogError("customSprite is not set for SpeedReqBlock.");
        }
    }

    private void PlayHitBudSound()
    {
        if (!playSoundOnHit) return;

        if (customHitBudSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(customHitBudSound);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                float collisionSpeed = playerRb.velocity.magnitude;
                Vector2 contactPoint = collision.contacts[0].point;

                if (collisionSpeed >= requiredSpeed)
                {
                    PlayBreakSound();
                    PlayHitEffect(contactPoint);
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
            PlayBreakSound();
        }
    }

    private void PlayBreakSound()
    {
        if (breakSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(breakSound);
        }
    }
}
