using UnityEngine;
using UnityEngine.SceneManagement;

public class PageSelectUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    public void OnClickLogin()
    {
        SceneManager.LoadScene("StageSelectScene1_6");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickStageSelect()
    {
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("StageSelectScene1-6");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickHome()
    {
        SceneBGMManager.instance.PlayTitleBGM();
        SceneManager.LoadScene("TitleScene");
        SEManager.Instance.PlayOneShot(clickSE);
    }

    public void OnClickRetry()
    {
        // GameManager�̌��݃X�e�[�W�擾���g���O��
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
            SceneBGMManager.instance.PlayStageBGM();
        }

        SceneManager.LoadScene($"Stage{stage:00}");
    }
}
