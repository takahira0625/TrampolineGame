using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BarMovement))]
public class BarRotationTest : MonoBehaviour
{
    [Header("回転設定")]
    [SerializeField] private bool rotateToDirection = true;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMin = 0.01f;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMax = 1f;
    [SerializeField] private float angleDeltaThreshold = 30f;

    [Header("スロー・操作関連")]
    [SerializeField] private RightClick rightClick;
    [SerializeField] private PlayerController playerController;

    private Transform slowMotionTarget;
    private Rigidbody2D rb;
    private BarMovement barMovement;
    private float currentAngle;
    private Vector2 lastPhysicsPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        barMovement = GetComponent<BarMovement>();
    }

    void Start()
    {
        lastPhysicsPos = rb.position;
        currentAngle = rb.rotation;
    }

    /// <summary>
    /// バーの前方向を取得
    /// </summary>
    public Vector2 GetForwardDirection()
    {
        float angleRad = currentAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    void FixedUpdate()
    {
        // === スロー中の特殊回転 ===
        if (playerController != null && playerController.isVelocitySlowed &&
            slowMotionTarget != null && (rightClick == null || !rightClick.IsMoving))
        {
            Vector2 ballDirection = (slowMotionTarget.position - transform.position).normalized;
            if (ballDirection.sqrMagnitude > 0.0001f)
            {
                float targetAngle = Mathf.Atan2(ballDirection.y, ballDirection.x) * Mathf.Rad2Deg - 90f;
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                float normalizedDelta = Mathf.Clamp01(Mathf.Abs(angleDiff) / angleDeltaThreshold);
                float adaptiveSmoothing = Mathf.Lerp(rotationSmoothingMin, rotationSmoothingMax, normalizedDelta);

                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle,
                    1f - Mathf.Pow(1f - adaptiveSmoothing, Time.fixedDeltaTime * 60f));
                rb.MoveRotation(currentAngle);
            }
        }
        // === 通常時：マウス方向に回転 ===
        else if (rotateToDirection && (rightClick == null || !rightClick.IsMoving))
        {
            // マウスのワールド座標を取得
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f; // 2D平面補正

            // 現在のバー位置 → マウス方向ベクトル
            Vector2 delta = (Vector2)mouseWorldPos - lastPhysicsPos;

            if (delta.sqrMagnitude > 0.0001f)
            {
                float targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90f;
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                float normalizedDelta = Mathf.Clamp01(Mathf.Abs(angleDiff) / angleDeltaThreshold);
                float adaptiveSmoothing = Mathf.Lerp(rotationSmoothingMin, rotationSmoothingMax, normalizedDelta);

                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle,
                    1f - Mathf.Pow(1f - adaptiveSmoothing, Time.fixedDeltaTime * 60f));
                rb.MoveRotation(currentAngle);
            }

            lastPhysicsPos = rb.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            slowMotionTarget = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && slowMotionTarget == collision.transform)
        {
            slowMotionTarget = null;
        }
    }
}
