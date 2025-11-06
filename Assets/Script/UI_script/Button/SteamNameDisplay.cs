using Steamworks;
using TMPro;
using UnityEngine;

public class SteamNameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private string format = "ようこそ、{0} さん！";
    [SerializeField] private string notReadyText = "Steam Not Login";
    [SerializeField] private SteamGatekeeper GateKeeper;

    void OnEnable()
    {
        if (SteamInit.IsReady)
        {
            Refresh();
        }
        else
        {
            if (nameText) nameText.text = notReadyText;
            SteamInit.OnReady += HandleSteamReady;
        }
    }

    void OnDisable()
    {
        SteamInit.OnReady -= HandleSteamReady;
    }

    void HandleSteamReady()
    {
        Refresh();
        SteamInit.OnReady -= HandleSteamReady;
    }

    public void Refresh()
    {
        if (nameText == null) { Debug.LogWarning("[SteamNameDisplay] Name Text 未割当"); return; }

        // 1. ゲストモード判定
        if (GateKeeper != null && GateKeeper.IsGuestMode())
        {
            nameText.text = "ようこそ、ゲストさん！";
            return;
        }

        // 2. Steam Ready 判定
        if (!SteamInit.IsReady)
        {
            nameText.text = notReadyText;
            return;
        }

        // 3. Steamユーザー名表示
        // ここは Init 済みで呼ばれる想定
        string persona = SteamFriends.GetPersonaName();
        nameText.text = string.Format(format, persona);
    }
}