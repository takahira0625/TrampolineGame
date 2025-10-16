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

    [SerializeField] Vector2 minPos = new Vector2(-1f, -1f);
    [SerializeField] Vector2 maxPos = new Vector2(1f, 1f);

    [Header("回転設定")]
    [SerializeField] bool rotateToDirection = false;
    [SerializeField, Range(0f, 1f)] float rotationSmoothingMin = 0.01f; // 小さい角度変化時の補間速度
    [SerializeField, Range(0f, 1f)] float rotationSmoothingMax = 1f;  // 大きい角度変化時の補間速度
    [SerializeField] float angleDeltaThreshold = 30f; // この角度差以上で素早く回転
    [Header("ボール反発設定")]
    [Tooltip("バーの速度をボールに加える倍率")]
    [SerializeField] float hitForceMultiplier = 1.5f;

    [Tooltip("バーの速度がこの値未満の場合は反発しない")]
    [SerializeField] float minHitSpeed = 2f;

    [Tooltip("バーが与える最大反発力（上限）")]
    [SerializeField] float maxHitForce = 50f;

    [Tooltip("左クリック中のみ反発を有効にする")]
    [SerializeField] bool requireLeftClick = true;

    [Header("追従挙動設定")]
    [Tooltip("左クリック中は追従を停止する")]
    [SerializeField] bool stopFollowOnLeftClick = true;

    [Tooltip("クリック後、追従を再開する際のスムージング（0＝ゆっくり、1＝瞬時）")]
    [SerializeField, Range(0f, 1f)] float releaseSmoothing = 0.3f;

    Camera cam;
    Rigidbody2D rb;
    Vector2 desiredPos;
    Vector2 lastPhysicsPos;
    float currentAngle;

    [Header("デッドゾーン設定")]
    [SerializeField] float deadZoneEnter = 0.01f;
    [SerializeField] float deadZoneExit = 0.01f;
    [SerializeField, Range(0f, 1f)] float smoothing = 0.25f;
    bool inChase = false;
    Vector2 holdPos;
    Vector2 filteredTarget;


    private Vector2 barVelocity;
    private Vector2 previousPosition;
    private Vector2 frozenPosition;
    private bool isReturningToMouse = false;
    private Vector2 returnStartPos;
    
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
        frozenPosition = rb.position;
    }
    void Update()
    {
        if (!cam) return;

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

        if (stopFollowOnLeftClick && Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                frozenPosition = rb.position;
                isReturningToMouse = false;
            }
            
            desiredPos = frozenPosition;
            return;
        }

        if (stopFollowOnLeftClick && Input.GetMouseButtonUp(0))
        {
            isReturningToMouse = true;
            returnStartPos = rb.position;
            filteredTarget = rb.position;
        }

        if (isReturningToMouse)
        {
            filteredTarget = Vector2.Lerp(filteredTarget, target, 
                1f - Mathf.Pow(1f - releaseSmoothing, Time.deltaTime * 60f));
            
            desiredPos = filteredTarget;
            
            float distanceToTarget = Vector2.Distance(filteredTarget, target);
            if (distanceToTarget < 2f)
            {
                isReturningToMouse = false;
                inChase = false;
                holdPos = target;
            }
            return;
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
                // 目標角度を計算
                float targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90f;

                // 現在の角度と目標角度の差を計算（-180〜180の範囲に正規化）
                float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

                // 角度差に応じて補間速度を調整
                float normalizedDelta = Mathf.Clamp01(Mathf.Abs(angleDifference) / angleDeltaThreshold);
                float adaptiveSmoothing = Mathf.Lerp(rotationSmoothingMin, rotationSmoothingMax, normalizedDelta);

                // 滑らかに補間
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 1f - Mathf.Pow(1f - adaptiveSmoothing, Time.fixedDeltaTime * 60f));


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
}