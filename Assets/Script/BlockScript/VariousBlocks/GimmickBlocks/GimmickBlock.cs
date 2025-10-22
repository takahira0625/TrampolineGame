using UnityEngine;
//GmmickBlock:�N�[���^�C�������M�~�b�N�u���b�N�̊��N���X
//1.�N�[���^�C�����͖��G 2.�N�[���^�C�����͐F���Â����� 3.�N�[���^�C�����̓o�E���X���P�ɂ���
public abstract class GimmickBlock : BaseBlock
{
    //�R���|�[�l���g�̎擾
    [SerializeField] protected CoolTimeScript cooldown;
    protected Collider2D col;
    protected Renderer rend;

    protected override void Awake()
    {
        base.Awake();
        //�@�����_���[�̎擾
        rend = GetComponent<Renderer>();
        //�AcooltimeComponent�̎擾
        if (cooldown == null)
        {
            cooldown = GetComponent<CoolTimeScript>();
            if (cooldown == null)
            {
                cooldown = gameObject.AddComponent<CoolTimeScript>();
            }
        }
        cooldown.OnCooldownChanged += HandleCooldownChanged;
        //�B�R���C�_�[�̎擾
        col = GetComponent<Collider2D>();
    }

    private void HandleCooldownChanged(bool isOnCooldown)
    {
        if (isOnCooldown)
        {
            //SetSprite(parameter.blackSprite);//���̃R�[�h��OK
            OnCooldownStart();
        }
        else
        {
            SetActiveState();
        }
    }
    protected virtual void OnPlayerTouch(GameObject player)
    {
        // ���ʂ́u�v���C���[�ڐG���v�����i�N�[���^�C���Ȃǁj
        //if (cooldown != null && !cooldown.IsOnCooldown)
        //{
        //    cooldown.StartCooldown(parameter.cooldownTime);
        //}
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerTouch(collision.gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerTouch(other.gameObject);
        }
    }
    protected override void TakeDamage(int damage)
    {
        if (cooldown != null && cooldown.IsOnCooldown) return; // �N�[���^�C�����͖��G
        base.TakeDamage(damage);  // �ʏ폈��

        if (health > 0) // 'health' �� BaseBlock ����p��
        {
            if (cooldown != null)
            {
                // ���ꂪ OnCooldownStart() ���g���K�[����
                cooldown.StartCooldown(parameter.cooldownTime);
            }
        }
    }

    /// <summary>
    /// BaseBlock��HP�A���X�v���C�g�ύX���u�������v����
    /// </summary>
    protected override void UpdateBlockAppearance()
    {
        // GimmickBlock �� SetActiveState / OnCooldownStart ��
        // �Ǝ��ɃX�v���C�g���Ǘ����邽�߁ABaseBlock �̏����͌Ă΂Ȃ��B
        // (��������ɂ��邱�ƂŁATakeDamage ���ɃX�v���C�g������ɕς��̂�h��)
    }

    protected virtual void OnCooldownStart() { }
    protected virtual void SetActiveState() { }

}
