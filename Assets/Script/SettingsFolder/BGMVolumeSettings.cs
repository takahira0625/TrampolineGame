using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text volumeText; // Slider�̎qText�����蓖�Ă�

    private const string VOLUME_KEY = "BGMVolume";

    private void Awake()
    {
        // �ۑ����ꂽ���ʂ�ǂݍ��ށi�f�t�H���g0.5�j
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.5f);

        // Slider�ɔ��f
        volumeSlider.value = savedVolume;

        // BGMManager�̉��ʂɔ��f
        BGMManager.Instance.SetVolume(savedVolume);

        // Text�\�����X�V
        UpdateVolumeText(savedVolume);
    }

    private void Start()
    {
        // Slider�̒l���ς�����Ƃ��̏�����o�^
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        // BGMManager�ɉ��ʂ𔽉f
        BGMManager.Instance.SetVolume(value);

        // PlayerPrefs�ɕۑ�
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();

        // Text�\�����X�V
        UpdateVolumeText(value);

        Debug.Log("BGM volume changed and saved: " + value);
    }

    // Text�\���X�V�p�֐�
    private void UpdateVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100);
        volumeText.text = percent + "%";
    }
}
