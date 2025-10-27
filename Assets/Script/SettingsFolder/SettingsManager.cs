using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Toggle clickSwapToggle;
    private const string CLICK_KEY = "UseLeftClick"; // �ۑ��p�L�[��

    private void Awake()
    {
        // �ۑ����ꂽ�ݒ��ǂݍ���
        int savedValue = PlayerPrefs.GetInt(CLICK_KEY, 1);
        InputManager.LeftClickSlow = (savedValue == 1);

        // �g�O���̌����ڂ𔽉f
        clickSwapToggle.isOn = InputManager.LeftClickSlow;

        Debug.Log("Click setting loaded: " + (InputManager.LeftClickSlow ? "Left" : "Right"));
    }

    private void Start()
    {
        // Listener ��o�^
        clickSwapToggle.onValueChanged.AddListener(OnClickSwapChanged);
    }

    private void OnClickSwapChanged(bool isSwapped)
    {
        InputManager.LeftClickSlow = isSwapped;

        // �ۑ�����
        PlayerPrefs.SetInt(CLICK_KEY, InputManager.LeftClickSlow ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Click setting changed and saved: " + (InputManager.LeftClickSlow ? "Left" : "Right"));
    }
}
