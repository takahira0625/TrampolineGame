using System.Collections;
using UnityEngine;
using UnityEngine.XR;

//Trigger2Dにしたらヒットストップの動機ずれが解消できるかも
public class RightClick : MonoBehaviour
{

    [SerializeField] private BarMovement barFollow;
    [Header("前進距離と時間")]
    [SerializeField] private float forwardDistance = 20.0f;
    [SerializeField] private float forwardTime = 0.1f;
    [SerializeField] private float returnTime = 0.3f;
    [Header("反射設定")]
    [SerializeField] private float reboundCoefficient = 0.7f;
    [SerializeField] private float pushSpeed = 20f;
    [SerializeField, Header("ヒットストップ設定")]
    private float hitStopDuration = 0.1f; // ヒットストップの時間（秒）
    //private float StartHitStopTime = 0.03f; // ヒットストップ開始までの時間（秒）
    private bool isMoving = false;
    public bool IsMoving => isMoving;
    private bool hasHitThisPush = false;

    private Vector2 originalPosition;
    private Quaternion startRotation;
    private Rigidbody2D rb;

    // ボール速度の遅延適用
    private Rigidbody2D pendingBallRb = null;
    private Vector2 pendingVelocity = Vector2.zero;

    private AudioClip SmashSE;
    private AudioClip CollisionSE;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        LoadSmashSE();
        LoadCollisionSE();

        // ✅ SEManager初期化確認
        if (SEManager.Instance == null)
        {
            Debug.LogError("SEManagerが見つかりません！");
        }
        else
        {
            Debug.Log("SEManagerが正常に初期化されました");
        }
    }

    void Update()
    {
        if (InputManager.IsRightClickPressed() && !isMoving)
        {
            startRotation = transform.rotation;
            StartCoroutine(MoveForwardAndBack());
        }
    }

    private IEnumerator MoveForwardAndBack()
    {
        if (barFollow != null)
        {
            barFollow.stopFollow = true;
        }

        isMoving = true;

        originalPosition = rb.position;
        Vector2 forward = (startRotation * Vector2.up).normalized;
        Vector2 targetForwardPos = originalPosition + forward * forwardDistance;
        float elapsed = 0f;
        
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
        hasHitThisPush = false;
        if (barFollow != null) barFollow.stopFollow = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        PlayerController playerCtrl = collision.gameObject.GetComponent<PlayerController>();
        if (ballRb == null || playerCtrl == null) return;
        
        pendingBallRb = ballRb;
        if (isMoving && hasHitThisPush) return; // 一度押し出し済みなら無視

        Vector2 normal = collision.contacts[0].normal;

        // 押し出し中なら
        if (isMoving)
        {
            hasHitThisPush = true;
            Vector2 forward = transform.up.normalized;

            // Active化されているなら
            if (playerCtrl.isActive)
            {
                StartCoroutine(HitStop());
                Debug.Log("ヒットストップ - Active押し出し2");
                PlaySE(SmashSE, "CollisionSE");
                float savedSpeed = playerCtrl.savedVelocity.magnitude;
                
                if (savedSpeed > pushSpeed)
                {
                    pendingVelocity = forward * (savedSpeed * 1.1f);
                }
                else
                {
                    pendingVelocity = forward * (pushSpeed + 1);
                }
                
                playerCtrl.isActive = false;
                Debug.Log("Active解除");
                
            }
            else // 非Active化だけど押し出しはしている
            {
                Debug.Log("非Active押し出し");
                
                // ✅ SEを再生(nullチェック追加)
                PlaySE(CollisionSE, "CollisionSE");
                
                pendingVelocity = forward * pushSpeed;
            }
        }
        else  // 通常の反射
        {
            Debug.Log("通常反射");

            // ✅ SEを再生(nullチェック追加)
            PlaySE(CollisionSE, "CollisionSE");

            Vector2 reflected = Vector2.Reflect(ballRb.velocity, normal);
            pendingVelocity = reflected * reboundCoefficient;
        }
    }

    /// <summary>
    /// SEを安全に再生する
    /// </summary>
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

    private IEnumerator HitStop()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f; // ゲームを止める
        yield return new WaitForSecondsRealtime(hitStopDuration); // リアルタイムで待つ
        Time.timeScale = originalTimeScale; // 元に戻す
        if (pendingBallRb != null)
        {
            pendingBallRb.velocity = pendingVelocity;
        }
    }
}