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
            Debug.Log($"{name}: Bounciness‚ğ{parameter.UpBounce}‚Éİ’è (ÀÛ‚Ì’l: {col.sharedMaterial.bounciness})");
        }
        SetSprite(parameter.speedUpSprite);
    }
}
