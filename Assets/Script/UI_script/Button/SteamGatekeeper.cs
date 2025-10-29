using Steamworks;
using System; // Guid�p�i���͖��g�p�ł�OK�j
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Steam �܂��� �Q�X�g �ŃQ�[���J�n�𐧌䂷��N���X�B
/// �ESteam���p�� �� �����L���O���e�ȂǑS�@�\�L��
/// �E�Q�X�g���p�� �� �ꎞ�v���C�̂݁i�L�^�E�ۑ��Ȃ��j
/// </summary>
public class SteamGatekeeper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject steamGatePanel;  // ���O�C���ē��p�l��
    [SerializeField] private Button startButton;         // �u�͂��߂�v�{�^��

    [Header("Options")]
    [SerializeField] private bool autoHideWhenSteamReady = true; // Steam�܂���Guest��OK�Ȃ玩���ŉB��

    [Header("Guest Login (Optional)")]
    [SerializeField] private Button guestLoginButton;          // �Q�X�g�{�^���i�C�Ӂj
    [SerializeField] private GameObject afterLoginHideTarget;  // ���O�C����ɉB��UI�i�C�Ӂj

    public bool IsSteamReady => SteamAPI.IsSteamRunning();
    private bool guestMode = false; // ���݃Q�X�g���[�h�����ǂ���

    private bool AlreadyLogin = false;
    void Start()
    {
        // GuestLoginButton���ݒ肳��Ă���΃C�x���g�o�^
        if (guestLoginButton != null)
        {
            guestLoginButton.onClick.RemoveAllListeners();
            guestLoginButton.onClick.AddListener(OnClickGuestLogin);
        }

        // ������Ԃ̔��f
        ApplyState(SteamAPI.IsSteamRunning());
    }

    void Update()
    {
        if (IsSteamReady)
        {
            if (!AlreadyLogin)
            {
                ApplyState(SteamAPI.IsSteamRunning());
                AlreadyLogin = true;
            }
        }
    }

    /// <summary>
    /// Steam���O�C���U���{�^���̏���
    /// </summary>
    public void OnClickLoginSteam()
    {
        bool isRunning = SteamAPI.IsSteamRunning();
        ApplyState(isRunning);

        if (!isRunning)
        {
            try
            {
                // Steam�N���C�A���g���N��
                Application.OpenURL("steam://open/main");
            }
            catch
            {
                // ���s����Web���O�C���փt�H�[���o�b�N
                Application.OpenURL("https://store.steampowered.com/login/");
            }
        }
    }

    /// <summary>
    /// �Q�X�g���[�h�ő��v���C�J�n�i���O�EID�Ȃ��j
    /// </summary>
    public void OnClickGuestLogin()
    {
        guestMode = true;
        Debug.Log("[GuestLogin] �Q�X�g���[�h�ŊJ�n���܂��i���O�EID�͔��s���܂���j�B");

        // ���O�C���p�l���Ȃǂ��\��
        if (steamGatePanel != null) steamGatePanel.SetActive(false);

        if (afterLoginHideTarget != null) afterLoginHideTarget.SetActive(false);

        // �X�^�[�g�{�^����L����
        ApplyState(SteamAPI.IsSteamRunning());
    }

    /// <summary>
    /// Steam�܂��̓Q�X�g���[�h�ŊJ�n�\���𔻒肵UI���X�V
    /// </summary>
    private void ApplyState(bool steamReady)
    {
        bool canStart = steamReady || guestMode;

        // �p�l���̕\��/��\��
        if (steamGatePanel != null)
            steamGatePanel.SetActive(!canStart || !autoHideWhenSteamReady);

        // �X�^�[�g�{�^���̗L��/����
        if (startButton != null)
            startButton.interactable = canStart;
    }

    // ====== �O������Ăяo���֗����\�b�h ======

    /// <summary>
    /// �����L���O���e���\���H�iSteam���[�U�[�̂݁j
    /// </summary>
    public static bool IsRankEligible()
    {
        return SteamAPI.IsSteamRunning();
    }

    /// <summary>
    /// ���݃Q�X�g���[�h���H�i�ꎞ�Z�b�V�����j
    /// </summary>
    public bool IsGuestMode()
    {
        return guestMode;
    }
}
