using UnityEngine;
using UnityEngine.SceneManagement;

public class PageSelectUI : MonoBehaviour
{
    public void OnClickLogin() => SceneManager.LoadScene("StageSelectScene");
    public void OnClickStageSelect() => SceneManager.LoadScene("StageSelectScene");
    public void OnClickHome() => SceneManager.LoadScene("TitleScene");
    public void OnClickRetry()
    {
        // GameManager�̌��݃X�e�[�W�擾���g���O��
        int stage = 1;
        if (GameManager.instance != null)
            stage = Mathf.Clamp(
                (int)GameManager.instance
                      .GetType()
                      .GetMethod("GetCurrentStageNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                      .Invoke(GameManager.instance, null),
                1, 12);

        SceneManager.LoadScene($"stage{stage:00}");
    }
}
