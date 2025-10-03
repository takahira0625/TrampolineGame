using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    [SerializeField] protected ParameterConfig parameter;
    [SerializeField] protected BreakBlock breakBlock; // ���o�S���̃X�N���v�g

    protected int health;
    protected virtual void Awake()
    {
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

        // �����ڂ�ݒ�
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            sr.drawMode = SpriteDrawMode.Sliced; // Sliced or Tiled
            // ���� Sprite �T�C�Y���擾���Ă��̂܂ܐݒ�
            sr.size = sr.sprite.bounds.size;
        }
        SetSprite(parameter.baseSprite);
    }
    //�}�e���A����ύX
    protected void SetSprite(Sprite sprite)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sprite != null)
        {
            sr.sprite = sprite;
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
