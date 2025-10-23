using System;
using UnityEngine;
// �����܂������iUnity �� Debug ���g���j
using Debug = UnityEngine.Debug;
// Random �� Unity �̕����g��
using URandom = UnityEngine.Random;

[Serializable]
public class RankingEntry
{
    public long steamId;
    public string displayName;
    public float timeSeconds;  // �\���p�i�������قǏ�ʁj

    public RankingEntry(long steamId, string displayName, float timeSeconds)
    {
        this.steamId = steamId;
        this.displayName = displayName;
        this.timeSeconds = timeSeconds;
    }
}