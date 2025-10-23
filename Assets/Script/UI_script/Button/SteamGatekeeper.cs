using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamGatekeeper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject steamGatePanel;
    [SerializeField] Button startButton;

    [Header("Options")]
    [SerializeField] bool autoHideWhenSteamReady = true;

    void Start()
    {
        ApplyState(SteamAPI.IsSteamRunning());
    }

    public void OnClickLoginSteam()
    {
        bool isRunning = SteamAPI.IsSteamRunning();
        ApplyState(isRunning);

        if (!isRunning)
        {
            try
            {
                // �@ Steam�A�v�����J���iSteam�N���C�A���g���C���X�g�[������Ă���΋N���j
                UnityEngine.Application.OpenURL("steam://open/main");
            }
            catch
            {
                // �A ����G���[�iSteam�����C���X�g�[���Ȃǁj�̏ꍇ��Web�u���E�U�փt�H�[���o�b�N
                UnityEngine.Application.OpenURL("https://store.steampowered.com/login/");
            }
        }
    }


    void ApplyState(bool steamReady)
    {
        // �p�l���̕\���^��\��
        if (steamGatePanel)
            steamGatePanel.SetActive(!steamReady || !autoHideWhenSteamReady);

        // �X�^�[�g�{�^���̗L���^����
        if (startButton)
            startButton.interactable = steamReady;
    }
}
