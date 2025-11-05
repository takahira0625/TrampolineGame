using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private AudioClip clickSE;
    public void ToggleSettings()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
