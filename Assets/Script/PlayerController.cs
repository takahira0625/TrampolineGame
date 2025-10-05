using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    // �O�����瓮���𐧌䂷�邽�߂̕ϐ�
    public bool canMove = true;

    public float outTimeToLose = 0.1f; // ��ʊO�ɏo�Ă���Q�[���I�[�o�[�܂ł̗P�\
    private float outTimer = 0f;
    private SpriteRenderer sr;

    [Header("���x����")]
    [Tooltip("�v���C���[���x�̏�� (m/s)�B0�ȉ��Ŗ�����")]
    public float maxSpeed = 40f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ��ʊO����
        if (!sr.isVisible)
        {
            outTimer += Time.deltaTime;
            if (outTimer >= outTimeToLose)
            {
                GameManager.instance.GameOver();
            }
        }
        else
        {
            outTimer = 0f;
            outTimer = 0f;
        }

        if (!canMove)

        if (!canMove)
        {
            rb.velocity = Vector2.zero;
        }
        // �ړ����͏����������ɏ����ꍇ�� FixedUpdate �ł͂Ȃ�������œ��͂̂ݎ擾���A
        // ���x�K�p�� FixedUpdate �ɉ񂷐݌v������
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        // �����ňړ��������s���ꍇ�� rb.velocity ��ݒ肵����ɑ��x����
        // ��i�������j: rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

        // ���x����N�����v
        if (maxSpeed > 0f)
        {
            float maxSq = maxSpeed * maxSpeed;
            Vector2 v = rb.velocity;
            if (v.sqrMagnitude > maxSq)
            {
                rb.velocity = v.normalized * maxSpeed;
            }
        }
    }
}