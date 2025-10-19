using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class SteamGatekeeper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject steamGatePanel;
    [SerializeField] Button startButton;

    [Header("Options")]
    [SerializeField] bool autoHideWhenSteamReady = true;

    void Start()
    {
        ApplyState(SteamAPI.IsSteamRunning());Å@//Ç±Ç±Ç≈bool steamReadyÇÃílÇï‘Ç∑
    }

    void ApplyState(bool steamReady)
    {
        if (steamGatePanel)
            steamGatePanel.SetActive(!steamReady || !autoHideWhenSteamReady);

        if (startButton)
            startButton.interactable = steamReady;
    }
}