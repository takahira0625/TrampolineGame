using System.Data.Common;
using UnityEngine;

public class BlackBlock : BaseBlock
{
    // SE(�N���X�S�̂ŋ��L)
    private static AudioClip hitSE;
    private static bool isSELoaded = false;

    protected override void Awake()
    {
        base.Awake();
        LoadHitSE();
        SetSprite(parameter.blackSprite);
    }

    protected override void TakeDamage(int damage)
    {
        // SE���Đ�
        if (hitSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(hitSE);
        }
    }

    /// <summary>
    /// ���ʉ������O�Ƀ��[�h(��x�̂�)
    /// </summary>
    private void LoadHitSE()
    {
        if (!isSELoaded)
        {
            hitSE = Resources.Load<AudioClip>("Audio/SE/Block/BlackBlock");
            isSELoaded = true;

            if (hitSE == null)
            {
                Debug.LogWarning("BlackBlock��SE��������܂���: Resources/Audio/SE/Block/BlackBlock");
            }
        }
    }
}
