using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BarFollowMouse : MonoBehaviour
{
    // �Ǐ]��~�t���O
    [HideInInspector] public bool stopFollow = false;

    [Header("���z�}�E�X�Q��")]
    [SerializeField] private Transform virtualMouseTransform;

    [Header("�Ǐ]��")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = false;
    [SerializeField] private float fixedY;

    [Header("��]�ݒ�")]
    [SerializeField] private bool rotateToDirection = true;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMin = 0.01f;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMax = 1f;
    [SerializeField] private float angleDeltaThreshold = 30f;

    [Header("�f�b�h�]�[���ݒ�")]
    [SerializeField, Range(0f, 1f)] private float smoothing = 0.25f;
    [SerializeField] private float deadZoneEnter = 0.01f;
    [SerializeField] private float deadZoneExit = 0.01f;

    private Rigidbody2D rb;
    private Vector2 desiredPos;
    private Vector2 filteredTarget;
    private Vector2 holdPos;
    private Vector2 lastPhysicsPos;
    private float currentAngle;
    private bool inChase = false;
    private Vector2 previousPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.useFullKinematicContacts = true;

        // ���z�}�E�X���w�肳��Ă��Ȃ��ꍇ�͎�������
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
        lastPhysicsPos = rb.position;
        previousPosition = rb.position;
        currentAngle = rb.rotation;
    }

    void Update()
    {
        if (stopFollow) return;
        if (virtualMouseTransform == null) return;

        // ���z�}�E�X�̃��[���h���W���擾
        Vector2 virtualMousePos = virtualMouseTransform.position;
        Vector2 target = rb.position;

        if (followX) target.x = virtualMousePos.x;
        if (followY) target.y = virtualMousePos.y; else target.y = fixedY;

        // �f�b�h�]�[������
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

    /// <summary>
    /// �o�[�̑O�������擾
    /// </summary>
    public Vector2 GetForwardDirection()
    {
        float angleRad = currentAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    void FixedUpdate()
    {
        Vector2 barVelocity = (desiredPos - previousPosition) / Time.fixedDeltaTime;

        if (rotateToDirection)
        {
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
        }

        rb.MovePosition(desiredPos);
        lastPhysicsPos = desiredPos;
        previousPosition = desiredPos;
    }
}