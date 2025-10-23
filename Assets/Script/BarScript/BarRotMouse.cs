using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BarMovement))]
public class BarRotation : MonoBehaviour
{
    [Header("回転設定")]
    [SerializeField] private bool rotateToDirection = true;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMin = 0.01f;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMax = 1f;
    [SerializeField] private float angleDeltaThreshold = 30f;

    [Header("スロー・操作関連")]
    [SerializeField] private RightClickTriggerOn rightClick;
    [SerializeField] private PlayerController playerController;

    private Transform slowMotionTarget;
    private Rigidbody2D rb;
    private BarMovement barMovement;
    private float currentAngle;
    private Vector2 lastPhysicsPos;
    private bool isActivated = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        barMovement = GetComponent<BarMovement>();

        // 自動取得
        if (rightClick == null)
        {
            rightClick = GetComponent<RightClickTriggerOn>();
        }

        // Playerを自動取得
        if (playerController == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
            }
        }
    }

    void Start()
    {
        lastPhysicsPos = rb.position;
        currentAngle = rb.rotation;
    }

    public Vector2 GetForwardDirection()
    {
        float angleRad = currentAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    void Update()
    {
        if (!isActivated && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(mousePos))
            {
                isActivated = true;
                Debug.Log("Bar Activated!");
            }
        }
    }

    void FixedUpdate()
    {
        if (!isActivated) return;

        // デバッグログ追加
        bool isPlayerActive = playerController != null && playerController.isActive;
        bool hasTarget = slowMotionTarget != null;
        bool isRightClickMoving = rightClick != null && rightClick.IsMoving;

        // === スロー中の特殊回転（条件を緩和） ===
        if (hasTarget && Input.GetMouseButton(0) && !isRightClickMoving)
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

                lastPhysicsPos = rb.position; // 位置を更新して通常回転と競合しないように
            }
        }
        // === 通常時：マウス方向に回転 ===
        else if (rotateToDirection && !isRightClickMoving)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;

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
            Debug.Log("SlowMotion Target Set: " + collision.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && slowMotionTarget == collision.transform)
        {
            slowMotionTarget = null;
            Debug.Log("SlowMotion Target Cleared");
        }
    }
}