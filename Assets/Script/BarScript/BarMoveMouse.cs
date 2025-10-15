using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BarMovement : MonoBehaviour
{
    // �Ǐ]��~�t���O
    [HideInInspector] public bool stopFollow = false;

    [Header("���z�}�E�X�Q��")]
    [SerializeField] private Transform virtualMouseTransform;

    [Header("�Ǐ]��")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = false;
    [SerializeField] private float fixedY;

    [Header("�f�b�h�]�[���ݒ�")]
    [SerializeField, Range(0f, 1f)] private float smoothing = 0.25f;
    [SerializeField] private float deadZoneEnter = 0.01f;
    [SerializeField] private float deadZoneExit = 0.01f;

    private Rigidbody2D rb;
    private Vector2 desiredPos;
    private Vector2 filteredTarget;
    private Vector2 holdPos;
    private Vector2 previousPosition;
    private bool inChase = false;

    // ���̃X�N���v�g����Q�Ƃł���v���p�e�B
    public Vector2 DesiredPosition => desiredPos;
    public Vector2 PreviousPosition => previousPosition;
    public Rigidbody2D Rigidbody => rb;

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
        previousPosition = rb.position;
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

    void FixedUpdate()
    {
        rb.MovePosition(desiredPos);
        previousPosition = desiredPos;
    }
}