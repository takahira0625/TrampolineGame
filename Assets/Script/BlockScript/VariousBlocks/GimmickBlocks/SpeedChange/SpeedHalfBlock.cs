using UnityEngine;

public class SpeedHalfBlock : SpeedChangeBlock
{
    [Header("HPごとの安定スプライト")]
    [Tooltip("HP=3 の時のスプライト (SpeedDown)")]
    [SerializeField] private Sprite activeSpriteHP3; // HP3の見た目
    [Tooltip("HP=2 の時のスプライト (SpeedDownHP2)")]
    [SerializeField] private Sprite activeSpriteHP2; // HP2の見た目
    [Tooltip("HP=1 の時のスプライト (SpeedDownHP1)")]
    [SerializeField] private Sprite activeSpriteHP1; // HP1の見た目

    [Header("クールタイム中のスプライト")]
    [Tooltip("HP3 -> 2 の時のクールスプライト (SpeedDownHP2_cool)")]
    [SerializeField] private Sprite coolTimeSpriteHP2; // HP3->2になった時のクール中
    [Tooltip("HP2 -> 1 の時のクールスプライト (SpeedDownHP1_cool)")]
    [SerializeField] private Sprite coolTimeSpriteHP1; // HP2->1になった時のクール中

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
    // GimmickBlock または SpeedChangeBlock の OnCooldownStart を上書き
    protected override void OnCooldownStart()
    {
        //    (物理特性(bounciness)を変更する)
        base.OnCooldownStart();

        if (health == 2) // HPが3から2になった
        {
            SetSprite(coolTimeSpriteHP2);
        }
        else if (health == 1) // HPが2から1になった
        {
            SetSprite(coolTimeSpriteHP1);
        }
    }
}
