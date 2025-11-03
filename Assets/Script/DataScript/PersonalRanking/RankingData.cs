using System;
using System.Collections.Generic;

[Serializable]
public class ScoreEntry
{
    public float time;
    public string replayFileName;
}

[Serializable]
public class StageRanking
{
    public string stageId;
    public List<ScoreEntry> scores;

    public StageRanking(string id)
    {
        stageId = id;
        scores = new List<ScoreEntry>();
    }
}

[Serializable]
public class PersonalRankings
{
    public List<StageRanking> allStageRankingsList;

    public PersonalRankings()
    {
        allStageRankingsList = new List<StageRanking>();
    }
}