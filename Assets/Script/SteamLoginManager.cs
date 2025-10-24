using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SteamLoginManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loginPanel;

    [Header("Events")]
    public UnityEvent<string /*personaName*/, string /*steamId64*/> OnSteamLoggedIn;

    // ���� �O���[�o���ɎQ�Ƃł����ԁiScoreSender �Ȃǂ���g���j ����
    public static bool Initialized { get; private set; }
    public static ulong SteamId64 { get; private set; }
    public static string PersonaName { get; private set; } = "";

    // �V���O���g���i���d�������h�~�j
    public static SteamLoginManager Instance { get; private set; }

    // �F�؃`�P�b�g�i�K�v�ɉ����Ďg���j
    private HAuthTicket authTicket = HAuthTicket.Invalid;
    private readonly byte[] ticketBuffer = new byte[2048];

    private void Awake()
    {
        // Singleton �m��
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ���łɏ������ς݂Ȃ�X�L�b�v
        if (Initialized) return;

        try
        {
            if (SteamAPI.Init())
            {
                Initialized = true;
                SteamId64 = SteamUser.GetSteamID().m_SteamID;
                PersonaName = SteamFriends.GetPersonaName();
                Debug.Log($"[SteamLoginManager] Steam initialized. User: {PersonaName} ({SteamId64})");
            }
            else
            {
                Initialized = false;
                Debug.LogError("[SteamLoginManager] SteamAPI.Init ���s�Bsteam_appid.txt �� Steam �N���C�A���g���m�F���Ă��������B");
            }
        }
        catch (System.DllNotFoundException e)
        {
            Initialized = false;
            Debug.LogError("[SteamLoginManager] Steamworks DLL ��������܂���: " + e);
        }
        catch (System.Exception e)
        {
            Initialized = false;
            Debug.LogError("[SteamLoginManager] ��O: " + e);
        }
    }

    private void Update()
    {
        if (Initialized) SteamAPI.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        if (Initialized)
        {
            SteamAPI.Shutdown();
            Initialized = false;
            Debug.Log("[SteamLoginManager] Steam API shutdown.");
        }
    }

    /// <summary>
    /// ���O�C���{�^������Ăԑz��B�ŐV�� Persona/ID �𓯊����A�K�v�Ȃ�F�؃`�P�b�g�𔭍s�B
    /// </summary>
    public void LoginWithSteam()
    {
        if (!Initialized)
        {
            Debug.LogError("[SteamLoginManager] Steam ���������ł��BSteam �N���C�A���g���N�����Ă��炨�������������B");
            return;
        }

        if (!SteamAPI.IsSteamRunning())
        {
            Debug.LogError("[SteamLoginManager] Steam �N���C�A���g���N��/���O�C�����Ă��܂���B");
            return;
        }

        // �ŐV���𓯊�
        SteamId64 = SteamUser.GetSteamID().m_SteamID;
        PersonaName = SteamFriends.GetPersonaName();

        // �F�؃`�P�b�g�i�K�v�ȏꍇ�̂݊��p�j
        uint size;
        var identity = new SteamNetworkingIdentity();
        identity.SetSteamID(SteamUser.GetSteamID());
        authTicket = SteamUser.GetAuthSessionTicket(ticketBuffer, ticketBuffer.Length, out size, ref identity);
        var ticketBase64 = System.Convert.ToBase64String(ticketBuffer, 0, (int)size);
        // �� �T�[�o���؂��K�v�Ȃ� ticketBase64 �𑗂��Ă�������

        // UI / Event
        OnSteamLoggedIn?.Invoke(PersonaName, SteamId64.ToString());
        if (loginPanel) loginPanel.SetActive(false);

        Debug.Log($"[SteamLoginManager] Logged in as {PersonaName} ({SteamId64}), ticket={authTicket.m_HAuthTicket}");
    }

    // ����������������������������������������������������������������������������������������������������������������������������������
    // �����̃l�X�g�N���X���C���F������ SteamAPI.Init ���Ď��s���Ȃ����ƁI
    // �}�l�[�W���̏�Ԃ�ǂނ����ɂ���B
    // ����������������������������������������������������������������������������������������������������������������������������������
    public class SteamNameDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;

        private void Start()
        {
            if (nameText == null) return;

            if (SteamLoginManager.Initialized)
            {
                nameText.text = $"Welcome, {SteamLoginManager.PersonaName}!";
                Debug.Log($"[Steam] ���O�C�����[�U�[: {SteamLoginManager.PersonaName}");
            }
            else
            {
                nameText.text = "Steam���O�C�����m�F�ł��܂���";
                Debug.LogWarning("[SteamNameDisplay] Steam ���������ł��B");
            }

            // ���O�C���������ɑ����f�������ꍇ�A�C�x���g�w��
            if (SteamLoginManager.Instance != null)
            {
                SteamLoginManager.Instance.OnSteamLoggedIn.AddListener(OnLoggedIn);
            }
        }

        private void OnDestroy()
        {
            if (SteamLoginManager.Instance != null)
            {
                SteamLoginManager.Instance.OnSteamLoggedIn.RemoveListener(OnLoggedIn);
            }
        }

        private void OnLoggedIn(string persona, string steamId64)
        {
            if (nameText != null) nameText.text = $"Welcome, {persona}!";
        }
    }
}

/// <summary>
/// �݊����b�p�[�F�����R�[�h���Q�Ƃ��� SteamManager.* ���
/// �iScoreSender �Ȃǂ� SteamManager.Initialized ���g���Ă��Ă������j
/// </summary>
public static class SteamManager
{
    public static bool Initialized => SteamLoginManager.Initialized;
    public static ulong SteamId64 => SteamLoginManager.SteamId64;
    public static string PersonaName => SteamLoginManager.PersonaName;
}
