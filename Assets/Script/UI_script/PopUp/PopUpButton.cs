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
        string currentSceneName = SceneManager.GetActiveScene().name;
        // シーン名が "Stage" で始まり、その後に数字が続く形式を想定
        if (currentSceneName.StartsWith("Stage") && currentSceneName.Length == 7) // "StageXX" の形式をチェック
        {
            // "Stage" の後の数字部分を抽出
            if (int.TryParse(currentSceneName.Substring(5, 2), out int stageNumber)) // "Stage" の5文字目から2桁の数字をパース
            {
                if (stageNumber >= 1 && stageNumber <= 6)
                {
                    SceneManager.LoadScene("StageSelectScene1_6");
                    Debug.Log($"Current scene {currentSceneName} is a Stage 1-6. Loading StageSelectScene1_6.");
                    return; // 処理が完了したのでここで関数を終了
                }
                else if (stageNumber >= 7 && stageNumber <= 12)
                {
                    SceneManager.LoadScene("StageSelectScene7_12");
                    Debug.Log($"Current scene {currentSceneName} is a Stage 7-12. Loading StageSelectScene7_12.");
                    return; // 処理が完了したのでここで関数を終了
                }
            }
        }
    }
}
