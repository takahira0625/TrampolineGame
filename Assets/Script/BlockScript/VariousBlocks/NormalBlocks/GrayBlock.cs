using System.Data.Common;
using UnityEngine;

public class GrayBlock : BaseBlock
{
    [SerializeField] private int extraHealth = 1;

    protected override void Awake()
    {
        base.Awake();
        health += extraHealth;

        SetSprite(parameter.graySprite);
    }
}
