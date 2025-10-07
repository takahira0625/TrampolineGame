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
    [SerializeField] bool rotateToDirection = false; // ピッタリ追従なら基本OFF推奨
    [SerializeField, Range(0f, 1f)] float rotationSmoothing = 0.15f; // 0=即座, 1=超ゆっくり

    Camera cam;
    Rigidbody2D rb;
    Vector2 desiredPos;     // Updateで決めて、FixedUpdateで即座にMovePosition
    Vector2 lastPhysicsPos; // 回転用
    float currentAngle;     // 現在の角度（補間用）

    [SerializeField] float deadZoneEnter = 0.02f; // 追従開始しきい値（ワールド座標）
    [SerializeField] float deadZoneExit = 0.01f; // 追従停止しきい値（小さめ）
    [SerializeField, Range(0f, 1f)] float smoothing = 0.25f; // 0=生の入力, 1=超まったり
    bool inChase = false;
    Vector2 holdPos; // 直近の静止位置
    Vector2 filteredTarget;
    
    void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // 見た目の遅れを補正
        rb.useFullKinematicContacts = true;
    }

    void Start()
    {
        fixedY = transform.position.y;
        desiredPos = rb.position;
        lastPhysicsPos = rb.position;
        holdPos = rb.position;
        filteredTarget = rb.position;
        currentAngle = rb.rotation; // 初期角度を記録
    }

    void Update()
    {
        if (!cam) return;

        // マウス→ワールド
        var mp = Input.mousePosition;
        mp.z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        var world = cam.ScreenToWorldPoint(mp);

        var target = rb.position; // 現在から作る

        if (followX) target.x = world.x;
        if (followY) target.y = world.y; else target.y = fixedY;

        if (useBounds)
        {
            target.x = Mathf.Clamp(target.x, minPos.x, maxPos.x);
            target.y = Mathf.Clamp(target.y, minPos.y, maxPos.y);
        }
        float d = Vector2.Distance(holdPos, target);

        // 追従開始/停止のヒステリシス
        if (!inChase && d >= deadZoneEnter)
        {
            inChase = true;
        }
        else if (inChase && d <= deadZoneExit)
        {
            inChase = false;
            holdPos = target;          // 新しい静止中心を更新
        }
        filteredTarget = Vector2.Lerp(filteredTarget, target, 1f - Mathf.Pow(1f - smoothing, Time.deltaTime * 60f));
        desiredPos = inChase ? filteredTarget : holdPos; // 揺れるときは holdPos に固定

    }

    void FixedUpdate()
    {
        // 物理刻みで即座に指定位置へ（スピード制限なし）
        if (rotateToDirection)
        {
            Vector2 delta = desiredPos - lastPhysicsPos;
            if (delta.sqrMagnitude > 0.005f)
            {
                // 目標角度を計算
                float targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90f;
                
                // 現在の角度から目標角度へ滑らかに補間
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 1f - Mathf.Pow(1f - rotationSmoothing, Time.fixedDeltaTime * 60f));
                
                rb.MoveRotation(currentAngle);
            }
        }
        rb.MovePosition(desiredPos);

        lastPhysicsPos = desiredPos;
    }
}
