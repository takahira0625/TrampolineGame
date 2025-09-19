using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    [SerializeField] protected int health = 3;              // �u���b�N�̗̑�
    [SerializeField] protected BreakBlock breakBlock;       // ���o�S���̃X�N���v�g

    protected virtual void Awake()
    {
        // ������ BreakBlock ���擾
        if (breakBlock == null)
            breakBlock = GetComponent<BreakBlock>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �̗͂����炷
            TakeDamage(1);
        }
    }
    protected virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            // �j�󏈗���BreakBlock�ɒʒm
            if (breakBlock != null)
                breakBlock.OnBreak();
        }
    }
}
