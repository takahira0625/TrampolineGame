using UnityEngine;

//�������͎g���Ă��܂���B

public class TrampolineScript : MonoBehaviour
{
    public float bounceForce = 20f; // ���˕Ԃ��͂̋���

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("�Փˌ��m�I �����: " + collision.gameObject.name);
        // �Փ˂��������Rigidbody2D���擾
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // �Փ˂�������̌��݂̑��x�����Z�b�g���A�@���x�N�g�������i�Փ˖ʂ��琂��������j�ɗ͂�������
            rb.velocity = Vector2.zero;
            rb.AddForce(collision.contacts[0].normal * bounceForce, ForceMode2D.Impulse);
        }
    }
}