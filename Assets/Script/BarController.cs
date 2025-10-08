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

    [Header("ボールをはじく設定")]
    [Tooltip("バーの速度をボールに伝える倍率")]
    [SerializeField] float hitForceMultiplier = 1.5f;
    [Tooltip("はじく際の最小バー速度（これ以下だと通常の反射）")]
    [SerializeField] float minHitSpeed = 2f;
    [Tooltip("はじく際の最大力（無限に加速しないように制限）")]
    [SerializeField] float maxHitForce = 50f;
    [Tooltip("左クリック押下中のみはじく機能を有効化")]
    [SerializeField] bool requireLeftClick = true;

    [Header("追従制御")]
    [Tooltip("左クリック中に追従を停止する")]
    [SerializeField] bool stopFollowOnLeftClick = true;

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

    private Vector2 barVelocity;
    private Vector2 previousPosition;
    private Vector2 frozenPosition; // ★ 追加: 左クリック時の固定位置
    
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
        previousPosition = rb.position;
        frozenPosition = rb.position; // ★ 追加
    }

    void Update()
    {
        if (!cam) return;

        // ★ 追加: 左クリック中は追従を停止
        if (stopFollowOnLeftClick && Input.GetMouseButton(0))
        {
            // 左クリック押下開始時に現在位置を記録
            if (Input.GetMouseButtonDown(0))
            {
                frozenPosition = rb.position;
            }
            
            // 固定位置を目標位置に設定
            desiredPos = frozenPosition;
            return; // 以降の追従処理をスキップ
        }

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
        filteredTarget = Vector2.Lerp(filteredTarget, target, 1f - Mathf.Pow(1f - smoothing, Time.deltaTime * 60f));
        desiredPos = inChase ? filteredTarget : holdPos;
    }

    void FixedUpdate()
    {
        barVelocity = (desiredPos - previousPosition) / Time.fixedDeltaTime;

        if (rotateToDirection)
        {
            Vector2 delta = desiredPos - lastPhysicsPos;
            if (delta.sqrMagnitude > 0.005f)
            {
                float targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90f;
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 
                    1f - Mathf.Pow(1f - rotationSmoothing, Time.fixedDeltaTime * 60f));
                
                rb.MoveRotation(currentAngle);
            }
        }
        rb.MovePosition(desiredPos);

        lastPhysicsPos = desiredPos;
        previousPosition = desiredPos;
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

            ApplyHitForce(collision);
        }
    }

    private void ApplyHitForce(Collision2D collision)
    {
        if (requireLeftClick && !Input.GetMouseButton(0))
        {
            return;
        }

        float barSpeed = barVelocity.magnitude;
        if (barSpeed < minHitSpeed)
        {
            return;
        }

        Rigidbody2D ballRb = collision.rigidbody;
        if (ballRb == null) return;

        Vector2 hitDirection = barVelocity.normalized;
        float hitForce = Mathf.Min(barSpeed * hitForceMultiplier, maxHitForce);

        Vector2 newVelocity = ballRb.velocity + hitDirection * hitForce;
        ballRb.velocity = newVelocity;

        Debug.Log($"ボールをはじいた！ バー速度: {barSpeed:F2}, 力: {hitForce:F2}, 方向: {hitDirection}");
    }
}
