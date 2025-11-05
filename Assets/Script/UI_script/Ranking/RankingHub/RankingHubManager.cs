using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // JSON読み込み用
using System.Linq; // JSON読み込み用

// 1. ボタンとハイライト用Imageをセットにするためのクラス
[System.Serializable]
public class HighlightedButton
{
    public Button button;      // 押すためのボタン
    public Image highlightImage; // 色を変えるためのImage (ボタン自体でもOK)
}

// 2. どちらのタブがアクティブか
public enum RankingTab
{
    Online,
    Personal
}

public class RankingHubManager : MonoBehaviour
{
    // --- シングルトン ---
    public static RankingHubManager Instance { get; private set; }

    [Header("現在の状態")]
    private int currentStageNum = 1; // 現在選択中のステージ番号 (デフォルト1)
    private RankingTab currentTab = RankingTab.Online; // 現在のタブ (デフォルトOnline)

    [Header("データ (JSON)")]
    private PersonalRankings allPersonalRankings; // 読み込んだ個人ランキング
    private string personalSaveFilePath;

    [Header("タブ設定")]
    [SerializeField] private HighlightedButton onlineTab;
    [SerializeField] private HighlightedButton personalTab;
    [SerializeField] private GameObject onlinePanel; // Onlineの詳細パネル
    [SerializeField] private GameObject personalPanel; // Personalの詳細パネル

    [Header("ステージボタン設定")]
    [Tooltip("Scroll View の Content の子として配置したボタン12個")]
    [SerializeField] private List<HighlightedButton> stageButtons;

    [Header("色設定")]
    [SerializeField] private Color selectedColor = Color.yellow; // 選択中の色
    [SerializeField] private Color deselectedColor = Color.gray; // 非選択の色

    [Header("ランキング表示 (Online)")]
    [SerializeField] private Text[] onlineRank_Names; // 3つ分の名前
    [SerializeField] private Text[] onlineRank_Times; // 3つ分のタイム

    [Header("ランキング表示 (Personal)")]
    [SerializeField] private Text[] personalRank_Times; // 3つ分のタイム
    [SerializeField] private Button[] personalRank_ReplayButtons; // 3つ分のリプレイボタン


    // --- 初期化 ---

    void Awake()
    {
        // シングルトンとJSON読み込み
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePersonalRanking();
        LoadPersonalRankings();
    }

    void Start()
    {
        // ==== 1. デフォルト表示の解決 ====
        // シーン開始時に、強制的に「Stage 1」と「Online」を選択した状態にする
        SelectStage(1);
        SelectTab(0); // 0 = Online
    }


    // --- 2. 機能 (ボタンから呼ばれるメソッド) ---

    /// ステージボタンが押された時に呼ばれる
    public void SelectStage(int stageNum)
    {
        currentStageNum = stageNum;

        // (A) ステージボタンの見た目を更新
        UpdateStageButtonVisuals();

        // (B) メインパネルの表示を更新
        RefreshDisplayPanel();
    }

    // Online/Personalタブが押された時に呼ばれる
    public void SelectTab(int tabIndex)
    {
        // int を enum に変換
        currentTab = (RankingTab)tabIndex;

        // (A) タブの見た目を更新
        UpdateTabVisuals();

        // (B) メインパネルの表示を更新
        RefreshDisplayPanel();
    }

    // リプレイボタンが押された時に呼ばれる
    public void OnReplayButtonClicked(int rank)
    {
        Debug.Log($"ステージ {currentStageNum} の {rank + 1} 位のリプレイを開始します");

        // TODO: ここでリプレイ再生処理を呼び出す
        // (例: `allPersonalRankings` から該当の `replayFileName` を取得して再生)
    }


    // --- 3. 内部処理 (UIを更新するメソッド) ---

    // 現在の状態（タブとステージ）に基づいて、パネル全体を更新する
    void RefreshDisplayPanel()
    {
        if (currentTab == RankingTab.Online)
        {
            onlinePanel.SetActive(true);
            personalPanel.SetActive(false);
            UpdateOnlinePanel();
        }
        else // Personal
        {
            onlinePanel.SetActive(false);
            personalPanel.SetActive(true);
            UpdatePersonalPanel();
        }
    }

    // オンラインパネルの内容を、現在のステージ番号で更新
    void UpdateOnlinePanel()
    {
        // TODO: ここで外部DBから currentStageNum のランキングを取得する処理
        // (例: ScoreSender.Instance.GetBoard(currentStageNum, OnOnlineDataReceived);)

        // (↓ 以下はダミーデータ。実際のデータ取得後にコールバックで実行する)
        onlineRank_Names[0].text = $"OYU_DAYO_S{currentStageNum}";
        onlineRank_Times[0].text = "00:10.00";
        onlineRank_Names[1].text = "PLAYER_B";
        onlineRank_Times[1].text = "00:11.00";
        onlineRank_Names[2].text = "PLAYER_C";
        onlineRank_Times[2].text = "00:12.00";
    }

    // 個人パネルの内容を、現在のステージ番号で更新
    void UpdatePersonalPanel()
    {
        string stageId = $"Stage-{currentStageNum}";

        // 読み込んだJSONから該当ステージの記録(ScoreEntryのリスト)を取得
        List<ScoreEntry> scores = GetPersonalRankingsForStage(stageId);

        // 3位までのランキングとリプレイボタンを更新
        for (int i = 0; i < 3; i++)
        {
            if (scores != null && i < scores.Count)
            {
                // 記録がある場合
                personalRank_Times[i].text = GameManager.FormatTime(scores[i].time);
                bool hasReplay = !string.IsNullOrEmpty(scores[i].replayFileName);

                // ボタンのGameObjectは常に表示する
                personalRank_ReplayButtons[i].gameObject.SetActive(true);
                // 「押せるかどうか」をデータ(hasReplay)で切り替える
                personalRank_ReplayButtons[i].interactable = hasReplay;
            }
            else
            {
                // 記録がない場合 (タイムもボタンも非表示)
                personalRank_Times[i].text = "--:--.--";
                personalRank_ReplayButtons[i].gameObject.SetActive(false); // 記録がない行はボタンごと非表示
            }
        }
    }

    // --- 4. ビジュアル更新 (ハイライト) ---

    void UpdateStageButtonVisuals()
    {
        for (int i = 0; i < stageButtons.Count; i++)
        {
            if (i == (currentStageNum - 1)) // 選択中のボタン
            {
                stageButtons[i].highlightImage.color = selectedColor;
            }
            else // 非選択のボタン
            {
                stageButtons[i].highlightImage.color = deselectedColor;
            }
        }
    }

    void UpdateTabVisuals()
    {
        if (currentTab == RankingTab.Online)
        {
            onlineTab.highlightImage.color = selectedColor;
            personalTab.highlightImage.color = deselectedColor;
        }
        else // Personal
        {
            onlineTab.highlightImage.color = deselectedColor;
            personalTab.highlightImage.color = selectedColor;
        }
    }


    // --- 5. JSON読み込み (RankingSceneUI/StageSelectManager と同じ) ---

    private void InitializePersonalRanking()
    {
        personalSaveFilePath = Path.Combine(Application.persistentDataPath, "personal_rankings.json");
    }

    private void LoadPersonalRankings()
    {
        if (!File.Exists(personalSaveFilePath))
        {
            allPersonalRankings = new PersonalRankings();
            return;
        }
        try
        {
            string json = File.ReadAllText(personalSaveFilePath);
            allPersonalRankings = JsonUtility.FromJson<PersonalRankings>(json);
            if (allPersonalRankings == null) allPersonalRankings = new PersonalRankings();
        }
        catch (Exception e)
        {
            Debug.LogError($"[RankingHubManager] 個人ランキングの読み込み失敗: {e.Message}");
            allPersonalRankings = new PersonalRankings();
        }
    }

    public List<ScoreEntry> GetPersonalRankingsForStage(string stageId)
    {
        if (allPersonalRankings == null) return new List<ScoreEntry>();
        StageRanking stageRank = allPersonalRankings.allStageRankingsList.FirstOrDefault(r => r.stageId == stageId);
        return (stageRank != null) ? stageRank.scores : new List<ScoreEntry>();
    }
}