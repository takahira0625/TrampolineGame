using System.Data.Common;
using UnityEngine;

public class BlackBlock : BaseBlock
{
    // SE(クラス全体で共有)
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
        // SEを再生
        if (hitSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(hitSE);
        }
    }

    /// <summary>
    /// 効果音を事前にロード(一度のみ)
    /// </summary>
    private void LoadHitSE()
    {
        if (!isSELoaded)
        {
            hitSE = Resources.Load<AudioClip>("Audio/SE/Block/BlackBlock");
            isSELoaded = true;

            if (hitSE == null)
            {
                Debug.LogWarning("BlackBlockのSEが見つかりません: Resources/Audio/SE/Block/BlackBlock");
            }
        }
    }
}
