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
    [SerializeField] bool rotateToDirection = false;
    [SerializeField, Range(0f, 1f)] float rotationSmoothing = 0.15f;

    [Header("�{�[�����͂����ݒ�")]
    [Tooltip("�o�[�̑��x���{�[���ɓ`����{��")]
    [SerializeField] float hitForceMultiplier = 1.5f;
    [Tooltip("�͂����ۂ̍ŏ��o�[���x�i����ȉ����ƒʏ�̔��ˁj")]
    [SerializeField] float minHitSpeed = 2f;
    [Tooltip("�͂����ۂ̍ő�́i�����ɉ������Ȃ��悤�ɐ����j")]
    [SerializeField] float maxHitForce = 50f;
    [Tooltip("���N���b�N�������݂̂͂����@�\��L����")]
    [SerializeField] bool requireLeftClick = true;

    [Header("�Ǐ]����")]
    [Tooltip("���N���b�N���ɒǏ]���~����")]
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
    private Vector2 frozenPosition; // �� �ǉ�: ���N���b�N���̌Œ�ʒu
    
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
        frozenPosition = rb.position; // �� �ǉ�
    }

    void Update()
    {
        if (!cam) return;

        // �� �ǉ�: ���N���b�N���͒Ǐ]���~
        if (stopFollowOnLeftClick && Input.GetMouseButton(0))
        {
            // ���N���b�N�����J�n���Ɍ��݈ʒu���L�^
            if (Input.GetMouseButtonDown(0))
            {
                frozenPosition = rb.position;
            }
            
            // �Œ�ʒu��ڕW�ʒu�ɐݒ�
            desiredPos = frozenPosition;
            return; // �ȍ~�̒Ǐ]�������X�L�b�v
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

        Debug.Log($"�{�[�����͂������I �o�[���x: {barSpeed:F2}, ��: {hitForce:F2}, ����: {hitDirection}");
    }
}
