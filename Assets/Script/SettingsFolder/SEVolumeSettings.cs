using UnityEngine;
using UnityEngine.UI;

public class SEVolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text volumeText; // Slider�̎qText�����蓖�Ă�

    private const string VOLUME_KEY = "SEVolume";

    private void Awake()
    {
        // PlayerPrefs����ۑ����ꂽ���ʂ�ǂݍ��ށi�f�t�H���g0.5�j
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.5f);

        // Slider�ɔ��f
        volumeSlider.value = savedVolume;

        // SEManager�̉��ʂɔ��f
        SEManager.Instance.SetVolume(savedVolume);

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
        // SEManager�ɉ��ʂ𔽉f
        SEManager.Instance.SetVolume(value);

        // PlayerPrefs�ɕۑ�
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();

        // Text�\�����X�V
        UpdateVolumeText(value);

        Debug.Log("SE volume changed and saved: " + value);
    }

    // Text�\���X�V�p�֐�
    private void UpdateVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100);
        volumeText.text = percent + "%";
    }
}
