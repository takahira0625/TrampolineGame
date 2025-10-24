using System;
using UnityEngine;

[Serializable]
public class RankingEntry
{
    public int out_rnk;
    public long out_steam_id;
    public string out_display_name;
    public long out_score;

    public float timeSeconds => Mathf.Abs((float)out_score) / 1000f;
}


