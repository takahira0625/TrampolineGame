using UnityEngine;
using UnityEngine.Events;
using Steamworks;                // Steamworks.NET
using TMPro;

public class SteamLoginManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject loginPanel;  // ���O�C��UI�i�������ɉB���j

    [Header("Events")]
    public UnityEvent<string /*personaName*/, string /*steamId64*/> OnSteamLoggedIn;

    bool inited;
    HAuthTicket authTicket = HAuthTicket.Invalid;
    byte[] ticketBuffer = new byte[2048];

    void Awake()
    {
        // ���ɂǂ����ŏ��������Ă���Ȃ�ȗ���
        try
        {
            inited = SteamAPI.Init();
            if (!inited) Debug.LogError("SteamAPI.Init ���s�Bsteam_appid.txt �� Steam �N���C�A���g���m�F�B");
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("Steamworks DLL ��������Ȃ�: " + e);
        }
    }

    void Update() { if (inited) SteamAPI.RunCallbacks(); }
    void OnApplicationQuit() { if (inited) SteamAPI.Shutdown(); }

    // --- �{�^������Ă� ---
    // �C���� SteamLoginManager.cs�i�����j

    // Awake() ���� Init �͍폜�i�܂��̓R�����g�A�E�g�j
    // void Awake() { /* ������ Init ���Ȃ� */ }

    public void LoginWithSteam()
    {
        // Init �͊��� SteamInit.cs �����s���Ă���O��
        if (!SteamAPI.IsSteamRunning())
        {
            Debug.LogError("Steam �N���C�A���g���N��/���O�C�����Ă��܂���B");
            return;
        }

        string persona = SteamFriends.GetPersonaName();
        string steamId = SteamUser.GetSteamID().m_SteamID.ToString();

        uint size;
        SteamNetworkingIdentity identity = new SteamNetworkingIdentity();
        identity.SetSteamID(SteamUser.GetSteamID());
        HAuthTicket authTicket = SteamUser.GetAuthSessionTicket(ticketBuffer, ticketBuffer.Length, out size, ref identity);

        var ticketBase64 = System.Convert.ToBase64String(ticketBuffer, 0, (int)size);
        OnSteamLoggedIn?.Invoke(persona, steamId);
        if (loginPanel) loginPanel.SetActive(false);
    }

    public class SteamNameDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;

        void Start()
        {
            if (SteamAPI.Init())
            {
                string persona = SteamFriends.GetPersonaName();
                nameText.text = $"�悤�����A{persona} ����I";
                Debug.Log($"[Steam] ���O�C�����[�U�[: {persona}");
            }
            else
            {
                nameText.text = "Steam���O�C�����m�F�ł��܂���";
                Debug.LogWarning("SteamAPI.Init() ���s - Steam���N�����Ă��Ȃ���������܂���");
            }
        }

        void OnApplicationQuit()
        {
            SteamAPI.Shutdown();
        }
    }


    // --- �Q�l�F�T�[�o���؁iEdge Function�Ȃǁj ---
    // IEnumerator VerifyOnServer(string ticketB64, string persona, string steamId) { ... }
}
