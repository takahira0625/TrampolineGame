using System;
using System.Collections.Generic;
using UnityEngine;

// 1ステージ分のGold/Silverタイムのセット
[Serializable]
public class StageBadgeTime
{
    // [Tooltip("このタイムより速ければGold")]
    public float goldTime = 30.0f; // 例: 30秒

    // [Tooltip("このタイムより速ければSilver")]
    public float silverTime = 60.0f; // 例: 60秒
}

// "Assets" > "Create" メニューからこのデータアセットを作成できるようにする
[CreateAssetMenu(fileName = "BadgeTimeSettings", menuName = "Ranking/Badge Time Settings")]
public class BadgeThresholds : ScriptableObject
{
    // 12ステージ分のデータをインスペクターで設定できるようにする
    [Header("全12ステージのバッジタイム設定")]
    [Tooltip("要素 0 = Stage 1, 要素 1 = Stage 2 ... のタイムを設定します")]
    public List<StageBadgeTime> allStageThresholds = new List<StageBadgeTime>(12);
}