using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text volumeText; // Sliderの子Textを割り当てる

    private const string VOLUME_KEY = "BGMVolume";

    private void Awake()
    {
        // 保存された音量を読み込む（デフォルト0.5）
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.5f);

        // Sliderに反映
        volumeSlider.value = savedVolume;

        // BGMManagerの音量に反映
        BGMManager.Instance.SetVolume(savedVolume);

        // Text表示も更新
        UpdateVolumeText(savedVolume);
    }

    private void Start()
    {
        // Sliderの値が変わったときの処理を登録
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        // BGMManagerに音量を反映
        BGMManager.Instance.SetVolume(value);

        // PlayerPrefsに保存
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();

        // Text表示を更新
        UpdateVolumeText(value);

        Debug.Log("BGM volume changed and saved: " + value);
    }

    // Text表示更新用関数
    private void UpdateVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100);
        volumeText.text = percent + "%";
    }
}
