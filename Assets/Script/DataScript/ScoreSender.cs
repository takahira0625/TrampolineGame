using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class ScoreSender : MonoBehaviour
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

    [Header("Steam")]
    [Tooltip("Steam�����������̃f�o�b�O�p�B0 �̏ꍇ�͑��M���Ȃ��B")]
    [SerializeField] private long debugSteamIdOverride = 0;

    
    public void SendClearTimeSeconds(float clearTimeSeconds)
    {
        StartCoroutine(PostScore(clearTimeSeconds));
    }

    // --- �������� ---

    private long GetSteamId()
    {
        // �� Steam�����������ł����M�ł���悤�AdebugSteamIdOverride��D��
        if (debugSteamIdOverride != 0)
            return debugSteamIdOverride;

        // Steam�A�g�Ȃ��Ńe�X�g���邽�ߏ�ɌŒ�ID��Ԃ�
        return 76561198000000000; // ������SteamID
    }

    private IEnumerator PostScore(float timeSec)
    {
        // ���̓`�F�b�N
        if (string.IsNullOrEmpty(backendBaseUrl))
        {
            Debug.LogError("[ScoreSender] backendBaseUrl �����ݒ�ł��B");
            yield break;
        }
        var baseUrl = backendBaseUrl.TrimEnd('/');

        var steamId = GetSteamId();
        if (steamId == 0)
        {
            Debug.LogError("[ScoreSender] SteamID ���擾�ł��܂���iSteam�������� or debugSteamIdOverride���ݒ�j");
            yield break;
        }

        // ���Z���^�C�����ǂ� �� �T�[�o�́u���̃~���b�v���X�R�A�Ƃ��Ď󂯎��z��
        int negativeMillis = -(int)Mathf.Round(timeSec * 1000f);

        var payload = $"{{\"steam_id\":{steamId},\"score\":{negativeMillis},\"mode\":\"{mode}\",\"stage\":{stageNumber}}}";
        var url = $"{baseUrl}/score"; // FastAPI ���� /score �G���h�|�C���g

        using (var req = new UnityWebRequest(url, "POST"))
        {
            var body = Encoding.UTF8.GetBytes(payload);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[ScoreSender] POST failed: {req.responseCode} {req.error} {req.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"[ScoreSender] POST ok: time={timeSec:F2}s �� score={negativeMillis} (mode={mode}, stage={stageNumber})");
            }
        }
    }
}
