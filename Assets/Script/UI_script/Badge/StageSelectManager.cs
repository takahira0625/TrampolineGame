using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // 必須
using System.Linq; // 必須

// バッジの種類を定義する「列挙型」
public enum BadgeType
{
    None,   // バッジなし
    Bronze, // ブロンズ
    Silver, // シルバー
    Gold    // ゴールド
}

public class StageSelectManager : MonoBehaviour
{
    // シーン内の他のスクリプト(各ボタン)がアクセスできるようにシングルトンにする
    public static StageSelectManager Instance { get; private set; }

    [Header("バッジのタイム設定")]
    [Tooltip("上記で作成した BadgeTimeSettings.asset を設定")]
    [SerializeField] private BadgeThresholds badgeTimeSettings;

    // --- JSON読み込み用 (RankingSceneUI.cs と同じ) ---
    private string personalSaveFilePath;
    private PersonalRankings currentPersonalRankings;

    void Awake()
    {
        // シングルトン初期化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 起動時にJSONを読み込む
        InitializePersonalRanking();
        LoadPersonalRankings();
    }

    /// 指定されたステージ番号のバッジタイプを返す (各ボタンから呼ばれる)
    public BadgeType GetBadgeForStage(int stageNum)
    {
        if (currentPersonalRankings == null || badgeTimeSettings == null)
        {
            return BadgeType.None; // データが読み込めていない
        }

        string stageId = $"Stage-{stageNum}";

        // 1. JSONから該当ステージの記録を探す
        StageRanking stageRank = currentPersonalRankings.allStageRankingsList
            .FirstOrDefault(r => r.stageId == stageId);

        // 2. 記録があるかチェック
        if (stageRank == null || stageRank.scores.Count == 0)
        {
            // 一度もクリアしていない
            return BadgeType.None;
        }

        // 3. 記録がある場合、ベストタイム（= リストの先頭）を取得
        float bestTime = stageRank.scores[0].time;

        // 4. 閾値データから、このステージ用のタイムを取得
        // (stageNum は 1～12 なので、リストのインデックス 0～11 に合わせる)
        if (stageNum - 1 < 0 || stageNum - 1 >= badgeTimeSettings.allStageThresholds.Count)
        {
            Debug.LogError($"BadgeTimeSettings にステージ {stageNum} (インデックス {stageNum - 1}) の設定がありません");
            return BadgeType.None;
        }
        StageBadgeTime threshold = badgeTimeSettings.allStageThresholds[stageNum - 1];

        // 5. タイムを比較してバッジを決定
        if (bestTime <= threshold.goldTime)
        {
            return BadgeType.Gold;
        }
        else if (bestTime <= threshold.silverTime)
        {
            return BadgeType.Silver;
        }
        else
        {
            // 一度はクリアしている (Gold, Silver ではない)
            return BadgeType.Bronze;
        }
    }


    // --- 以下、RankingSceneUI.cs からコピーしたJSON読み込み処理 ---

    private void InitializePersonalRanking()
    {
        personalSaveFilePath = Path.Combine(Application.persistentDataPath, "personal_rankings.json");
    }

    private void LoadPersonalRankings()
    {
        if (!File.Exists(personalSaveFilePath))
        {
            currentPersonalRankings = new PersonalRankings();
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
            Debug.LogError($"[StageSelectManager] 個人ランキングの読み込み失敗: {e.Message}");
            currentPersonalRankings = new PersonalRankings();
        }
    }
}