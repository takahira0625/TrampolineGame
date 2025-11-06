using UnityEngine;
using UnityEngine.SceneManagement;

public class PageSelectUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    [SerializeField] private Fade fade;

    void Start()
    {
        FindFadeCanvas();
        fade.FadeOut(0.5f);
    }

    private void FindFadeCanvas()
    {
        GameObject fadeCanvasObject = GameObject.Find("FadeCanvas");
        if (fadeCanvasObject != null)
        {
            fade = fadeCanvasObject.GetComponent<Fade>();
            if (fade == null)
            {
                Debug.LogWarning("FadeCanvas オブジェクトに Fade コンポーネントが見つかりません。");
            }
        }
        else
        {
            Debug.LogWarning("FadeCanvas オブジェクトがシーン内に見つかりません。");
        }
    }

    public void OnClickLogin()
    {
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        FindFadeCanvas();
        fade.FadeIn(0.5f, () =>
        {
            SceneManager.LoadScene("StageSelectScene1_6");
        });
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
    public void OnClickRankingHubScene()
    {
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("RankingHubScene");
        SEManager.Instance.PlayOneShot(clickSE);
    }
    public void OnClickStageSelect()
    {
        FindFadeCanvas();
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        fade.FadeIn(0.5f, () =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("FromScene"));
        });
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickHome()
    {
        FindFadeCanvas();
        SEManager.Instance.StopAll();
        SceneBGMManager.instance.PlayTitleBGM();
        fade.FadeIn(0.5f, () =>
        {
            SceneManager.LoadScene("TitleScene");
        });
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickRetry()
    {
        // 保存されたステージ名を取得
        string stageName = GameManager.instance.LoadLastStageName();
        FindFadeCanvas();
        SEManager.Instance.StopAll();
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () =>
        {
            SceneManager.LoadScene(stageName);
        });
    }
    public static string GetCurrentSceneName()
    {
        // 2. ���̃R�[�h�����݂̃V�[�������擾���܂�
        return SceneManager.GetActiveScene().name;
    }
}
