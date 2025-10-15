using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BarMovement))]
public class BarRotation : MonoBehaviour
{
    [Header("���z�}�E�X�Q��")]
    [SerializeField] private Transform virtualMouseTransform;

    [Header("��]�ݒ�")]
    [SerializeField] private bool rotateToDirection = true;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMin = 0.01f;
    [SerializeField, Range(0f, 1f)] private float rotationSmoothingMax = 1f;
    [SerializeField] private float angleDeltaThreshold = 30f;

    // Inspector�ŃA�T�C��
    [SerializeField] private RightClick rightClick;
    [SerializeField] private PlayerController playerController;
    private Transform slowMotionTarget; // �X���[���Ɍ����Ώ�

    private Rigidbody2D rb;
    private BarMovement barMovement;
    private float currentAngle;
    private Vector2 lastPhysicsPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        barMovement = GetComponent<BarMovement>();

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
        lastPhysicsPos = rb.position;
        currentAngle = rb.rotation;
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
        if (playerController.IsInSlowMotion && slowMotionTarget != null && (rightClick == null || !rightClick.IsMoving))
        {
            Vector2 ballDirection = (slowMotionTarget.position - transform.position).normalized;
            if (ballDirection.sqrMagnitude > 0.0001f) {
                float targetAngle = Mathf.Atan2(ballDirection.y, ballDirection.x) * Mathf.Rad2Deg - 90;
                // ���݂̊p�x�ƖڕW�p�x�����炩�ɕ��
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                float normalizedDelta = Mathf.Clamp01(Mathf.Abs(angleDiff) / angleDeltaThreshold);
                float adaptiveSmoothing = Mathf.Lerp(rotationSmoothingMin, rotationSmoothingMax, normalizedDelta);

                // ���炩�ɍX�V
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 1f - Mathf.Pow(1f - adaptiveSmoothing, Time.fixedDeltaTime * 60f));
                rb.MoveRotation(currentAngle);
            }
        }
        else if (rotateToDirection && (rightClick == null || !rightClick.IsMoving))
        {

            // �ʏ�̉�]�����i���̃R�[�h�j
            Vector2 desiredPos = barMovement.DesiredPosition;
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

            lastPhysicsPos = desiredPos;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            slowMotionTarget = collision.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && slowMotionTarget == collision.transform)
        {
            slowMotionTarget = null;
        }
    }
}