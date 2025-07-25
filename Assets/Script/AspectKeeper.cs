using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectKeeper : MonoBehaviour
{

    [SerializeField]
    private Camera targetCamera; //対象とするカメラ

    [SerializeField]
    private Vector2 aspectVec; //目的解像度
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var screenAspect = Screen.width / (float)Screen.height; //画面のアスペクト比
        var targetAspect = aspectVec.x / aspectVec.y; //目的のアスペクト比

        var magRate = targetAspect / screenAspect; //目的アスペクト比にするための倍率

        var viewportRect = new Rect(0, 0, 1, 1); //Viewport初期値でRectを作成
        if (magRate < 1)
        {
            viewportRect.width = magRate; //使用する横幅を変更
            viewportRect.width = magRate; //使用する横幅を変更
        }
        else
        {
            viewportRect.height = 1 / magRate; //使用する縦幅を変更
            viewportRect.height = 1 / magRate; //使用する縦幅を変更
        }
        targetCamera.rect = viewportRect; //カメラのViewportに適用
    }
}
