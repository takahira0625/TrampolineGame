using UnityEngine;
using UnityEngine.SceneManagement;

public class PageSelectUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    [SerializeField] private Fade fade;

    void Start()
    {
        fade.FadeOut(0.5f);
    }
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
        SceneManager.LoadScene("StageSelectScene1_6");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickStageSelect7_12()
    {
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("StageSelectScene7_12");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickUserGuideScene()
    {
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        PlayerPrefs.SetString("FromScene", GetCurrentSceneName());
        SceneManager.LoadScene("UserGuideScene");
        SEManager.Instance.PlayOneShot(clickSE);
    }
    public void OnClickStageSelect()
    {
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene(PlayerPrefs.GetString("FromScene"));
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
        // 保存されたステージ番号を取得
        int stage = GameManager.instance.LoadLastStageNumber();

        SEManager.Instance.StopAll();
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();

        SceneManager.LoadScene($"Stage{stage:00}");
    }
    public static string GetCurrentSceneName()
    {
        // 2. このコードが現在のシーン名を取得します
        return SceneManager.GetActiveScene().name;
    }
}
