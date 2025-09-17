using UnityEngine;
using TMPro;

public class PullAndLaunch : MonoBehaviour
{
    [Header("�͂̒���")]
    [SerializeField] private float launchPowerMultiplier = 5f;
    [SerializeField] private float maxDragDistance = 3f;

    [Header("�����ݒ�")]
    [SerializeField] private float gravityValue = 1f;

    [Header("���̌����ڒ���")]
    [SerializeField] private float arrowLengthMultiplier = 1.0f;
    [SerializeField] private float arrowThickness = 1.0f;

    [Header("�S�[���ݒ�")]
    [SerializeField] private GameObject goalTextObject;

    [Header("�I�u�W�F�N�g�Q��")]
    [SerializeField] private Transform arrow;

    // ������ ���̍s��ǉ� ������
    [SerializeField] public LineDrawer lineDrawer;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 startPosition;
    private Vector2 dragVector;
    private bool isDragging = false;
    private bool isGrounded = true;
    private bool isGoal = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        arrow.gameObject.SetActive(false);
        rb.gravityScale = 0;
        goalTextObject.SetActive(false);

        // ������ ���̍s��ǉ� ������
        // �Q�[���J�n���͐��������Ȃ��悤�ɂ���
        if (lineDrawer != null) lineDrawer.enabled = false;
    }

    void Update()
    {
        if (isGoal) return;

        // �v���C���[����~���Ă��āA�n�ʂɐڐG���Ă���Ƃ�
        if (rb.velocity.magnitude < 0.1f && isGrounded)
        {
            // ���������Ȃ��悤�ɂ��ďd�͂��߂�
            if (lineDrawer != null && lineDrawer.enabled)
            {
                lineDrawer.enabled = false;
                rb.gravityScale = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                arrow.gameObject.SetActive(true);
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                arrow.gameObject.SetActive(false);
                Launch();
            }
        }

        if (isDragging)
        {
            UpdateArrow();
        }
    }

    private void Launch()
    {
        rb.gravityScale = gravityValue;

        // ���ˌ�ɐ���������悤�ɂ���
        if (lineDrawer != null) lineDrawer.enabled = true;

        Vector2 launchVector = startPosition - (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // �������苗�����ő�l�𒴂����琧��
        if (launchVector.magnitude > maxDragDistance)
        {
            launchVector = launchVector.normalized * maxDragDistance;
        }

        // Rigidbody2D �ɗ͂������Ĕ���
        rb.AddForce(launchVector * launchPowerMultiplier, ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("���ʂ̏��ɒ��n�I");
        }
        else if (collision.gameObject.CompareTag("Trampoline"))
        {
            Debug.Log("�g�����|�����ɒ��n�I");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Trampoline"))
        {
            isGrounded = false;
        }
    }

    // �����\�b�h�͕ύX�Ȃ�
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGoal) return;
        if (other.gameObject.CompareTag("GoalTop"))
        {
            AchieveGoal();
        }
    }

    private void AchieveGoal()
    {
        isGoal = true;
        goalTextObject.SetActive(true);
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        UnityEngine.Debug.Log("GOAL!");
    }

    private void UpdateArrow()
    {
        Vector2 currentMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        dragVector = currentMousePosition - startPosition;

        if (dragVector.magnitude > maxDragDistance)
        {
            dragVector = dragVector.normalized * maxDragDistance;
        }

        arrow.position = rb.position;
        float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle);
        arrow.localScale = new Vector3(dragVector.magnitude * arrowLengthMultiplier, arrowThickness, 1);
    }
}