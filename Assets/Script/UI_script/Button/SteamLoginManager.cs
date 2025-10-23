using UnityEngine;
using UnityEngine.Events;
using Steamworks;                // Steamworks.NET
using TMPro;

public class SteamLoginManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject loginPanel;  // ログインUI（成功時に隠す）

    [Header("Events")]
    public UnityEvent<string /*personaName*/, string /*steamId64*/> OnSteamLoggedIn;

    bool inited;
    HAuthTicket authTicket = HAuthTicket.Invalid;
    byte[] ticketBuffer = new byte[2048];

    void Awake()
    {
        // 既にどこかで初期化しているなら省略可
        try
        {
            inited = SteamAPI.Init();
            if (!inited) Debug.LogError("SteamAPI.Init 失敗。steam_appid.txt と Steam クライアントを確認。");
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("Steamworks DLL が見つからない: " + e);
        }
    }

    void Update() { if (inited) SteamAPI.RunCallbacks(); }
    void OnApplicationQuit() { if (inited) SteamAPI.Shutdown(); }

    // --- ボタンから呼ぶ ---
    // 修正版 SteamLoginManager.cs（抜粋）

    // Awake() 内の Init は削除（またはコメントアウト）
    // void Awake() { /* ここで Init しない */ }

    public void LoginWithSteam()
    {
        // Init は既に SteamInit.cs が実行している前提
        if (!SteamAPI.IsSteamRunning())
        {
            Debug.LogError("Steam クライアントが起動/ログインしていません。");
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
                nameText.text = $"ようこそ、{persona} さん！";
                Debug.Log($"[Steam] ログインユーザー: {persona}");
            }
            else
            {
                nameText.text = "Steamログインが確認できません";
                Debug.LogWarning("SteamAPI.Init() 失敗 - Steamが起動していないかもしれません");
            }
        }

        void OnApplicationQuit()
        {
            SteamAPI.Shutdown();
        }
    }


    // --- 参考：サーバ検証（Edge Functionなど） ---
    // IEnumerator VerifyOnServer(string ticketB64, string persona, string steamId) { ... }
}
