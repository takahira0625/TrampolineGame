using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BarController : MonoBehaviour
{
    [Header("�Ǐ]��")]
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = false;
    [SerializeField] float fixedY;

    [Header("�͈͐����i�C�Ӂj")]
    [SerializeField] bool useBounds = false;
    [SerializeField] Vector2 minPos = new Vector2(-8f, -4f);
    [SerializeField] Vector2 maxPos = new Vector2(8f, 4f);

    [Header("��]�i�C�Ӂj")]
    [SerializeField] bool rotateToDirection = false; // �s�b�^���Ǐ]�Ȃ��{OFF����
    [SerializeField, Range(0f, 1f)] float rotationSmoothing = 0.15f; // 0=����, 1=���������

    Camera cam;
    Rigidbody2D rb;
    Vector2 desiredPos;     // Update�Ō��߂āAFixedUpdate�ő�����MovePosition
    Vector2 lastPhysicsPos; // ��]�p
    float currentAngle;     // ���݂̊p�x�i��ԗp�j

    [SerializeField] float deadZoneEnter = 0.02f; // �Ǐ]�J�n�������l�i���[���h���W�j
    [SerializeField] float deadZoneExit = 0.01f; // �Ǐ]��~�������l�i�����߁j
    [SerializeField, Range(0f, 1f)] float smoothing = 0.25f; // 0=���̓���, 1=���܂�����
    bool inChase = false;
    Vector2 holdPos; // ���߂̐Î~�ʒu
    Vector2 filteredTarget;
    
    void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // �����ڂ̒x���␳
        rb.useFullKinematicContacts = true;
    }

    void Start()
    {
        fixedY = transform.position.y;
        desiredPos = rb.position;
        lastPhysicsPos = rb.position;
        holdPos = rb.position;
        filteredTarget = rb.position;
        currentAngle = rb.rotation; // �����p�x���L�^
    }

    void Update()
    {
        if (!cam) return;

        // �}�E�X�����[���h
        var mp = Input.mousePosition;
        mp.z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        var world = cam.ScreenToWorldPoint(mp);

        var target = rb.position; // ���݂�����

        if (followX) target.x = world.x;
        if (followY) target.y = world.y; else target.y = fixedY;

        if (useBounds)
        {
            target.x = Mathf.Clamp(target.x, minPos.x, maxPos.x);
            target.y = Mathf.Clamp(target.y, minPos.y, maxPos.y);
        }
        float d = Vector2.Distance(holdPos, target);

        // �Ǐ]�J�n/��~�̃q�X�e���V�X
        if (!inChase && d >= deadZoneEnter)
        {
            inChase = true;
        }
        else if (inChase && d <= deadZoneExit)
        {
            inChase = false;
            holdPos = target;          // �V�����Î~���S���X�V
        }
        filteredTarget = Vector2.Lerp(filteredTarget, target, 1f - Mathf.Pow(1f - smoothing, Time.deltaTime * 60f));
        desiredPos = inChase ? filteredTarget : holdPos; // �h���Ƃ��� holdPos �ɌŒ�

    }

    void FixedUpdate()
    {
        // �������݂ő����Ɏw��ʒu�ցi�X�s�[�h�����Ȃ��j
        if (rotateToDirection)
        {
            Vector2 delta = desiredPos - lastPhysicsPos;
            if (delta.sqrMagnitude > 0.005f)
            {
                // �ڕW�p�x���v�Z
                float targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90f;
                
                // ���݂̊p�x����ڕW�p�x�֊��炩�ɕ��
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 1f - Mathf.Pow(1f - rotationSmoothing, Time.fixedDeltaTime * 60f));
                
                rb.MoveRotation(currentAngle);
            }
        }
        rb.MovePosition(desiredPos);

        lastPhysicsPos = desiredPos;
    }
}
