using UnityEngine;
using UnityEngine.UI; // Legacy Text

public class HUDTimer : MonoBehaviour
{
    [SerializeField] private Text timerText;

    void Reset()
    {
        // �����œ����I�u�W�F�N�g�� Text ���E���i�C�Ӂj
        if (timerText == null) timerText = GetComponent<Text>();
    }

    void Update()
    {
        if (timerText == null)
        {
            Debug.LogWarning("HUDTimer: timerText �����ݒ�ł��BText (Legacy) ���A�T�C�����Ă��������B");
            return;
        }

        if (GameManager.instance != null)
        {
            float t = GameManager.instance.ElapsedTime;
            timerText.text = GameManager.FormatTime(t); // ��: 01:23.45
        }
        else
        {
            timerText.text = "--:--.--";
        }
    }
}
