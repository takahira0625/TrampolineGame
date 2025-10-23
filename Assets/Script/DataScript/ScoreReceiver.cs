using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class ScoreReceiver : MonoBehaviour
{
    [Header("Backend")]
    [SerializeField] private string backendBaseUrl = "http://127.0.0.1:8000"; 
    [SerializeField] private string mode = "all_time";                         

    [Header("Stage")]
    [SerializeField, Range(1, 12)] private int stageNumber = 1;
    public int StageNumber   
    {
        get => stageNumber;
        set => stageNumber = Mathf.Clamp(value, 1, 12);
    }

    // API���X�|���X�p�̎󂯎M
    [System.Serializable] private class ApiRow { public long steam_id; public string display_name; public int score; }
    [System.Serializable] private class ApiWrap { public string mode; public int stage; public int count; public ApiRow[] items; }

    /// <summary>���3�����擾���ăR�[���o�b�N�ŕԂ�</summary>
    public void FetchTop3(System.Action<List<RankingEntry>> onDone)
    {
        StartCoroutine(FetchRoutine(onDone));
    }

    private IEnumerator FetchRoutine(System.Action<List<RankingEntry>> onDone)
    {
        var list = new List<RankingEntry>();

        if (string.IsNullOrEmpty(backendBaseUrl))
        {
            Debug.LogError("[ScoreReceiver] backendBaseUrl �����ݒ�ł��B");
            onDone?.Invoke(list);
            yield break;
        }

        string url = $"{backendBaseUrl.TrimEnd('/')}/leaderboard/top?mode={UnityWebRequest.EscapeURL(mode)}&stage={stageNumber}&limit=3";

        using (var req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[ScoreReceiver] GET failed: {req.responseCode} {req.error} {req.downloadHandler.text}");
                onDone?.Invoke(list);
                yield break;
            }

            var json = req.downloadHandler.text;
            ApiWrap data = null;
            try
            {
                data = JsonUtility.FromJson<ApiWrap>(json);
            }
            catch
            {
                Debug.LogError($"[ScoreReceiver] JSON parse error: {json}");
                onDone?.Invoke(list);
                yield break;
            }

            if (data?.items != null)
            {
                foreach (var r in data.items)
                {
                    // �T�[�o�́u���̃~���b�v��Ԃ��z�� �� �b�ɖ߂��i�Z���قǏ�ʁj
                    float timeSec = Mathf.Abs(r.score) / 1000f;
                    list.Add(new RankingEntry(r.steam_id, r.display_name, timeSec));
                }
            }

            onDone?.Invoke(list);
        }
    }
}
