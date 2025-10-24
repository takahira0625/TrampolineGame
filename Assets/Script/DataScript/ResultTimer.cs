using UnityEngine;
using UnityEngine.UI; // Legacy Text

public class ResultTimer : MonoBehaviour
{
    [SerializeField] private Text timerText;
    private float resultTime;

    void Reset()
    {
        // �����œ����I�u�W�F�N�g�� Text ���擾
        if (timerText == null)
        {
            timerText = GetComponent<Text>();
            if (timerText == null)
                Debug.LogWarning("[ResultTimer] timerText �����ݒ�ł��BText �R���|�[�l���g���A�T�C�����Ă��������B");
        }
    }

    void Start()
    {
        // PlayerPrefs �� "finaltime" �����݂��邩�m�F
        if (!PlayerPrefs.HasKey("finaltimer"))
        {
            Debug.LogWarning("[ResultTimer] PlayerPrefs �� 'finaltime' �����݂��܂���B0 �Ƃ��ĕ\�����܂��B");
            resultTime = 0f;
        }
        else
        {
            resultTime = PlayerPrefs.GetFloat("finaltimer", 0f);
        }

        // timerText �� null �`�F�b�N
        if (timerText == null)
        {
            Debug.LogError("[ResultTimer] timerText ���ݒ肳��Ă��܂���BInspector �ŃA�T�C�����Ă��������B");
            return;
        }

        // �\�����X�V
        try
        {
            timerText.text = GameManager.FormatTime(resultTime);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ResultTimer] �^�C���t�H�[�}�b�g���ɃG���[: {e.Message}");
            timerText.text = "--:--.--";
        }
    }
}
