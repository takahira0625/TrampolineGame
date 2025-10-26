using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoalScaleAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ���݂� scale ����� 1.5 �{ �� �߂� �A�j���[�V����
        transform.DOScale(transform.localScale * 1.2f, 0.5f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.OutQuad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
