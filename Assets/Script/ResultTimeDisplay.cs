using UnityEngine;
using UnityEngine.UI; // ← Text はこっち

public class ResultTimeDisplay : MonoBehaviour
{
    [SerializeField] private Text timeText;

    void Start()
    {
        if (timeText == null)
        {
            Debug.LogError("ResultTimeDisplay: timeText が未設定です。TextMeshProUGUI をアサインしてください。");
            return;
        }

        if (GameManager.instance != null)
        {
            float t = GameManager.instance.FinalTime;
            timeText.text = "TIME  " + GameManager.FormatTime(t);
        }
        else
        {
            timeText.text = "TIME  --:--.--";
        }
    }
}
