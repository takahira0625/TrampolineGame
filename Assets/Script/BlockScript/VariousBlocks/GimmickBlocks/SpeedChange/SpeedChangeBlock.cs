using UnityEngine;

public class SpeedChangeBlock : GimmickBlock
{
    protected PhysicsMaterial2D eachMaterial; // �Q�Ƃ�ێ�

    protected override void Awake()
    {
        base.Awake();

        // �� Material �C���X�^���X����
        if (col != null && col.sharedMaterial != null)
        {
            eachMaterial = new PhysicsMaterial2D();
            eachMaterial.bounciness = col.sharedMaterial.bounciness;
            eachMaterial.friction = col.sharedMaterial.friction;
            col.sharedMaterial = eachMaterial;
        }
    }

    protected override void OnCooldownStart()
    {
        if (eachMaterial != null)
            eachMaterial.bounciness = parameter.Bounce;
            col.sharedMaterial = eachMaterial;

        Debug.Log($"{name}: �N�[���^�C���� Bounciness��{parameter.Bounce}�ɐݒ�");
    }
}
