using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour
{
    // ボタンの OnClick() から呼び出してシーン遷移するだけ
    public void OnClickStage1()
    {
        // 名前入力なし → 単純に遷移
        SceneManager.LoadScene("SampleScene 2");
    }
}
