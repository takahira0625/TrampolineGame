using System.Data.Common;
using UnityEngine;

public class BlackBlock : BaseBlock
{
    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.blackSprite);
    }

    protected override void TakeDamage(int damage)
    {
        // ‰½‚àˆ—‚µ‚È‚¢ ¨ â‘Î‚É‰ó‚ê‚È‚¢
        Debug.Log("BlackBlock‚Í‰ó‚ê‚Ü‚¹‚ñI");
    }
}
