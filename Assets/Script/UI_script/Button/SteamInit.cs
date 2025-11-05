using Steamworks;
using UnityEngine;

public class SteamInit : MonoBehaviour
{
    public static bool IsReady { get; private set; }
    public static event System.Action OnReady;

    bool inited;

    void Awake()
    {
        // 最初の試行
        TryInitSteam();
    }

    private void TryInitSteam() // 初期化処理をメソッド化
    {
        if (inited) return; // 既に初期化済みなら何もしない

        try
        {
            inited = SteamAPI.Init();
            IsReady = inited;
            if (inited)
            {
                Debug.Log($"[Steam] OK Persona={SteamFriends.GetPersonaName()} / ID={SteamUser.GetSteamID().m_SteamID}");
                OnReady?.Invoke();
            }
            else
            {
                // 初期化失敗時も、SteamAPI.IsSteamRunning()がfalseならエラーメッセージを出力
                if (!SteamAPI.IsSteamRunning())
                {
                    Debug.LogWarning("SteamAPI.Init 失敗。Steamクライアントが起動していない可能性があります。");
                }
                else
                {
                    Debug.LogError("SteamAPI.Init 失敗。Steamクライアントは起動していますが、その他の問題があります。");
                }
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("Steamworks DLL が見つからない: " + e);
        }
    }

    void Update()
    {
        if (inited)
        {
            // 既に初期化済みならコールバックを実行
            SteamAPI.RunCallbacks();
        }
        else
        {
            // 未初期化の場合、Steamクライアントが起動したかチェックし、再初期化を試みる
            if (SteamAPI.IsSteamRunning())
            {
                TryInitSteam();
            }
        }
    }

    void OnApplicationQuit() { if (inited) SteamAPI.Shutdown(); }
}