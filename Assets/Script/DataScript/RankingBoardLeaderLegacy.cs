using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ← Legacy Text 用
using static System.Net.Mime.MediaTypeNames;
using Debug = UnityEngine.Debug;
using UIText = UnityEngine.UI.Text;  // ← 追加：TextはUnityのものを使う

public class RankingBoardLeaderLegacy : MonoBehaviour
{
    [Header("Scene Texts (Legacy UI.Text)")]
    public UIText stageText;     // 例: "RANKING: STAGE1"
    public UIText firstText;     // 1位表示用（"user 00:12.34"）
    public UIText secondText;    // 2位
    public UIText thirdText;     // 3位
    public UIText yourScoreText; // "MY SCORE: 00:34.56"

    [Header("Data")]
    public ScoreReceiver receiver;           // 同シーンの ScoreReceiver
    public bool showSteamIdInName = false;   // 名前横にSteamID併記するか

    private void Start()
    {
        if (receiver == null) receiver = FindObjectOfType<ScoreReceiver>();
        if (receiver == null)
        {
            Debug.LogError("[RankingBoardLeaderLegacy] ScoreReceiver が見つかりません。");
            return;
        }

        // タイトル（STAGE番号）
        if (stageText != null)
        {
            stageText.text = $"RANKING: STAGE{receiver.StageNumber}";
        }

        // 自分のスコア
        if (yourScoreText != null)
        {
            float myTime = (GameManager.instance != null) ? GameManager.instance.FinalTime : -1f;
            yourScoreText.text = (myTime >= 0f)
                ? $"MY SCORE: {GameManager.FormatTime(myTime)}"
                : "MY SCORE: --:--.--";
        }

        // Top3取得 → 描画
        receiver.FetchTop3(RenderTop3);
    }

    private void RenderTop3(List<RankingEntry> entries)
    {
        // 初期化（データ無し表示）
        SetSafe(firstText, "—");
        SetSafe(secondText, "—");
        SetSafe(thirdText, "—");

        if (entries == null || entries.Count == 0) return;

        // 1位
        if (entries.Count >= 1) SetSafe(firstText, ComposeLine(entries[0]));
        // 2位
        if (entries.Count >= 2) SetSafe(secondText, ComposeLine(entries[1]));
        // 3位
        if (entries.Count >= 3) SetSafe(thirdText, ComposeLine(entries[2]));
    }

    private string ComposeLine(RankingEntry e)
    {
        // timeSeconds は秒（ScoreReceiver で負のms→秒に変換済）
        string name = e.displayName;
        if (showSteamIdInName) name += $" ({e.steamId})";
        return $"{name}   {GameManager.FormatTime(e.timeSeconds)}";
    }

    private void SetSafe(UIText t, string value)
    {
        if (t != null) t.text = value;
    }
}
