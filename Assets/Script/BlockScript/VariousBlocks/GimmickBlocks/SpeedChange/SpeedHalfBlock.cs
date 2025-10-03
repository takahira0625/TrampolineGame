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
            Debug.Log($"{name}: Bounciness‚ğ{parameter.DownBounce}‚Éİ’è (ÀÛ‚Ì’l: {col.sharedMaterial.bounciness})");
        }
        SetSprite(parameter.speedHalfSprite);
    }
}
