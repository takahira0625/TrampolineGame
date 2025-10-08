using UnityEngine;

[CreateAssetMenu(fileName = "ParameterConfig", menuName = "Configs/ParameterConfig")]
public class ParameterConfig : ScriptableObject
{
    [Header("��b�ݒ�")]
    public int Health = 3;

    [Header("�T�C�Y�ݒ�")]
    public float Width = 1f;
    public float Height = 1f;

    [Header("�N�[���^�C���ݒ�")]
    public float cooldownTime = 3.0f;

    [Header("�o�E���X�ݒ�")]
    public float Bounce = 1.0f;
    public float UpBounce = 2.0f;
    public float DownBounce = 0.5f;

    [Header("�e�N�X�`���ݒ�")]
    public Sprite baseSprite;
    public Sprite graySprite;
    public Sprite blackSprite;
    public Sprite speedHalfSprite;
    public Sprite speedUpSprite;
    public Sprite cooldownSprite;
    public Sprite KeySprite;
    public Sprite GoalSprite;
    public Sprite BombSprite;
    public Sprite WarpSprite;
    public Sprite DoubleSprite;
}
