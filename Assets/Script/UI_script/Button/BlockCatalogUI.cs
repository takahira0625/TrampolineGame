using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockCatalogUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    [SerializeField] private Fade fade;

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
        SceneManager.LoadScene("NormalBlockScene");
    }

    public void OnClickKeyGoalBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        SceneManager.LoadScene("KeyGoalBlockScene");
    }

    public void OnClickSpeedUpBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        SceneManager.LoadScene("SpeedUpBlockScene");
    }

    public void OnClickSpeedDownBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        SceneManager.LoadScene("SpeedDownBlockScene");
    }

    public void OnClickSpeedReqBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        SceneManager.LoadScene("SpeedReqBlockScene");
    }

    public void OnClickBombBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        SceneManager.LoadScene("BombBlockScene");
    }

    public void OnClickDoubleBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        SceneManager.LoadScene("DoubleBlockScene");
    }

    public void OnClickWarpBlock()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        SceneManager.LoadScene("WarpBlockScene");
    }

    public void BackToBlockCatalog()
    {
        BGMManager.Instance.Stop();
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayTitleBGM();
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "NormalBlockScene" | currentSceneName == "KeyGoalBlockScene")
        {
            SceneManager.LoadScene("BlockCatalogScene01");
        }
        else
        {
            SceneManager.LoadScene("BlockCatalogScene02");
        }
    }
}
