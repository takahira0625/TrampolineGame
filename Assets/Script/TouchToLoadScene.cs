using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchToLoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // マウスの左クリック、または画面がタッチされた瞬間に実行
        if (Input.GetMouseButtonDown(0))
        {
            // "SelectScene"という名前のシーンを読み込む
            SceneManager.LoadScene("SelectScene");
        }
    }
}
