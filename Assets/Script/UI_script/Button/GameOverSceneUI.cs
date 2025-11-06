using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class GameOverSceneUI : MonoBehaviour
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

    private void RestoreBGMVolume()
    {
        if (BGMManager.Instance != null)
        {
            float currentVolume = BGMManager.Instance.GetVolume();
            BGMManager.Instance.SetVolume(Mathf.Min(currentVolume * 2f, 1f));
        }
    }

    public void OnClickLogin()
    {
        FindFadeCanvas();
        SEManager.Instance.StopAll();
        BGMManager.Instance.Stop();
        SEManager.Instance.PlayOneShot(clickSE);
        RestoreBGMVolume();
        fade.FadeIn(0.5f, () =>
        {
            SceneManager.LoadScene("StageSelectScene1_6");
        });
    }

    public void OnClickStageSelect()
    {
        FindFadeCanvas();
        SEManager.Instance.StopAll();
        BGMManager.Instance.Stop();
        SEManager.Instance.PlayOneShot(clickSE);
        
        RestoreBGMVolume();
        // ステージセレクトシーンに遷移
        // PlayerPrefsから最後にプレイしたステージ名を取得
        string lastPlayedStage = "";
        if (GameManager.instance != null)
        {
            lastPlayedStage = GameManager.instance.LoadLastStageName();
        }
        else
        {
            Debug.LogWarning("GameManager.instance が見つかりません。PlayerPrefsから直接読み込みます。");
            lastPlayedStage = PlayerPrefs.GetString("LastStageName", ""); // GameManagerがなければ直接読み込む
        }

        if (string.IsNullOrEmpty(lastPlayedStage))
        {
            Debug.LogWarning("No last played stage found. Loading default StageSelect.");
            fade.FadeIn(0.5f, () =>
            {
                SceneManager.LoadScene("StageSelectScene1_6");
            });
            return;
        }

        // lastPlayedStageが "Stage" で始まり、その後に数字が続く形式を想定
        if (lastPlayedStage.StartsWith("Stage") && lastPlayedStage.Length >= 6) // StageXX or StageX
        {
            // Regexを使ってステージ番号をより柔軟に抽出
            var m = Regex.Match(lastPlayedStage, @"Stage\s*0?(\d{1,2})");
            if (m.Success && int.TryParse(m.Groups[1].Value, out int stageNumber))
            {
                if (stageNumber >= 1 && stageNumber <= 6)
                {
                    fade.FadeIn(0.5f, () =>
                    {
                        SceneManager.LoadScene("StageSelectScene1_6");
                    });
                    SceneBGMManager.instance.PlayTitleBGM();
                    Debug.Log($"Last played stage was {lastPlayedStage}. Loading StageSelectScene1_6.");
                    return;
                }
                else if (stageNumber >= 7 && stageNumber <= 12)
                {
                    fade.FadeIn(0.5f, () =>
                    {
                        SceneManager.LoadScene("StageSelectScene7_12");
                    });
                    SceneBGMManager.instance.PlayTitleBGM();
                    Debug.Log($"Last played stage was {lastPlayedStage}. Loading StageSelectScene7_12.");
                    return;
                }
                else
                {
                    Debug.LogWarning($"Stage number {stageNumber} out of expected range (1-12). Loading default StageSelect.");
                    
                    fade.FadeIn(0.5f, () => { SceneManager.LoadScene("StageSelectScene1_6"); });
                    SceneBGMManager.instance.PlayTitleBGM();
                    return;
                }
            }
        }
        // 上記のステージ名パターンに一致しない場合のデフォルトの挙動
        Debug.LogWarning($"Last played stage '{lastPlayedStage}' did not match expected pattern. Loading default StageSelect.");
        fade.FadeIn(0.5f, () => { SceneManager.LoadScene("StageSelectScene1_6"); }); // どの条件にも当てはまらない場合のデフォルト
    }

    public void OnClickHome()
    {
        FindFadeCanvas();
        SEManager.Instance.StopAll();
        BGMManager.Instance.Stop();
        
        SEManager.Instance.PlayOneShot(clickSE);
        RestoreBGMVolume();
        fade.FadeIn(0.5f, () => { SceneManager.LoadScene("TitleScene"); });
        SceneBGMManager.instance.PlayTitleBGM();
    }

    public void OnClickRetry()
    {
        FindFadeCanvas();
        // 保存されたステージ名を取得
        string stageName = GameManager.instance.LoadLastStageName();

        SEManager.Instance.StopAll();
        SEManager.Instance.PlayOneShot(clickSE);
        RestoreBGMVolume();
        fade.FadeIn(0.5f, () =>
        {
            SceneManager.LoadScene(stageName);
        });
        SceneBGMManager.instance.PlayStageBGM();
    }
}
