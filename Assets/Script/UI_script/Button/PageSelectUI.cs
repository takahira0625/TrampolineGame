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
        SceneManager.LoadScene("UserGuideScene");
        SEManager.Instance.PlayOneShot(clickSE);
    }
    public void OnClickUserGuideScene_JP()
    {
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("UserGuideScene_JP");
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
        // 保存されたステージ名を取得
        string stageName = GameManager.instance.LoadLastStageName();

        SEManager.Instance.StopAll();
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();

        SceneManager.LoadScene(stageName);
    }
    public static string GetCurrentSceneName()
    {
        // 2. ���̃R�[�h�����݂̃V�[�������擾���܂�
        return SceneManager.GetActiveScene().name;
    }
}
