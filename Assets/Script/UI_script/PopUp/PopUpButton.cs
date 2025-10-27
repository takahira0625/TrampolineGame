using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopUpButton : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 現在のシーンを再読み込みする
    /// </summary>
    public void ReloadCurrentScene()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        BGMManager.Instance.Stop();
        SceneBGMManager.instance.PlayStageBGM();
        // Time.timeScaleが0の場合は元に戻す
        Time.timeScale = 1f;

        // 現在のシーン名を取得して再読み込み
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    /// <summary>
    /// StageSelectScene1_6に遷移する
    /// </summary>
    public void LoadStageSelectScene()
    {
        // Time.timeScaleが0の場合は元に戻す
        Time.timeScale = 1f;

        // SEを再生
        if (clickSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(clickSE);
        }

        // BGMを変更
        if (SceneBGMManager.instance != null)
        {
            BGMManager.Instance.Stop();
            SceneBGMManager.instance.PlayTitleBGM();
        }

        // ステージセレクトシーンに遷移
        SceneManager.LoadScene("StageSelectScene1_6");
    }
}
