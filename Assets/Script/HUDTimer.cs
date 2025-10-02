using UnityEngine;
using UnityEngine.UI; // Legacy Text

public class HUDTimer : MonoBehaviour
{
    [SerializeField] private Text timerText;

    void Reset()
    {
        // 自動で同じオブジェクトの Text を拾う（任意）
        if (timerText == null) timerText = GetComponent<Text>();
    }

    void Update()
    {
        if (timerText == null)
        {
            Debug.LogWarning("HUDTimer: timerText が未設定です。Text (Legacy) をアサインしてください。");
            return;
        }

        if (GameManager.instance != null)
        {
            float t = GameManager.instance.ElapsedTime;
            timerText.text = GameManager.FormatTime(t); // 例: 01:23.45
        }
        else
        {
            timerText.text = "--:--.--";
        }
    }
}
