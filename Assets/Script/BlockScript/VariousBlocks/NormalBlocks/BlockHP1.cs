using System.Data.Common;
using UnityEngine;

public class BlockHP1 : BaseBlock
{
    [SerializeField] private int extraHealth = -2;

    protected override void Awake()
    {
        base.Awake();
        health += extraHealth;

        SetSprite(parameter.hp1Sprite);
    }
}
