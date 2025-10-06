using UnityEngine;

public class SpeedDoubleBlock : SpeedChangeBlock
{
    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
    }
    protected override void SetActiveState()
    {
        if (eachMaterial != null)
        {
            eachMaterial.bounciness = parameter.UpBounce;
            col.sharedMaterial = eachMaterial;
        }
        SetSprite(parameter.speedUpSprite);
    }
}
