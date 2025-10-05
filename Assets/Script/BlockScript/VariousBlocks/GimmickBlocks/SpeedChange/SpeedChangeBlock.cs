using UnityEngine;

public class SpeedChangeBlock : GimmickBlock
{
    protected PhysicsMaterial2D eachMaterial; // 参照を保持

    protected override void Awake()
    {
        base.Awake();

        // 個別 Material インスタンス生成
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

        Debug.Log($"{name}: クールタイム中 Bouncinessを{parameter.Bounce}に設定");
    }
}
