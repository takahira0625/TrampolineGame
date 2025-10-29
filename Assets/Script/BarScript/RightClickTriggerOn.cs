using System.Collections;
using UnityEngine;

public class RightClickTriggerOn : MonoBehaviour
{
    [SerializeField] private BarMovement barFollow;
    [Header("前進距離と時間")]
    [SerializeField] float backDistance = 2f;        // 少し下がる距離（調整可能）
    [SerializeField] float backTime = 0.05f;           // 下がる時間（短め）
    [SerializeField] private float forwardDistance = 10.0f + 2f;
    [SerializeField] private float forwardTime = 0.15f;
    [SerializeField] private float returnTime = 0.3f;

    [Header("反射設定")]
    [SerializeField] private float reboundCoefficient = 0.7f;
    [SerializeField] private float reboundExitSpeed = 25f; // Exit時に飛ばす速度
    [SerializeField] private float pushSpeed = 20f;

    [SerializeField] private GameObject electroHitPrefab;

    [SerializeField, Header("ヒットストップ設定")]
    private float hitStopDuration = 0.1f;
    [SerializeField, Header("右クリックで前進開始時のSE")]
    private AudioClip pushStartSE;

    private bool isMoving = false;
    public bool IsMoving => isMoving;
    private bool hasHitThisPush = false;

    private Vector2 originalPosition;
    private Quaternion startRotation;
    private Collider2D col;
    private Rigidbody2D rb;

    // 反射データの記録
    private Rigidbody2D lastBallRb;
    private Vector2 lastNormal;
    private AudioClip SmashSE;
    private AudioClip CollisionSE;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
    private void LoadSmashSE()
    {
        if (SmashSE == null)
        {
            SmashSE = Resources.Load<AudioClip>("Audio/SE/Block/Smash");

            if (SmashSE == null)
            {
                Debug.LogWarning("SmashSEが見つかりません: Resources/Audio/SE/Block/Smash");
            }
            else
            {
                Debug.Log("SmashSEをロードしました: " + SmashSE.name);
            }
        }
    }

    private void LoadCollisionSE()
    {
        if (CollisionSE == null)
        {
            // ✅ 修正: CollisionSEに正しく代入
            CollisionSE = Resources.Load<AudioClip>("Audio/SE/Block/Collision");

            if (CollisionSE == null)
            {
                Debug.LogWarning("CollisionSEが見つかりません: Resources/Audio/SE/Block/Collision");
            }
            else
            {
                Debug.Log("CollisionSEをロードしました: " + CollisionSE.name);
            }
        }
    }
    void Start()
    {
        col = GetComponent<Collider2D>();
        LoadSmashSE();
        LoadCollisionSE();
    }

    void Update()
    {
        if (InputManager.IsRightClickPressed() && !isMoving && !col.isTrigger)
        {
            Debug.Log("右クリックで前進開始");
            PlaySE(pushStartSE, "PushStartSE");
            startRotation = transform.rotation;
            StartCoroutine(MoveForwardAndBack());
        }
    }

    private IEnumerator MoveForwardAndBack()
    {
        if (barFollow != null)
            barFollow.stopFollow = true;

        isMoving = true;
        col.isTrigger = true;

        originalPosition = rb.position;

        Vector2 forward = (startRotation * Vector2.up).normalized;
        Vector2 targetBackPos = originalPosition - forward * backDistance;
        Vector2 targetForwardPos = originalPosition + forward * forwardDistance;
        
        float elapsed = 0f;

        // --- 少し下がる ---
        while (elapsed < backTime)
        {
            Vector2 newPos = Vector2.Lerp(originalPosition, targetBackPos, elapsed / backTime);
            rb.MovePosition(newPos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(targetBackPos);

        elapsed = 0f;
        // --- 前進 ---
        while (elapsed < forwardTime)
        {
            Vector2 newPos = Vector2.Lerp(originalPosition, targetForwardPos, elapsed / forwardTime);
            rb.MovePosition(newPos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(targetForwardPos);

        // --- 戻る ---
        elapsed = 0f;
        while (elapsed < returnTime)
        {
            Vector2 newPos = Vector2.Lerp(targetForwardPos, originalPosition, elapsed / returnTime);
            rb.MovePosition(newPos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(originalPosition);

        isMoving = false;
        col.isTrigger = false;
        hasHitThisPush = false;
        if (barFollow != null) barFollow.stopFollow = false;
    }
    private void PlaySE(AudioClip clip, string clipName)
    {
        if (clip == null)
        {
            Debug.LogWarning($"{clipName}がnullのため再生できません");
            return;
        }

        if (SEManager.Instance == null)
        {
            Debug.LogError("SEManager.Instanceがnullのため再生できません");
            return;
        }

        Debug.Log($"{clipName}を再生します");
        SEManager.Instance.PlayOneShot(clip);
    }
    // --- Trigger中の押し出し ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.IsChildOf(transform)) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D ballRb = other.attachedRigidbody;
        PlayerController playerCtrl = other.GetComponent<PlayerController>();
        if (ballRb == null || playerCtrl == null || (isMoving && hasHitThisPush)) return;
        
        // --- 押し出し中 ---
        if (isMoving)
        {
            GameManager.instance.StartTimerOnce();
            hasHitThisPush = true;
            Vector2 forward = transform.up.normalized;

            if (playerCtrl.isActive)
            {
                
                Debug.Log("ヒットストップ: Active押し出し");
                PlaySE(SmashSE, "CollisionSE");

                float savedSpeed = playerCtrl.savedVelocity.magnitude;
                ballRb.velocity = forward * Mathf.Max(savedSpeed * 1.1f, pushSpeed + 1);
                playerCtrl.isActive = false;

                // エフェクト生成
                if (electroHitPrefab != null)
                {
                    Vector3 spawnPos = ballRb.transform.position;
                    GameObject effect = Instantiate(electroHitPrefab, spawnPos, Quaternion.identity);

                    // 子ParticleSystemがあればSorting Layerを統一して前面表示
                    var renderers = effect.GetComponentsInChildren<ParticleSystemRenderer>();
                    foreach (var r in renderers)
                    {
                        r.sortingLayerName = "Foreground"; // 必要に応じて変更
                        r.sortingOrder = 10;
                    }

                    // ループする場合でも一定時間で消す（例：1秒）
                    Destroy(effect, 1f);
                }
                StartCoroutine(HitStop());

            }
            else
            {
                PlaySE(CollisionSE, "CollisionSE");
                Debug.Log("非Active押し出し");
                ballRb.velocity = forward * pushSpeed;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isMoving) return; // Trigger中はスキップ

        Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (ballRb == null) return;

        Vector2 dir = ballRb.velocity.normalized;
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.StartTimerOnce();
        }
        // magnitudeを指定して速度を変更
        ballRb.velocity = dir * reboundExitSpeed;
    }


    private IEnumerator HitStop()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.4f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = originalTimeScale;
    }
}
