using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    public void OnClickLogin()
    {
        SEManager.Instance.StopAll();
        BGMManager.Instance.Stop();
        SceneManager.LoadScene("StageSelectScene1_6");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickStageSelect()
    {
        SEManager.Instance.StopAll();
        BGMManager.Instance.Stop();
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("StageSelectScene1_6");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickHome()
    {
        SEManager.Instance.StopAll();
        BGMManager.Instance.Stop();
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
}
