using UnityEngine;
using UnityEngine.SceneManagement;

public class PageSelectUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    public void OnClickLogin()
    {
        SceneManager.LoadScene("StageSelectScene");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickStageSelect()
    {
        SceneManager.LoadScene("StageSelectScene");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickHome()
    {
        SceneManager.LoadScene("TitleScene");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickRetry()
    {
        // GameManagerの現在ステージ取得を使う前提
        int stage = 1;
        if (GameManager.instance != null)
        {
            stage = Mathf.Clamp(
                (int)GameManager.instance
                      .GetType()
                      .GetMethod("GetCurrentStageNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                      .Invoke(GameManager.instance, null),
                1, 12);
            SEManager.Instance.PlayOneShot(clickSE);
        }

        SceneManager.LoadScene($"stage{stage:00}");
    }
}
