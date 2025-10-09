using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    [SerializeField] protected ParameterConfig parameter;
    [SerializeField] protected BreakBlock breakBlock; // ���o�S���̃X�N���v�g

    protected int health;

    protected virtual void Awake()
    {
        Physics.bounceThreshold = 0.0f;
        //�@�R���|�[�l���g��ǉ��iBreakBlock.cs,ParameterConfig.cs�j

        // -------------------------------
        // BreakBlock �������擾�E�ǉ�
        // -------------------------------
        if (breakBlock == null)
        {
            breakBlock = GetComponent<BreakBlock>();
            if (breakBlock == null)
            {
                breakBlock = gameObject.AddComponent<BreakBlock>();
            }
        }
        // -------------------------------
        // ParameterConfig �������擾
        // -------------------------------
        if (parameter == null)
        {
            // Resources �t�H���_�� ParameterConfig ��u���z��
            parameter = Resources.Load<ParameterConfig>("ParameterConfig");
        }
        // health �ݒ�
        if (parameter != null)
            health = parameter.Health;

        SetSprite(parameter.baseSprite);
    }

    // �X�v���C�g��ύX
    protected void SetSprite(Sprite sprite)
    {
        if (sprite == null || parameter == null) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // �X�v���C�g��ݒ�
        sr.sprite = sprite;

        // Sliced���[�h�ɐݒ�
        sr.drawMode = SpriteDrawMode.Sliced;

        // �T�C�Y��K�p
        sr.size = new Vector2(parameter.Width, parameter.Height);
        Debug.Log(sr.size);

        // BoxCollider2D�̃T�C�Y�����킹��
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(parameter.Width, parameter.Height);
        }
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
