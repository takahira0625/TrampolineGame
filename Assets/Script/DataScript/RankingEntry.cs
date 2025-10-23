using System;
using UnityEngine;
// あいまい解消（Unity の Debug を使う）
using Debug = UnityEngine.Debug;
// Random も Unity の方を使う
using URandom = UnityEngine.Random;

[Serializable]
public class RankingEntry
{
    public long steamId;
    public string displayName;
    public float timeSeconds;  // 表示用（小さいほど上位）

    public RankingEntry(long steamId, string displayName, float timeSeconds)
    {
        this.steamId = steamId;
        this.displayName = displayName;
        this.timeSeconds = timeSeconds;
    }
}