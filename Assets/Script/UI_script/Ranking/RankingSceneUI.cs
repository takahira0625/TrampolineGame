using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class RankingSceneUI : MonoBehaviour
{
    [Header("UI パネル (切り替え対象)")]
    [Tooltip("オンラインランキングの時に表示するUI要素の親オブジェクト")]
    [SerializeField] private GameObject onlineRankingPanel;

    [Tooltip("個人ランキングの時に表示するUI要素の親オブジェクト")]
    [SerializeField] private GameObject personalRankingPanel;


    [Header("切り替えボタン")]
    [Tooltip("個人ランキング表示に切り替えるボタン")]
    [SerializeField] private Button showPersonalButton;

    [Tooltip("オンラインランキング表示に戻すボタン")]
    [SerializeField] private Button showOnlineButton;


    [Header("個人ランキング表示用テキスト")]
    [Tooltip("個人ランキング1位のタイム表示用Text")]
    [SerializeField] private Text personalRank1_Time;

    [Tooltip("個人ランキング2位のタイム表示用Text")]
    [SerializeField] private Text personalRank2_Time;

    [Tooltip("個人ランキング3位のタイム表示用Text")]
    [SerializeField] private Text personalRank3_Time;

    private string personalSaveFilePath;
    private PersonalRankings currentPersonalRankings;

    private string currentStageId;

    void Start()
    {
        // --- 1. ボタンに関数を登録 ---
        if (showPersonalButton != null)
            showPersonalButton.onClick.AddListener(ShowPersonalRanking);
        if (showOnlineButton != null)
            showOnlineButton.onClick.AddListener(ShowOnlineRanking);

        // --- 2. 表示するステージのIDを特定 ---
        int stageNum = PlayerPrefs.GetInt("LastClearedStageNumber", 1);
        currentStageId = $"Stage-{stageNum}";
        Debug.Log($"[RankingSceneUI.Start] PlayerPrefs.GetInt(\"LastClearedStageNumber\") で読み込んだ生の値: {stageNum}");

        // --- 3. JSONファイルから個人ランキングを直接読み込む ---
        InitializePersonalRanking();
        LoadPersonalRankings();
        LoadPersonalRankingData();

        // --- 4. 初期表示 ---
        ShowOnlineRanking();
        Debug.Log($"[RankingSceneUI] PlayerPrefsから読み込んだステージID: '{currentStageId}' のランキングを探します。");
    }

    // 個人ランキングのタイムを読み込み、Textに設定する
    void LoadPersonalRankingData()
    {
        // このクラスが読み込んだデータを直接使う
        List<ScoreEntry> scores = GetPersonalRankingsForStage(currentStageId);

        if (personalRank1_Time != null)
            personalRank1_Time.text = "1ST  " + ((scores.Count > 0) ? GameManager.FormatTime(scores[0].time) : "--:--.--");

        if (personalRank2_Time != null)
            personalRank2_Time.text = "2ND  " + ((scores.Count > 1) ? GameManager.FormatTime(scores[1].time) : "--:--.--");

        if (personalRank3_Time != null)
            personalRank3_Time.text = "3RD  " + ((scores.Count > 2) ? GameManager.FormatTime(scores[2].time) : "--:--.--");
    }

    // 「個人ランキング」ボタンが押された時の処理
    void ShowPersonalRanking()
    {
        if (onlineRankingPanel != null) onlineRankingPanel.SetActive(false);
        if (personalRankingPanel != null) personalRankingPanel.SetActive(true);
    }

    // 「オンラインに戻る」ボタンが押された時の処理
    void ShowOnlineRanking()
    {
        if (onlineRankingPanel != null) onlineRankingPanel.SetActive(true);
        if (personalRankingPanel != null) personalRankingPanel.SetActive(false);
    }

    // 個人ランキングのファイルパスを決定する
    private void InitializePersonalRanking()
    {
        // GameManager.cs と同じパスを参照
        personalSaveFilePath = Path.Combine(Application.persistentDataPath, "personal_rankings.json");
    }

    // ファイルから個人ランキングを読み込む
    private void LoadPersonalRankings()
    {
        if (!File.Exists(personalSaveFilePath))
        {
            currentPersonalRankings = new PersonalRankings();
            Debug.Log("[RankingSceneUI] ランキングファイルがまだ作成されていません。");
            return;
        }
        try
        {
            string json = File.ReadAllText(personalSaveFilePath);
            currentPersonalRankings = JsonUtility.FromJson<PersonalRankings>(json);
            if (currentPersonalRankings == null)
            {
                currentPersonalRankings = new PersonalRankings();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[RankingSceneUI] 個人ランキングの読み込み失敗: {e.Message}");
            currentPersonalRankings = new PersonalRankings();
        }
    }

    /// 指定したステージの個人ランキングリスト（上位3件）を取得する
    public List<ScoreEntry> GetPersonalRankingsForStage(string stageId)
    {
        if (currentPersonalRankings == null)
        {
            Debug.LogWarning("[RankingSceneUI] データがまだロードされていません。");
            return new List<ScoreEntry>();
        }

        Debug.Log($"[RankingSceneUI] JSONデータ内のステージ件数: {currentPersonalRankings.allStageRankingsList.Count} 件");
        StageRanking stageRank = currentPersonalRankings.allStageRankingsList
            .FirstOrDefault(r => r.stageId == stageId);

        if (stageRank != null)
        {
            Debug.Log($"[RankingSceneUI] '{stageId}' のデータを発見しました。記録件数: {stageRank.scores.Count} 件");
            return stageRank.scores;
        }

        Debug.LogWarning($"[RankingSceneUI] '{stageId}' のデータが見つかりませんでした。");
        return new List<ScoreEntry>();
    }
}