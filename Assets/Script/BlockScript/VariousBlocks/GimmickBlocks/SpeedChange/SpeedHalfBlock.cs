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
            Debug.Log($"{name}: Bounciness��{parameter.DownBounce}�ɐݒ� (���ۂ̒l: {col.sharedMaterial.bounciness})");
        }
        SetSprite(parameter.speedHalfSprite);
    }
}
