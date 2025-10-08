using UnityEngine;

[CreateAssetMenu(fileName = "ParameterConfig", menuName = "Configs/ParameterConfig")]
public class ParameterConfig : ScriptableObject
{
    [Header("基礎設定")]
    public int Health = 3;

    [Header("サイズ設定")]
    public float Width = 1f;
    public float Height = 1f;

    [Header("クールタイム設定")]
    public float cooldownTime = 3.0f;

    [Header("バウンス設定")]
    public float Bounce = 1.0f;
    public float UpBounce = 2.0f;
    public float DownBounce = 0.5f;

    [Header("テクスチャ設定")]
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
