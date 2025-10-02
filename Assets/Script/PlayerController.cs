using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    // �O�����瓮���𐧌䂷�邽�߂̕ϐ�
    public bool canMove = true;

    public float outTimeToLose = 0.01f; // ��ʊO�ɏo�Ă���Q�[���I�[�o�[�܂ł̗P�\
    private float outTimer = 0f;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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
            outTimer = 0f; // ��ʓ��ɖ߂����烊�Z�b�g
        }
        // canMove��true�̎������ړ��������s��
        if (canMove)
        {
           
        }
        else
        {
            // �������~�߂�
            rb.velocity = Vector2.zero;
        }
    }
}