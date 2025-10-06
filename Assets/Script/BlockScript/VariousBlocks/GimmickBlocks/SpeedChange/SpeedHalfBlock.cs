using UnityEngine;

public class SpeedHalfBlock : SpeedChangeBlock
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
            eachMaterial.bounciness = parameter.DownBounce;
            col.sharedMaterial = eachMaterial;
        }
        SetSprite(parameter.speedHalfSprite);
    }
}
