using UnityEngine;

public class SpeedHalfBlock : SpeedChangeBlock
{
    [Header("HP���Ƃ̈���X�v���C�g")]
    [Tooltip("HP=3 �̎��̃X�v���C�g (SpeedDown)")]
    [SerializeField] private Sprite activeSpriteHP3; // HP3�̌�����
    [Tooltip("HP=2 �̎��̃X�v���C�g (SpeedDownHP2)")]
    [SerializeField] private Sprite activeSpriteHP2; // HP2�̌�����
    [Tooltip("HP=1 �̎��̃X�v���C�g (SpeedDownHP1)")]
    [SerializeField] private Sprite activeSpriteHP1; // HP1�̌�����

    [Header("�N�[���^�C�����̃X�v���C�g")]
    [Tooltip("HP3 -> 2 �̎��̃N�[���X�v���C�g (SpeedDownHP2_cool)")]
    [SerializeField] private Sprite coolTimeSpriteHP2; // HP3->2�ɂȂ������̃N�[����
    [Tooltip("HP2 -> 1 �̎��̃N�[���X�v���C�g (SpeedDownHP1_cool)")]
    [SerializeField] private Sprite coolTimeSpriteHP1; // HP2->1�ɂȂ������̃N�[����

    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
    }
    protected override void SetActiveState()
    {
        if (eachMaterial != null)
        {
            eachMaterial.bounciness = parameter.DownBounce;
            col.sharedMaterial = eachMaterial;
        }

        if (health == 3)
        {
            SetSprite(activeSpriteHP3);
        }
        else if (health == 2)
        {
            SetSprite(activeSpriteHP2);
        }
        else if (health == 1)
        {
            SetSprite(activeSpriteHP1);
        }
    }
    // GimmickBlock �܂��� SpeedChangeBlock �� OnCooldownStart ���㏑��
    protected override void OnCooldownStart()
    {
        //    (��������(bounciness)��ύX����)
        base.OnCooldownStart();

        if (health == 2) // HP��3����2�ɂȂ���
        {
            SetSprite(coolTimeSpriteHP2);
        }
        else if (health == 1) // HP��2����1�ɂȂ���
        {
            SetSprite(coolTimeSpriteHP1);
        }
    }
}
