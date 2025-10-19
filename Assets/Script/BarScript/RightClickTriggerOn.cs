using System.Collections;
using UnityEngine;

public class RightClickTriggerOn : MonoBehaviour
{
    [SerializeField] private BarMovement barFollow;
    [Header("�O�i�����Ǝ���")]
    [SerializeField] private float forwardDistance = 10.0f;
    [SerializeField] private float forwardTime = 0.1f;
    [SerializeField] private float returnTime = 0.3f;

    [Header("���ːݒ�")]
    [SerializeField] private float reboundCoefficient = 0.7f;
    [SerializeField] private float reboundExitSpeed = 25f; // Exit���ɔ�΂����x
    [SerializeField] private float pushSpeed = 20f;

    [SerializeField, Header("�q�b�g�X�g�b�v�ݒ�")]
    private float hitStopDuration = 0.1f;

    private bool isMoving = false;
    public bool IsMoving => isMoving;
    private bool hasHitThisPush = false;

    private Vector2 originalPosition;
    private Quaternion startRotation;
    private Collider2D col;
    private Rigidbody2D rb;

    // ���˃f�[�^�̋L�^
    private Rigidbody2D lastBallRb;
    private Vector2 lastNormal;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isMoving && !col.isTrigger)
        {
            startRotation = transform.rotation;
            StartCoroutine(MoveForwardAndBack());
        }
    }

    private IEnumerator MoveForwardAndBack()
    {
        if (barFollow != null)
            barFollow.stopFollow = true;

        isMoving = true;
        col.isTrigger = true;

        originalPosition = rb.position;
        Vector2 forward = (startRotation * Vector2.up).normalized;
        Vector2 targetForwardPos = originalPosition + forward * forwardDistance;

        float elapsed = 0f;

        // --- �O�i ---
        while (elapsed < forwardTime)
        {
            Vector2 newPos = Vector2.Lerp(originalPosition, targetForwardPos, elapsed / forwardTime);
            rb.MovePosition(newPos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(targetForwardPos);

        // --- �߂� ---
        elapsed = 0f;
        while (elapsed < returnTime)
        {
            Vector2 newPos = Vector2.Lerp(targetForwardPos, originalPosition, elapsed / returnTime);
            rb.MovePosition(newPos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(originalPosition);

        isMoving = false;
        col.isTrigger = false;
        hasHitThisPush = false;
        if (barFollow != null) barFollow.stopFollow = false;
    }

    // --- Trigger���̉����o�� ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.IsChildOf(transform)) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D ballRb = other.attachedRigidbody;
        PlayerController playerCtrl = other.GetComponent<PlayerController>();
        if (ballRb == null || playerCtrl == null || (isMoving && hasHitThisPush)) return;

        // --- �����o���� ---
        if (isMoving)
        {
            hasHitThisPush = true;
            Vector2 forward = transform.up.normalized;

            if (playerCtrl.isActive)
            {
                StartCoroutine(HitStop());
                Debug.Log("�q�b�g�X�g�b�v: Active�����o��");

                float savedSpeed = playerCtrl.savedVelocity.magnitude;
                ballRb.velocity = forward * Mathf.Max(savedSpeed * 1.1f, pushSpeed + 1);

                playerCtrl.isActive = false;
            }
            else
            {
                Debug.Log("��Active�����o��");
                ballRb.velocity = forward * pushSpeed;
            }
        }
    }

    // --- Exit�Ŗ@�������ɔ�΂� ---
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isMoving) return; // Trigger���̓X�L�b�v

        Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (ballRb == null) return;

        Vector2 dir = ballRb.velocity.normalized;

        // magnitude���w�肵�đ��x��ύX
        ballRb.velocity = dir * reboundExitSpeed;
    }


    private IEnumerator HitStop()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = originalTimeScale;
    }
}
