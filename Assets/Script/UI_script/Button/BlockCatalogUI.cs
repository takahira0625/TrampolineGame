using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockCatalogUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    [SerializeField] private Fade fade;

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
    void Start()
    {
        FindFadeCanvas();
        fade.FadeOut(0.5f);
    }
    public void OnClickCatalog01()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("BlockCatalogScene01");
    }

    public void OnClickCatalog02()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("BlockCatalogScene02");
    }

    public void OnClickCatalog03()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("BlockCatalogScene03");
    }

    public void OnClickStageSelect()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("StageSelectScene1_6");
    }

    public void OnClickNormalBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () =>
        {
            SceneManager.LoadScene("NormalBlockScene");
        });
    }

    public void OnClickKeyGoalBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () =>
        {
            SceneManager.LoadScene("KeyGoalBlockScene");
        });
    }

    public void OnClickSpeedUpBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () =>
        {
            SceneManager.LoadScene("SpeedUpBlockScene");
        });
    }

    public void OnClickSpeedDownBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () => { SceneManager.LoadScene("SpeedDownBlockScene"); });
    }

    public void OnClickSpeedReqBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () => { SceneManager.LoadScene("SpeedReqBlockScene"); });
    }

    public void OnClickBombBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () => { SceneManager.LoadScene("BombBlockScene"); });
    }

    public void OnClickDoubleBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () => { SceneManager.LoadScene("DoubleBlockScene"); });
    }

    public void OnClickWarpBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () => { SceneManager.LoadScene("WarpBlockScene"); });
    }

    public void OnClickKingBombBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        fade.FadeIn(0.5f, () => { SceneManager.LoadScene("KingBombBlockScene"); });
    }

    public void BackToBlockCatalog()
    {
        BGMManager.Instance.Stop();
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayTitleBGM();
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "NormalBlockScene" | currentSceneName == "KeyGoalBlockScene")
        {
            fade.FadeIn(0.5f, () => { SceneManager.LoadScene("BlockCatalogScene01"); });
        }
        else if (currentSceneName == "KingBombBlockScene")
        {
            fade.FadeIn(0.5f, () => { SceneManager.LoadScene("BlockCatalogScene03"); });
        }
        else
        {
            fade.FadeIn(0.5f, () => { SceneManager.LoadScene("BlockCatalogScene02"); });
        }
    }

    public void OnClickStage00()
    {
        //開発用
        SceneManager.LoadScene("Stage00");
    }
}
