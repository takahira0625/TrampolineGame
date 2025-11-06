using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // JSON読み込み用
using System.Linq; // JSON読み込み用

// 1. ボタンの「見た目セット」を定義するクラス
[System.Serializable]
public class ButtonVisuals
{
    public Color imageColor = Color.white;
    public Sprite imageSprite;
    public Color textColor = Color.white;
    [Tooltip("アイコンの画像")]
    public Sprite iconSprite;              // アイコンの画像
    [Tooltip("アイコンの色")]
    public Color iconColor = Color.white;   // アイコンの色
}

// 2. ボタンとハイライト用Imageをセットにするためのクラス
[System.Serializable]
public class HighlightedButton
{
    public Button button;      // 押すためのボタン
    public Image highlightImage; // 色/画像を変えるためのImage
    public Text buttonText;    // 色を変えるためのText

    [Tooltip("テキストの横にあるアイコン用のImage")]
    public Image iconImage;      // アイコンImage

    [Header("ビジュアル設定")]
    public ButtonVisuals selectedVisuals;   // ★ 選択時の見た目セット
    public ButtonVisuals deselectedVisuals; // ★ 非選択時の見た目セット
}

// 3. どちらのタブがアクティブか
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

    [Header("ランキング表示 (Online)")]
    [SerializeField] private Text[] onlineRank_Names; // 3つ分の名前
    [SerializeField] private Text[] onlineRank_Times; // 3つ分のタイム

    [Header("ランキング表示 (Personal)")]
    [SerializeField] private Text[] personalRank_Times; // 3つ分のタイム
    [SerializeField] private Button[] personalRank_ReplayButtons; // 3つ分のリプレイボタン

    private ScoreSender scoreSender;

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
        // --- 【追記】 ScoreSender を探す ---
        scoreSender = FindObjectOfType<ScoreSender>();
        if (scoreSender == null)
        {
            // GameManager が ScoreSenderPrefab から自動生成するはずなので、
            // このシーンに ScoreSender がいなくてもエラーとは限らない。
            // DontDestroyOnLoad されているはずのインスタンスを探す。
            Debug.LogWarning("[RankingHubManager] ScoreSender が見つかりません。");
        }

        // --- 【追記】 ScoreSender からのイベントを購読 ---
        ScoreSender.OnBoardDataReceived += HandleBoardData;
        ScoreSender.OnBoardDataFailed += HandleBoardError;

        // ==== 1. デフォルト表示の解決 ====
        // シーン開始時に、強制的に「Stage 1」と「Online」を選択した状態にする
        SelectStage(1);
        SelectTab(0); // 0 = Online
    }

    void OnDestroy()
    {
        ScoreSender.OnBoardDataReceived -= HandleBoardData;
        ScoreSender.OnBoardDataFailed -= HandleBoardError;
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
        if (scoreSender == null)
        {
            // scoreSender が見つからない場合、再度探してみる
            scoreSender = FindObjectOfType<ScoreSender>();
            if (scoreSender == null)
            {
                HandleBoardError("ScoreSender not found");
                return;
            }
        }

        // 1. UIを「読み込み中...」の状態にする
        for (int i = 0; i < 3; i++)
        {
            onlineRank_Names[i].text = "Loading...";
            onlineRank_Times[i].text = "--:--.--";
        }

        // 2. ScoreSenderに「現在のステージのデータをください」とリクエスト
        scoreSender.RequestBoard(currentStageNum);
    }

    // ScoreSender がデータ受信に成功したときに呼ばれる
    private void HandleBoardData(List<RankingEntry> board)
    {
        // 1. まず全スロットを「記録なし」でクリア
        for (int i = 0; i < 3; i++)
        {
            onlineRank_Names[i].text = "-";
            onlineRank_Times[i].text = "--:--.--";
        }

        // 2. 受信したデータ (board) があれば、UIに設定
        if (board != null)
        {
            foreach (var entry in board)
            {
                if (entry.out_rnk >= 1 && entry.out_rnk <= 3)
                {
                    int index = entry.out_rnk - 1;

                    onlineRank_Names[index].text = entry.out_display_name;

                    float timeInSeconds = -entry.out_score / 1000f;
                    onlineRank_Times[index].text = GameManager.FormatTime(timeInSeconds);
                }
            }
        }
    }

    // ScoreSender がデータ受信に失敗したときに呼ばれる
    private void HandleBoardError(string errorMessage)
    {
        Debug.LogError(errorMessage);
        for (int i = 0; i < 3; i++)
        {
            onlineRank_Names[i].text = "ERROR";
            onlineRank_Times[i].text = "N/A";
        }
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
            if (stageButtons[i] == null || stageButtons[i].button == null || stageButtons[i].highlightImage == null) continue;

            // 適用する「見た目セット」を決定
            ButtonVisuals visualsToApply;

            if (i == (currentStageNum - 1)) // 選択中のボタン
            {
                stageButtons[i].button.interactable = false;
                visualsToApply = stageButtons[i].selectedVisuals; // ★ 選択時のセット
            }
            else // 非選択のボタン
            {
                stageButtons[i].button.interactable = true;
                visualsToApply = stageButtons[i].deselectedVisuals; // ★ 非選択時のセット
            }

            // 決定した「見た目セット」をUIに適用
            stageButtons[i].highlightImage.color = visualsToApply.imageColor;
            stageButtons[i].highlightImage.sprite = visualsToApply.imageSprite;
            if (stageButtons[i].buttonText != null)
                stageButtons[i].buttonText.color = visualsToApply.textColor;
        }
    }

    void UpdateTabVisuals()
    {
        // 適用するビジュアルセットを決定
        var onlineVisuals = (currentTab == RankingTab.Online) ? onlineTab.selectedVisuals : onlineTab.deselectedVisuals;
        var personalVisuals = (currentTab == RankingTab.Personal) ? personalTab.selectedVisuals : personalTab.deselectedVisuals;

        // Onlineタブに適用
        onlineTab.button.interactable = (currentTab != RankingTab.Online); // 選択中じゃない時だけ押せる
        onlineTab.highlightImage.color = onlineVisuals.imageColor;
        onlineTab.highlightImage.sprite = onlineVisuals.imageSprite;
        if (onlineTab.buttonText != null)
            onlineTab.buttonText.color = onlineVisuals.textColor;

        if (onlineTab.iconImage != null)
        {
            onlineTab.iconImage.sprite = onlineVisuals.iconSprite;
            onlineTab.iconImage.color = onlineVisuals.iconColor;
        }

        // Personalタブに適用
        personalTab.button.interactable = (currentTab != RankingTab.Personal); // 選択中じゃない時だけ押せる
        personalTab.highlightImage.color = personalVisuals.imageColor;
        personalTab.highlightImage.sprite = personalVisuals.imageSprite;
        if (personalTab.buttonText != null)
            personalTab.buttonText.color = personalVisuals.textColor;

        if (personalTab.iconImage != null)
        {
            personalTab.iconImage.sprite = personalVisuals.iconSprite;
            personalTab.iconImage.color = personalVisuals.iconColor;
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