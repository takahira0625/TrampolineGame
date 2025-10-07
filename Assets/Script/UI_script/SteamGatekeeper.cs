using UnityEngine;
using UnityEngine.UI;          // Button
using Steamworks;              // SteamAPI.IsSteamRunning()

public class SteamGatekeeper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject steamGatePanel;  // Steam�ɑ����p�l���i�����O�C���������\���j
    [SerializeField] Button startButton;      // �u�X�^�[�g�v�{�^��

    [Header("Options")]
    [SerializeField] bool autoHideWhenSteamReady = true; // �N������SteamOK�Ȃ玩���ŕ���

    void Start()
    {
        ApplyState(SteamAPI.IsSteamRunning());
    }

    // �uSteam�Ń��O�C���v�{�^������Ăԑz��i�C�Ӂj
    public void OnClickLoginSteam() => ApplyState(SteamAPI.IsSteamRunning());

    // �u�Q�X�g�Ŏn�߂�v�{�^���͕ʓr�V�[���J�ڂ֌q����OK

    void ApplyState(bool steamReady)
    {
        // Panel�̕\��/��\��
        if (steamGatePanel)
            steamGatePanel.SetActive(!steamReady || !autoHideWhenSteamReady);

        // Start�{�^���̖�����/�L����
        if (startButton)
            startButton.interactable = steamReady;
    }
}
