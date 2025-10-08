using UnityEngine;

public class Double : GimmickBlock
{
    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
    }
    protected override void SetActiveState()
    {
        SetSprite(parameter.DoubleSprite);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
