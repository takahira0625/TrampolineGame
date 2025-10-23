using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Debug �� Unity �̂��̂��g���i�����܂������j
using Debug = UnityEngine.Debug;

public class RankingBoardLeader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform rankingContainer;     // Top3 ����ׂ�e
    [SerializeField] private GameObject rankingEntryPrefab;  // �q�� RankText / NameText / ScoreText (TMP) ������
    [SerializeField] private TextMeshProUGUI yourScoreText;  // �����̃X�R�A�\�����i�C�Ӂj

    [Header("Data")]
    [SerializeField] private ScoreReceiver receiver;         // ���V�[������ ScoreReceiver�i���w��Ȃ玩�������j
    [SerializeField] private bool showSteamIdInName = false; // ���O�̉��� SteamID �𕹋L����

    private void Start()
    {
        // ��M�R���|�[�l���g�̊m��
        if (receiver == null) receiver = FindObjectOfType<ScoreReceiver>();
        if (receiver == null)
        {
            Debug.LogError("[RankingBoardLeader] ScoreReceiver ��������܂���B");
            return;
        }

        // �����̃X�R�A���ɕ\���iGameManager �� DontDestroyOnLoad �O��j
        ShowYourScore();

        // Top3 ���擾���`��
        receiver.FetchTop3(RenderTop3);
    }

    /// <summary>�����̃X�R�A�iFinalTime�j�� yourScoreText �ɕ\��</summary>
    private void ShowYourScore()
    {
        if (yourScoreText == null) return;

        float myTime = (GameManager.instance != null) ? GameManager.instance.FinalTime : -1f;
        if (myTime >= 0f)
        {
            // �����ڂ𐮂���iMM:SS.ff�j
            yourScoreText.text = $"Your Time: {GameManager.FormatTime(myTime)}";
        }
        else
        {
            yourScoreText.text = "Your Time: --:--.--";
        }
    }

    /// <summary>Top3 ��`��</summary>
    private void RenderTop3(List<RankingEntry> entries)
    {
        if (rankingContainer == null)
        {
            Debug.LogError("[RankingBoardLeader] rankingContainer �����ݒ�ł��B");
            return;
        }
        if (rankingEntryPrefab == null)
        {
            Debug.LogError("[RankingBoardLeader] rankingEntryPrefab �����ݒ�ł��B");
            return;
        }

        // �����s���N���A
        foreach (Transform c in rankingContainer) Destroy(c.gameObject);

        // �f�[�^�Ȃ�
        if (entries == null || entries.Count == 0)
        {
            var empty = Instantiate(rankingEntryPrefab, rankingContainer);
            SetText(empty.transform, "RankText", "-");
            SetText(empty.transform, "NameText", "�f�[�^�Ȃ�");
            SetText(empty.transform, "ScoreText", "-");
            return;
        }

        // �ő�3���ɐ������ĕ\��
        int display = Mathf.Min(entries.Count, 3);
        for (int i = 0; i < display; i++)
        {
            var e = entries[i];
            var go = Instantiate(rankingEntryPrefab, rankingContainer);

            SetText(go.transform, "RankText", (i + 1).ToString());

            string name = e.displayName;
            if (showSteamIdInName) name += $" ({e.steamId})";
            SetText(go.transform, "NameText", name);

            // �T�[�o�͕��̃~���b��Ԃ��AScoreReceiver �ŕb�ɕϊ��ς� �� �����ł͕\�����`����
            SetText(go.transform, "ScoreText", GameManager.FormatTime(e.timeSeconds));
        }
    }

    /// <summary>�q�I�u�W�F�N�g childName �� TMP �e�L�X�g�� value ������i���S�Łj</summary>
    private void SetText(Transform parent, string childName, string value)
    {
        var child = parent.Find(childName);
        if (child == null)
        {
            Debug.LogWarning($"[RankingBoardLeader] '{childName}' ���v���n�u���Ɍ�����܂���B");
            return;
        }
        var tmp = child.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogWarning($"[RankingBoardLeader] '{childName}' �� TextMeshProUGUI ������܂���B");
            return;
        }
        tmp.text = value;
    }
}
