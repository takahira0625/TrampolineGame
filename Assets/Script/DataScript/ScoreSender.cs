using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ScoreSender : MonoBehaviour
{
    [Header("Supabase")]
    [SerializeField] private string supabaseUrl = "https://olkfwkewkrgpqdkrdlbe.supabase.co";
    [SerializeField] private string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im9sa2Z3a2V3a3JncHFka3JkbGJlIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjA2OTM1MjQsImV4cCI6MjA3NjI2OTUyNH0.a65YIAmFXKnUYOB02_u_Foi4p9O5t6pWWf2xGVz7MEY"; // ← anon key に差し替え

    public int StageNumber { get; set; } = 1;
    public static List<RankingEntry> LastBoard { get; private set; }

    // ランキングデータを正常に受信したときに発行されるイベント
    public static event Action<List<RankingEntry>> OnBoardDataReceived;

    // ランキングデータの取得に失敗したときに発行されるイベント
    public static event Action<string> OnBoardDataFailed;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ランキングシーン名の規則に合わせて判定
        if (!scene.name.StartsWith("RankingScene")) return;

        if (LastBoard != null)
        {
            var board = FindObjectOfType<RankingBoardLeaderLegacy>();
            if (board != null) board.RenderTop3(LastBoard);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Steamから直接送るショートカットAPI（推奨）
    // ─────────────────────────────────────────────────────────────────────────
    /// <summary>
    /// SteamからID/ユーザー名を取得して、プレイヤー名upsert→スコア送信→Top3描画まで実行
    /// </summary>
    public void SubmitCurrentSteamUser(string mode, int stage, int score)
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("[ScoreSender] Steam未初期化。'Unknown'名＆ID=0で送信します。");
            SubmitScoreAndGetBoard(0L, "Unknown", mode, stage, score);
            return;
        }

        long steamId = (long)SteamUser.GetSteamID().m_SteamID;
        string playerName = SteamFriends.GetPersonaName(); // 例: "shogo34180115"
        SubmitScoreAndGetBoard(steamId, playerName, mode, stage, score);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 既存互換API（playerName 指定あり/なし）
    // ─────────────────────────────────────────────────────────────────────────
    public void SubmitScoreAndGetBoard(long steamId, string playerName, string mode, int stage, int score)
        => StartCoroutine(SubmitFlow(steamId, playerName, mode, stage, score));

    public void SubmitScoreAndGetBoard(long steamId, string mode, int stage, int score)
        => StartCoroutine(SubmitFlow(steamId, "Unknown", mode, stage, score));

    // ─────────────────────────────────────────────────────────────────────────
    // 一連の流れ（名前保存 → スコア送信）
    // ─────────────────────────────────────────────────────────────────────────
    private IEnumerator SubmitFlow(long steamId, string playerName, string mode, int stage, int score)
    {
        yield return PostScoreAndGetBoard(steamId, playerName, mode, stage, score); // スコア送信＋Top3取得→UI反映
    }

    // ─────────────────────────────────────────────────────────────────────────
    // players.display_name を upsert（RPC: upsert_player）
    // ─────────────────────────────────────────────────────────────────────────
    [Serializable]
    private class UpsertPlayerPayload
    {
        public long in_steam_id;
        public string in_display_name;
    }

    private IEnumerator UpsertPlayer(long steamId, string displayName)
    {
        string url = $"{supabaseUrl}/rest/v1/rpc/upsert_player";

        var payload = new UpsertPlayerPayload
        {
            in_steam_id = steamId,
            in_display_name = displayName
        };
        string json = JsonUtility.ToJson(payload);
        Debug.Log($"[ScoreSender] upsert_player body = {json}");

        using (var req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("apikey", supabaseKey);
            req.SetRequestHeader("Authorization", "Bearer " + supabaseKey);

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.LogError($"[ScoreSender] upsert_player failed: {req.error}\n{req.downloadHandler.text}");
            else
                Debug.Log("[ScoreSender] upsert_player success");
        }
    }

    // RankingHubから呼ばれる「ランキング取得専用」メソッド
    public void RequestBoard(int stageNum)
    {
        // 既存のRPCを再利用し、スコア送信（score=0）と同時にボード取得を行います。
        // （DB側で score=0 が無視されるか、ランクインしない前提）
        // "RequestOnly" という名前は、DB側でデバッグするのに役立ちます。
        StartCoroutine(PostScoreAndGetBoard(0, "RequestOnly", "all_time", stageNum, 0));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // スコア送信＋Top3取得（RPC: submit_score_and_get_board）
    // ─────────────────────────────────────────────────────────────────────────
    [Serializable]
    private class RpcPayload
    {
        public string in_p_mode;
        public int in_p_stage;
        public long in_p_steam_id;
        public string in_p_display_name;
        public int in_p_score;
    }

    private IEnumerator PostScoreAndGetBoard(long steamId, string playerName, string mode, int stage, int score)
    {
        string url = $"{supabaseUrl}/rest/v1/rpc/submit_score_and_get_board";

        var payload = new RpcPayload
        {
            in_p_mode = mode,
            in_p_stage = stage,
            in_p_steam_id = steamId,
            in_p_display_name = playerName,
            in_p_score = score
        };

        string json = JsonUtility.ToJson(payload);
        Debug.Log($"[ScoreSender] RPC body = {json}");

        using (var req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("apikey", supabaseKey);
            req.SetRequestHeader("Authorization", "Bearer " + supabaseKey);

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                string errorMsg = $"[ScoreSender] RPC failed: {req.responseCode}\n{req.downloadHandler.text}";
                Debug.LogError(errorMsg);
                OnBoardDataFailed?.Invoke(errorMsg); // 失敗イベントを発行
                yield break;
            }

            string response = req.downloadHandler.text;
            Debug.Log($"[ScoreSender] RPC success: {response}");

            // JSON → Top3 → UIへ
            try
            {
                var entries = new List<RankingEntry>(JsonHelper.FromJson<RankingEntry>(response));
                LastBoard = entries;

                // 1. 既存の（古い）ランキングシーンへの通知（そのまま残す！）
                var board = FindObjectOfType<RankingBoardLeaderLegacy>();
                if (board != null)
                {
                    Debug.Log("[ScoreSender] 既存の RankingBoardLeaderLegacy を発見。RenderTop3 を呼び出します。");
                    board.RenderTop3(entries);
                }

                // 2. 新しいランキングハブ（RankingHubManager）への通知（追加する）
                Debug.Log("[ScoreSender] OnBoardDataReceived イベントを発行します。");
                OnBoardDataReceived?.Invoke(entries);
            }
            catch (Exception e)
            {
                string errorMsg = $"[ScoreSender] JSON parse error: {e}";
                Debug.LogError(errorMsg);
                OnBoardDataFailed?.Invoke(errorMsg); // 失敗イベントを発行
            }
        }
    }
}

// ─────────────────────────────────────────────────────────────────────────
// JSON配列をListに変換するためのヘルパー（UnityのJsonUtilityは配列直パース不可）
// ─────────────────────────────────────────────────────────────────────────
public static class JsonHelper
{
    [Serializable]
    private class Wrapper<T> { public T[] Items; }

    public static T[] FromJson<T>(string json)
    {
        string fixedJson = "{\"Items\":" + json + "}";
        var wrapper = JsonUtility.FromJson<Wrapper<T>>(fixedJson);
        return wrapper.Items;
    }
}
