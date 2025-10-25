using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoalScaleAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 現在の scale を基準に 1.5 倍 → 戻る アニメーション
        transform.DOScale(transform.localScale * 1.2f, 0.5f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.OutQuad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
