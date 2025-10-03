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
        // �����������Ȃ� �� ��΂ɉ��Ȃ�
        Debug.Log("BlackBlock�͉��܂���I");
    }
}
