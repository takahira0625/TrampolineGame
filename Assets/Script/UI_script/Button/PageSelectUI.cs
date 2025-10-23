using UnityEngine;
using UnityEngine.SceneManagement;

public class PageSelectUI : MonoBehaviour
{
    public void OnClickLogin() => SceneManager.LoadScene("StageSelectScene1_6");
    public void OnClickStageSelect1_6() => SceneManager.LoadScene("StageSelectScene1_6");
    public void OnClickStageSelect7_12() => SceneManager.LoadScene("StageSelectScene7_12");
    public void OnClickHome() => SceneManager.LoadScene("TitleScene");
    
    public void OnClickRetry()
    {
        // GameManagerの現在ステージ取得を使う前提
        int stage = 1;
        if (GameManager.instance != null)
            stage = Mathf.Clamp(
                (int)GameManager.instance
                      .GetType()
                      .GetMethod("GetCurrentStageNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                      .Invoke(GameManager.instance, null),
                1, 12);

        SceneManager.LoadScene($"stage{stage:00}");
    }
}
