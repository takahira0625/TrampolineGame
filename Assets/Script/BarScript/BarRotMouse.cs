using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BarMovement))]
public class BarRotation : MonoBehaviour
{
    [Header("仮想マウス参照")]
    [SerializeField] private Transform virtualMouseTransform;

    [Header("回転設定")]
    [SerializeField] private bool rotateToDirection = true;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMin = 0.01f;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMax = 1f;
    [SerializeField] private float angleDeltaThreshold = 30f;

    // Inspectorでアサイン
    [SerializeField] private RightClick rightClick;
    [SerializeField] private PlayerController playerController;
    private Transform slowMotionTarget; // スロー中に向く対象

    private Rigidbody2D rb;
    private BarMovement barMovement;
    private float currentAngle;
    private Vector2 lastPhysicsPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        barMovement = GetComponent<BarMovement>();

        // 仮想マウスが指定されていない場合は自動検索
        if (virtualMouseTransform == null)
        {
            VirtualMouse virtualMouse = FindObjectOfType<VirtualMouse>();
            if (virtualMouse != null)
            {
                virtualMouseTransform = virtualMouse.transform;
            }
            else
            {
                Debug.LogError("VirtualMouse not found! Please assign it in the inspector.");
            }
        }
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
        if (playerController.IsInSlowMotion && slowMotionTarget != null && (rightClick == null || !rightClick.IsMoving))
        {
            Vector2 ballDirection = (slowMotionTarget.position - transform.position).normalized;
            if (ballDirection.sqrMagnitude > 0.0001f) {
                float targetAngle = Mathf.Atan2(ballDirection.y, ballDirection.x) * Mathf.Rad2Deg - 90;
                // 現在の角度と目標角度を滑らかに補間
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                float normalizedDelta = Mathf.Clamp01(Mathf.Abs(angleDiff) / angleDeltaThreshold);
                float adaptiveSmoothing = Mathf.Lerp(rotationSmoothingMin, rotationSmoothingMax, normalizedDelta);

                // 滑らかに更新
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 1f - Mathf.Pow(1f - adaptiveSmoothing, Time.fixedDeltaTime * 60f));
                rb.MoveRotation(currentAngle);
            }
        }
        else if (rotateToDirection && (rightClick == null || !rightClick.IsMoving))
        {

            // 通常の回転処理（元のコード）
            Vector2 desiredPos = barMovement.DesiredPosition;
            Vector2 delta = desiredPos - lastPhysicsPos;

            if (delta.sqrMagnitude > 0.0001f)
            {
                float targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90f;
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                float normalizedDelta = Mathf.Clamp01(Mathf.Abs(angleDiff) / angleDeltaThreshold);
                float adaptiveSmoothing = Mathf.Lerp(rotationSmoothingMin, rotationSmoothingMax, normalizedDelta);
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 1f - Mathf.Pow(1f - adaptiveSmoothing, Time.fixedDeltaTime * 60f));
                rb.MoveRotation(currentAngle);
            }

            lastPhysicsPos = desiredPos;
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