using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    // �O�����瓮���𐧌䂷�邽�߂̕ϐ�
    public bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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