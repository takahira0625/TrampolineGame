using UnityEngine;

//GimmickBlock:�N�[���^�C�������M�~�b�N�u���b�N�̊��N���X
//1.�N�[���^�C�����͖��G 2.�N�[���^�C�����͐F���Â����� 3.�N�[���^�C�����̓o�E���X���P�ɂ���
public abstract class GimmickBlock : BaseBlock
{
    // ... �iAwake, HandleCooldownChanged �܂ł͕ύX�Ȃ��j ...
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

    // OnPlayerTouch �͎q�N���X�Ŏ�������̂Œ��g�͋�̂܂�
    protected virtual void OnPlayerTouch(GameObject player)
    {
    }

    // ������ �C���_ 1 ������
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �N�[���^�C�����łȂ����m�F
            if (cooldown != null && !cooldown.IsOnCooldown)
            {
                // �v���C���[���X���[���łȂ����m�F
                PlayerController pc = collision.gameObject.GetComponent<PlayerController>();

                // pc �����݂��A���X���[���łȂ� (!pc.isActive) �ꍇ�̂� OnPlayerTouch ���Ă�
                if (pc != null && !pc.isActive)
                {
                    OnPlayerTouch(collision.gameObject);
                }
            }
        }

        // base �̏����iTakeDamage�j�� OnPlayerTouch �̃`�F�b�N��ɍs��
        base.OnCollisionEnter2D(collision);
    }
    // ������ �C���_ 1 �I�� ������


    // ������ �C���_ 2 ������
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �N�[���^�C�����łȂ����m�F
            if (cooldown != null && !cooldown.IsOnCooldown)
            {
                // �v���C���[���X���[���łȂ����m�F
                PlayerController pc = other.GetComponent<PlayerController>();

                // pc �����݂��A���X���[���łȂ� (!pc.isActive) �ꍇ�̂� OnPlayerTouch ���Ă�
                if (pc != null && !pc.isActive)
                {
                    OnPlayerTouch(other.gameObject);
                }
            }
        }
    }
    // ������ �C���_ 2 �I�� ������

    // ... �iTakeDamage, UpdateBlockAppearance �Ȃǂ̎c��̃��\�b�h�͕ύX�Ȃ��j ...
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