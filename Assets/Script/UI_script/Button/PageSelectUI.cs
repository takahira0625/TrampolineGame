using UnityEngine;
using UnityEngine.SceneManagement;

public class PageSelectUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    public void OnClickLogin()
    {
        SEManager.Instance.StopAll();
        SceneManager.LoadScene("StageSelectScene1_6");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickStageSelect1_6()
    {
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("StageSelectScene1-6");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickStageSelect7_12()
    {
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("StageSelectScene7-12");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickHome()
    {
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("TitleScene");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickRetry()
    {
        int stage = 1;
        if (GameManager.instance != null)
        {
            SEManager.Instance.StopAll();
            stage = Mathf.Clamp(
                (int)GameManager.instance
                      .GetType()
                      .GetMethod("GetCurrentStageNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                      .Invoke(GameManager.instance, null),
                1, 12);
            SEManager.Instance.PlayOneShot(clickSE);
            SceneBGMManager.instance.PlayStageBGM();
        }

        SceneManager.LoadScene($"Stage{stage:00}");
    }
}
