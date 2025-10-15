using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BarMovement : MonoBehaviour
{
    // 追従停止フラグ
    [HideInInspector] public bool stopFollow = false;

    [Header("仮想マウス参照")]
    [SerializeField] private Transform virtualMouseTransform;

    [Header("追従軸")]
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

    // 他のスクリプトから参照できるプロパティ
    public Vector2 DesiredPosition => desiredPos;
    public Vector2 PreviousPosition => previousPosition;
    public Rigidbody2D Rigidbody => rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.useFullKinematicContacts = true;

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
        fixedY = transform.position.y;
        desiredPos = rb.position;
        filteredTarget = rb.position;
        holdPos = rb.position;
        previousPosition = rb.position;
    }

    void Update()
    {
        if (stopFollow) return;
        if (virtualMouseTransform == null) return;

        // 仮想マウスのワールド座標を取得
        Vector2 virtualMousePos = virtualMouseTransform.position;
        Vector2 target = rb.position;

        if (followX) target.x = virtualMousePos.x;
        if (followY) target.y = virtualMousePos.y; else target.y = fixedY;

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

        filteredTarget = Vector2.Lerp(filteredTarget, target, 1f - Mathf.Pow(1f - smoothing, Time.deltaTime * 60f));
        desiredPos = inChase ? filteredTarget : holdPos;
    }

    void FixedUpdate()
    {
        rb.MovePosition(desiredPos);
        previousPosition = desiredPos;
    }
}