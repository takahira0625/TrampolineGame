using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class ScoreSender : MonoBehaviour
{
    [Header("Backend")]
    [SerializeField] private string backendBaseUrl = "http://127.0.0.1:8000";
    [SerializeField] private string mode = "all_time";                       

    [Header("Stage")]
    [SerializeField, Range(1, 12)] private int stageNumber = 1;              
    public int StageNumber                                                
    {
        get => stageNumber;
        set => stageNumber = Mathf.Clamp(value, 1, 12);
    }

    [Header("Steam")]
    [Tooltip("Steam未初期化時のデバッグ用。0 の場合は送信しない。")]
    [SerializeField] private long debugSteamIdOverride = 0;

    
    public void SendClearTimeSeconds(float clearTimeSeconds)
    {
        StartCoroutine(PostScore(clearTimeSeconds));
    }

    // --- 内部実装 ---

    private long GetSteamId()
    {
        // ★ Steamが未初期化でも送信できるよう、debugSteamIdOverrideを優先
        if (debugSteamIdOverride != 0)
            return debugSteamIdOverride;

        // Steam連携なしでテストするため常に固定IDを返す
        return 76561198000000000; // ←仮のSteamID
    }

    private IEnumerator PostScore(float timeSec)
    {
        // 入力チェック
        if (string.IsNullOrEmpty(backendBaseUrl))
        {
            Debug.LogError("[ScoreSender] backendBaseUrl が未設定です。");
            yield break;
        }
        var baseUrl = backendBaseUrl.TrimEnd('/');

        var steamId = GetSteamId();
        if (steamId == 0)
        {
            Debug.LogError("[ScoreSender] SteamID が取得できません（Steam未初期化 or debugSteamIdOverride未設定）");
            yield break;
        }

        // ★短いタイムが良い → サーバは「負のミリ秒」をスコアとして受け取る想定
        int negativeMillis = -(int)Mathf.Round(timeSec * 1000f);

        var payload = $"{{\"steam_id\":{steamId},\"score\":{negativeMillis},\"mode\":\"{mode}\",\"stage\":{stageNumber}}}";
        var url = $"{baseUrl}/score"; // FastAPI 側の /score エンドポイント

        using (var req = new UnityWebRequest(url, "POST"))
        {
            var body = Encoding.UTF8.GetBytes(payload);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[ScoreSender] POST failed: {req.responseCode} {req.error} {req.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"[ScoreSender] POST ok: time={timeSec:F2}s → score={negativeMillis} (mode={mode}, stage={stageNumber})");
            }
        }
    }
}
