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
            Debug.Log($"{name}: Bounciness��{parameter.UpBounce}�ɐݒ� (���ۂ̒l: {col.sharedMaterial.bounciness})");
        }
        SetSprite(parameter.speedUpSprite);
    }
}
