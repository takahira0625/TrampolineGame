using UnityEngine;
using UnityEngine.UI; // Legacy Text

public class ResultTimer : MonoBehaviour
{
    [SerializeField] private Text timerText;
    private float resultTime;

    void Reset()
    {
        // 自動で同じオブジェクトの Text を取得
        if (timerText == null)
        {
            timerText = GetComponent<Text>();
            if (timerText == null)
                Debug.LogWarning("[ResultTimer] timerText が未設定です。Text コンポーネントをアサインしてください。");
        }
    }

    void Start()
    {
        // PlayerPrefs に "finaltime" が存在するか確認
        if (!PlayerPrefs.HasKey("finaltimer"))
        {
            Debug.LogWarning("[ResultTimer] PlayerPrefs に 'finaltime' が存在しません。0 として表示します。");
            resultTime = 0f;
        }
        else
        {
            resultTime = PlayerPrefs.GetFloat("finaltimer", 0f);
        }

        // timerText の null チェック
        if (timerText == null)
        {
            Debug.LogError("[ResultTimer] timerText が設定されていません。Inspector でアサインしてください。");
            return;
        }

        // 表示を更新
        try
        {
            timerText.text = GameManager.FormatTime(resultTime);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ResultTimer] タイムフォーマット中にエラー: {e.Message}");
            timerText.text = "--:--.--";
        }
    }
}
