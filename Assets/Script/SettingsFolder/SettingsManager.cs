using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Toggle clickSwapToggle;
    private const string CLICK_KEY = "UseLeftClick"; // 保存用キー名

    private void Awake()
    {
        // 保存された設定を読み込む
        int savedValue = PlayerPrefs.GetInt(CLICK_KEY, 1);
        InputManager.LeftClickSlow = (savedValue == 1);

        // トグルの見た目を反映
        clickSwapToggle.isOn = InputManager.LeftClickSlow;

        Debug.Log("Click setting loaded: " + (InputManager.LeftClickSlow ? "Left" : "Right"));
    }

    private void Start()
    {
        // Listener を登録
        clickSwapToggle.onValueChanged.AddListener(OnClickSwapChanged);
    }

    private void OnClickSwapChanged(bool isSwapped)
    {
        InputManager.LeftClickSlow = isSwapped;

        // 保存処理
        PlayerPrefs.SetInt(CLICK_KEY, InputManager.LeftClickSlow ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Click setting changed and saved: " + (InputManager.LeftClickSlow ? "Left" : "Right"));
    }
}
