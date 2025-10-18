using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BarMovement : MonoBehaviour
{
    [HideInInspector] public bool stopFollow = false;

    [Header("追従軸設定")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = false;
    [SerializeField] private float fixedY;

    [Header("デッドゾーン設定")]
    [SerializeField, Range(0f, 1f)] private float smoothing = 0.25f;
    [SerializeField] private float deadZoneEnter = 0.01f;
    [SerializeField] private float deadZoneExit = 0.01f;

    private Rigidbody2D rb;
    private Vector2 desiredPos;
    private Vector2 filteredTarget;
    private Vector2 holdPos;
    private Vector2 previousPosition;
    private bool inChase = false;
    private Camera mainCamera;

    // 公開プロパティ
    public Vector2 DesiredPosition => desiredPos;
    public Vector2 PreviousPosition => previousPosition;
    public Rigidbody2D Rigidbody => rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.useFullKinematicContacts = true;

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Please tag your camera as 'MainCamera'.");
        }
    }

    void Start()
    {
        fixedY = transform.position.y;
        desiredPos = rb.position;
        filteredTarget = rb.position;
        holdPos = rb.position;
        previousPosition = rb.position;
    }

    void Update()
    {
        if (stopFollow) return;
        if (mainCamera == null) return;

        // === マウス座標をワールド座標に変換 ===
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 target = rb.position;

        if (followX) target.x = mouseWorldPos.x;
        if (followY) target.y = mouseWorldPos.y;
        else target.y = fixedY;

        // デッドゾーン判定
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

        // スムージング
        filteredTarget = Vector2.Lerp(filteredTarget, target, 1f - Mathf.Pow(1f - smoothing, Time.deltaTime * 60f));
        desiredPos = inChase ? filteredTarget : holdPos;
    }

    void FixedUpdate()
    {
        if (stopFollow) return;
        rb.MovePosition(desiredPos);
        previousPosition = desiredPos;
    }

}
