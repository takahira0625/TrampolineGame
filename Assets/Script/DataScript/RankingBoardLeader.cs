using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Debug は Unity のものを使う（あいまい解消）
using Debug = UnityEngine.Debug;

public class RankingBoardLeader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform rankingContainer;     // Top3 を並べる親
    [SerializeField] private GameObject rankingEntryPrefab;  // 子に RankText / NameText / ScoreText (TMP) を持つ
    [SerializeField] private TextMeshProUGUI yourScoreText;  // 自分のスコア表示欄（任意）

    [Header("Data")]
    [SerializeField] private ScoreReceiver receiver;         // 同シーン内の ScoreReceiver（未指定なら自動検索）
    [SerializeField] private bool showSteamIdInName = false; // 名前の横に SteamID を併記する

    private void Start()
    {
        // 受信コンポーネントの確保
        if (receiver == null) receiver = FindObjectOfType<ScoreReceiver>();
        if (receiver == null)
        {
            Debug.LogError("[RankingBoardLeader] ScoreReceiver が見つかりません。");
            return;
        }

        // 自分のスコアを先に表示（GameManager が DontDestroyOnLoad 前提）
        ShowYourScore();

        // Top3 を取得→描画
        receiver.FetchTop3(RenderTop3);
    }

    /// <summary>自分のスコア（FinalTime）を yourScoreText に表示</summary>
    private void ShowYourScore()
    {
        if (yourScoreText == null) return;

        float myTime = (GameManager.instance != null) ? GameManager.instance.FinalTime : -1f;
        if (myTime >= 0f)
        {
            // 見た目を整える（MM:SS.ff）
            yourScoreText.text = $"Your Time: {GameManager.FormatTime(myTime)}";
        }
        else
        {
            yourScoreText.text = "Your Time: --:--.--";
        }
    }

    /// <summary>Top3 を描画</summary>
    private void RenderTop3(List<RankingEntry> entries)
    {
        if (rankingContainer == null)
        {
            Debug.LogError("[RankingBoardLeader] rankingContainer が未設定です。");
            return;
        }
        if (rankingEntryPrefab == null)
        {
            Debug.LogError("[RankingBoardLeader] rankingEntryPrefab が未設定です。");
            return;
        }

        // 既存行をクリア
        foreach (Transform c in rankingContainer) Destroy(c.gameObject);

        // データなし
        if (entries == null || entries.Count == 0)
        {
            var empty = Instantiate(rankingEntryPrefab, rankingContainer);
            SetText(empty.transform, "RankText", "-");
            SetText(empty.transform, "NameText", "データなし");
            SetText(empty.transform, "ScoreText", "-");
            return;
        }

        // 最大3件に制限して表示
        int display = Mathf.Min(entries.Count, 3);
        for (int i = 0; i < display; i++)
        {
            var e = entries[i];
            var go = Instantiate(rankingEntryPrefab, rankingContainer);

            SetText(go.transform, "RankText", (i + 1).ToString());

            string name = e.displayName;
            if (showSteamIdInName) name += $" ({e.steamId})";
            SetText(go.transform, "NameText", name);

            // サーバは負のミリ秒を返し、ScoreReceiver で秒に変換済み → ここでは表示整形だけ
            SetText(go.transform, "ScoreText", GameManager.FormatTime(e.timeSeconds));
        }
    }

    /// <summary>子オブジェクト childName の TMP テキストに value を入れる（安全版）</summary>
    private void SetText(Transform parent, string childName, string value)
    {
        var child = parent.Find(childName);
        if (child == null)
        {
            Debug.LogWarning($"[RankingBoardLeader] '{childName}' がプレハブ内に見つかりません。");
            return;
        }
        var tmp = child.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogWarning($"[RankingBoardLeader] '{childName}' に TextMeshProUGUI がありません。");
            return;
        }
        tmp.text = value;
    }
}
