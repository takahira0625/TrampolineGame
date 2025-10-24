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

        if (entries.Count >= 1) firstText.text = ComposeLine(entries[0]);
        if (entries.Count >= 2) secondText.text = ComposeLine(entries[1]);
        if (entries.Count >= 3) thirdText.text = ComposeLine(entries[2]);
    }

    private string ComposeLine(RankingEntry e)
    {
        return $"{e.out_display_name}   {GameManager.FormatTime(e.timeSeconds)}";
    }
}
