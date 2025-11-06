using System;
using System.Collections.Generic; // ← using が必要です

[Serializable]
public class ScoreEntry // ← 個人ランキングの1回の記録 (タイムとリプレイファイル名)
{
    public float time;
    public string replayFileName;
}

[Serializable]
public class StageRanking // ← 1ステージ分の個人ランキング
{
    public string stageId;
    public List<ScoreEntry> scores;

    // ↓ コンストラクタ (1回だけ定義)
    public StageRanking(string id)
    {
        stageId = id;
        scores = new List<ScoreEntry>();
    }
}

[Serializable]
public class PersonalRankings // ← 全ステージの個人ランキング
{
    public int lastClearedStageNumber; // 最後にクリアしたステージ番号
    public List<StageRanking> allStageRankingsList;

    // ↓ コンストラクタ (1回だけ定義)
    public PersonalRankings()
    {
        allStageRankingsList = new List<StageRanking>();
        lastClearedStageNumber = 1;
    }
}