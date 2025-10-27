using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockCatalogUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    [SerializeField] private Fade fade;

    public void OnClickCatalog01()
    {
        SceneManager.LoadScene("BlockCatalogScene01");
    }

    public void OnClickCatalog02()
    {
        SceneManager.LoadScene("BlockCatalogScene02");
    }

    public void OnClickStageSelect()
    {
        SceneManager.LoadScene("StageSelectScene1_6");
    }

    public void OnClickNormalBlock()
    {
        SceneManager.LoadScene("NormalBlockScene");
    }

    public void OnClickKeyGoalBlock()
    {
        SceneManager.LoadScene("KeyGoalBlockScene");
    }

    public void OnClickSpeedUpBlock()
    {
        SceneManager.LoadScene("SpeedUpBlockScene");
    }

    public void OnClickSpeedDownBlock()
    {
        SceneManager.LoadScene("SpeedDownBlockScene");
    }

    public void OnClickSpeedReqBlock()
    {
        SceneManager.LoadScene("SpeedReqBlockScene");
    }

    public void OnClickBombBlock()
    {
        SceneManager.LoadScene("BombBlockScene");
    }

    public void OnClickDoubleBlock()
    {
        SceneManager.LoadScene("DoubleBlockScene");
    }

    public void OnClickWarpBlock()
    {
        SceneManager.LoadScene("WarpBlockScene");
    }

    public void BackToBlockCatalog()
    {
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
