using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BarController : MonoBehaviour
{
    [Header("追従軸")]
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = false;
    [SerializeField] float fixedY;

    [Header("範囲制限（任意）")]
    [SerializeField] bool useBounds = false;
    [SerializeField] Vector2 minPos = new Vector2(-8f, -4f);
    [SerializeField] Vector2 maxPos = new Vector2(8f, 4f);

    [Header("回転（任意）")]
    [SerializeField] bool rotateToDirection = false;
    [SerializeField, Range(0f, 1f)] float rotationSmoothing = 0.15f;

    Camera cam;
    Rigidbody2D rb;
    Vector2 desiredPos;
    Vector2 lastPhysicsPos;
    float currentAngle;

    [SerializeField] float deadZoneEnter = 0.02f;
    [SerializeField] float deadZoneExit = 0.01f;
    [SerializeField, Range(0f, 1f)] float smoothing = 0.25f;
    bool inChase = false;
    Vector2 holdPos;
    Vector2 filteredTarget;
    
    void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.useFullKinematicContacts = true;
    }

    void Start()
    {
        fixedY = transform.position.y;
        desiredPos = rb.position;
        lastPhysicsPos = rb.position;
        holdPos = rb.position;
        filteredTarget = rb.position;
        currentAngle = rb.rotation;
    }

    void Update()
    {
        if (!cam) return;

        // マウス→ワールド
        var mp = Input.mousePosition;
        mp.z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        var world = cam.ScreenToWorldPoint(mp);

        var target = rb.position;

        if (followX) target.x = world.x;
        if (followY) target.y = world.y; else target.y = fixedY;

        if (useBounds)
        {
            target.x = Mathf.Clamp(target.x, minPos.x, maxPos.x);
            target.y = Mathf.Clamp(target.y, minPos.y, maxPos.y);
        }
        float d = Vector2.Distance(holdPos, target);

        if (!inChase && d >= deadZoneEnter)
        {
            inChase = true;
        }
        else if (inChase && d <= deadZoneExit)
        {
            inChase = false;
            holdPos = target;
        }
        // ★ 修正: Time.unscaledDeltaTime を使用（スローモーションの影響を受けない）
        filteredTarget = Vector2.Lerp(filteredTarget, target, 1f - Mathf.Pow(1f - smoothing, Time.unscaledDeltaTime * 60f));
        desiredPos = inChase ? filteredTarget : holdPos;
    }

    void FixedUpdate()
    {
        if (rotateToDirection)
        {
            Vector2 delta = desiredPos - lastPhysicsPos;
            if (delta.sqrMagnitude > 0.005f)
            {
                float targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90f;
                
                // ★ 修正: Time.unscaledDeltaTime を使用
                // FixedUpdate内なので fixedUnscaledDeltaTime の方が正確ですが、
                // Time.fixedDeltaTime が timeScale の影響を受けるため、
                // スローモーション時は手動で計算
                float deltaTime = Time.timeScale > 0 ? Time.fixedDeltaTime / Time.timeScale : 0.02f;
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 
                    1f - Mathf.Pow(1f - rotationSmoothing, deltaTime * 60f));
                
                rb.MoveRotation(currentAngle);
            }
        }
        rb.MovePosition(desiredPos);

        lastPhysicsPos = desiredPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.StartTimerOnce();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.StartTimerOnce();
            }
        }
    }
}
