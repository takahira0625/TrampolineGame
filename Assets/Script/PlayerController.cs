using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    // �O�����瓮���𐧌䂷�邽�߂̕ϐ�
    public bool canMove = true;

    public float outTimeToLose = 0.1f; // ��ʊO�ɏo�Ă���Q�[���I�[�o�[�܂ł̗P�\
    private float outTimer = 0f;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            outTimer = 0f;
        }

        if (!canMove)
        {
            rb.velocity = Vector2.zero;
        }
    }
}