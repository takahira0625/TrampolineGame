using UnityEngine;
using UnityEngine.UI; // �� Text �͂�����

public class ResultTimeDisplay : MonoBehaviour
{
    [SerializeField] private Text timeText;

    void Start()
    {
        if (timeText == null)
        {
            Debug.LogError("ResultTimeDisplay: timeText �����ݒ�ł��BTextMeshProUGUI ���A�T�C�����Ă��������B");
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
