using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using UIText = UnityEngine.UI.Text;

public class RankingBoardLeaderLegacy : MonoBehaviour
{
    [Header("Scene Texts")]
    public UIText firstText;
    public UIText secondText;
    public UIText thirdText;

    public void RenderTop3(List<RankingEntry> entries)
    {
        if (entries == null || entries.Count == 0)
        {
            firstText.text = "—";
            secondText.text = "—";
            thirdText.text = "—";
            return;
        }

        if (entries.Count >= 1) firstText.text = ComposeLine(entries[0], 1);
        if (entries.Count >= 2) secondText.text = ComposeLine(entries[1], 2);
        if (entries.Count >= 3) thirdText.text = ComposeLine(entries[2], 3);
    }

    private string ComposeLine(RankingEntry e, int rank)
    {
        string rankLabel = GetRankLabel(rank);
        string name = e.out_display_name.Length > 15 ? e.out_display_name.Substring(0, 15) : e.out_display_name;
        string timeText = GameManager.FormatTime(e.timeSeconds);

        // 等幅フォントを使用する前提（Courier Newなど）
        return $"{rankLabel,-4}{name,-15}{timeText,10}";
    }


    /// <summary>
    /// 順位を「1ST」「2ND」「3RD」「4TH」形式で返す
    /// </summary>
    private string GetRankLabel(int rank)
    {
        switch (rank)
        {
            case 1: return "1ST";
            case 2: return "2ND";
            case 3: return "3RD";
            default: return $"{rank}TH";
        }
    }
}
