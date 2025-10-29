using UnityEngine;
using TMPro;
using Steamworks;

public class SteamNameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private string format = "ようこそ、{0} さん！";
    [SerializeField] private string notReadyText = "Steam未ログイン";
    [SerializeField] private SteamGatekeeper GateKeeper;

    private int count = 0;

    void OnEnable()
    {
        if (SteamInit.IsReady) { Refresh(); }
        else
        {
            if (nameText) nameText.text = notReadyText;
            SteamInit.OnReady += HandleSteamReady; // ★初期化完了を待つ
        }
    }

    private void Update()
    {

        if (GateKeeper != null && GateKeeper.IsSteamReady ==true)
        {
            if (count == 0)
            {
                Refresh();
                count++;
            }
        }
    }
    void OnDisable()
    {
        SteamInit.OnReady -= HandleSteamReady;
    }

    void HandleSteamReady()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (nameText == null) { Debug.LogWarning("[SteamNameDisplay] Name Text 未割当"); return; }
        // ここは Init 済みで呼ばれる想定
        string persona = SteamFriends.GetPersonaName();
        nameText.text = string.Format(format, persona);
    }
}
