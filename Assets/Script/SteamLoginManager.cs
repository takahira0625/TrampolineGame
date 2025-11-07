using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SteamLoginManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loginPanel;

    [Header("Events")]
    public UnityEvent<string /*personaName*/, string /*steamId64*/> OnSteamLoggedIn;

    // ── グローバルに参照できる状態（ScoreSender などから使う） ──
    public static bool Initialized { get; private set; }
    public static ulong SteamId64 { get; private set; }
    public static string PersonaName { get; private set; } = "";

    // シングルトン（多重初期化防止）
    public static SteamLoginManager Instance { get; private set; }

    // 認証チケット（必要に応じて使う）
    private HAuthTicket authTicket = HAuthTicket.Invalid;
    private readonly byte[] ticketBuffer = new byte[2048];

    private void Awake()
    {
        // Singleton 確保
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // すでに初期化済みならスキップ
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
                Debug.LogError("[SteamLoginManager] SteamAPI.Init 失敗。steam_appid.txt と Steam クライアントを確認してください。");
            }
        }
        catch (System.DllNotFoundException e)
        {
            Initialized = false;
            Debug.LogError("[SteamLoginManager] Steamworks DLL が見つかりません: " + e);
        }
        catch (System.Exception e)
        {
            Initialized = false;
            Debug.LogError("[SteamLoginManager] 例外: " + e);
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
    /// ログインボタンから呼ぶ想定。最新の Persona/ID を同期し、必要なら認証チケットを発行。
    /// </summary>
    public void LoginWithSteam()
    {
        // ... (中略：認証チケット発行までの既存ロジック) ...

        // UI / Event
        OnSteamLoggedIn?.Invoke(PersonaName, SteamId64.ToString());
        if (loginPanel) loginPanel.SetActive(false);

        // ★★★ 追記するロジック ★★★
        // ログイン成功後、StageSelectScene1_6に遷移する
        SceneManager.LoadScene("StageSelectScene1_6");
        // ★★★ 追記するロジック ★★★

        Debug.Log($"[SteamLoginManager] Logged in as {PersonaName} ({SteamId64}), ticket={authTicket.m_HAuthTicket}");
    }

    // ─────────────────────────────────────────────────────────────────
    // 既存のネストクラスを修正：ここで SteamAPI.Init を再実行しないこと！
    // マネージャの状態を読むだけにする。
    // ─────────────────────────────────────────────────────────────────
    public class SteamNameDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;

        private void Start()
        {
            if (nameText == null) return;

            if (SteamLoginManager.Initialized)
            {
                nameText.text = $"Welcome, {SteamLoginManager.PersonaName}!";
                Debug.Log($"[Steam] ログインユーザー: {SteamLoginManager.PersonaName}");
            }
            else
            {
                nameText.text = "Steamログインが確認できません";
                Debug.LogWarning("[SteamNameDisplay] Steam 未初期化です。");
            }

            // ログイン完了時に即反映したい場合、イベント購読
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
/// 互換ラッパー：既存コードが参照する SteamManager.* を提供
/// （ScoreSender などが SteamManager.Initialized を使っていても動く）
/// </summary>
public static class SteamManager
{
    public static bool Initialized => SteamLoginManager.Initialized;
    public static ulong SteamId64 => SteamLoginManager.SteamId64;
    public static string PersonaName => SteamLoginManager.PersonaName;
}
