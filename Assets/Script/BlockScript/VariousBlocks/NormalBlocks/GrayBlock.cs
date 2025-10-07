using System.Data.Common;
using UnityEngine;

public class Block : BaseBlock
{
    [SerializeField] private int extraHealth = 3;

    protected override void Awake()
    {
        base.Awake();
        health += extraHealth;

        SetSprite(parameter.graySprite);
    }
}
