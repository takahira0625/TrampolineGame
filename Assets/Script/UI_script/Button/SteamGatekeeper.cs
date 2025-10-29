using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SteamGatekeeper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject steamGatePanel;
    [SerializeField] private Button startButton;
    [SerializeField] private SteamNameDisplay nameDisplay;

    [Header("Options")]
    [SerializeField] private bool autoHideWhenSteamReady = true;

    [Header("Guest Login (Optional)")]
    [SerializeField] private Button guestLoginButton;
    [SerializeField] private GameObject afterLoginHideTarget;

    private bool guestMode = false;
    private bool personaReady = false;   // ��NameDisplay����̒ʒm��true

    void Start()
    {
        if (guestLoginButton != null)
        {
            guestLoginButton.onClick.RemoveAllListeners();
            guestLoginButton.onClick.AddListener(OnClickGuestLogin);
        }
        ApplyState();

        StartCoroutine(AutoCloseWhenAlreadyLoggedIn());
    }

    public void OnClickLoginSteam()
    {
        guestMode = false;

        bool running = SteamAPI.IsSteamRunning();
        bool logged = false;

        // Steamworks �������ς݂̂Ƃ����� BLoggedOn ���Q��
        if (SteamInit.IsReady && running)
            logged = SteamUser.BLoggedOn();

        // �� ���N�� �܂��� �����O�C�� �̏ꍇ�� Steam ��O�ʂɏo��
        if (!running || !logged)
        {
            try
            {
                UnityEngine.Application.OpenURL("steam://open/main"); // �N���C�A���g��O�ʉ�
            }
            catch
            {
                // �ň��u���E�U�̃��O�C���ł�
                UnityEngine.Application.OpenURL("https://store.steampowered.com/login/");
            }
        }

        // �� ������ Display ���̍ă`�F�b�N�𑦃X�^�[�g�i�ҋ@�R���[�`��&�R�[���o�b�N������j
        nameDisplay?.TryRefreshAndNotify();

        UnityEngine.Debug.Log("[SteamGatekeeper] Login clicked; waiting persona name...");
    }



    public void OnClickGuestLogin()
    {
        guestMode = true;
        Debug.Log("[SteamGatekeeper] Guest mode");
        ApplyState();
    }

    /// <summary>SteamNameDisplay ����Ă΂��B���[�U�[�����\���ł��������������B</summary>
    public void NotifySteamPersonaReady()
    {
        personaReady = true;
        Debug.Log("[SteamGatekeeper] Persona ready �� close gate");
        ApplyState();
    }

    private void ApplyState()
    {
        bool canStart = personaReady || guestMode;

        if (startButton) startButton.interactable = canStart;
        if (steamGatePanel) steamGatePanel.SetActive(!(canStart && autoHideWhenSteamReady));
        if (afterLoginHideTarget) afterLoginHideTarget.SetActive(!canStart);

        UnityEngine.Debug.Log($"[Gatekeeper] ApplyState: personaReady={personaReady}, guestMode={guestMode}, canStart={canStart}");
    }


    // SteamGatekeeper.cs �̒��ɒǉ�
    private IEnumerator AutoCloseWhenAlreadyLoggedIn()
    {
        // SteamInit �����������܂őҋ@
        while (!SteamInit.IsReady) yield return null;

        // �ő� 3 �b�قǗl�q������i0.25s �~ 12�j
        for (int i = 0; i < 12 && !personaReady; i++)
        {
            // �N���C�A���g�N�� & ���O�C���ς݂�
            if (SteamAPI.IsSteamRunning() && SteamUser.BLoggedOn())
            {
                string persona = SteamFriends.GetPersonaName();
                if (!string.IsNullOrEmpty(persona) && persona != "[unknown]")
                {
                    UnityEngine.Debug.Log($"[Gatekeeper] auto-close: persona={persona}");
                    personaReady = true;
                    ApplyState(); // �������Ŋm���Ƀp�l�������
                    yield break;
                }
                else
                {
                    // �܂���Ȃ���v�����o���ď����҂�
                    SteamFriends.RequestUserInformation(SteamUser.GetSteamID(), true);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
        UnityEngine.Debug.Log("[Gatekeeper] auto-close: not ready yet");
    }

}
