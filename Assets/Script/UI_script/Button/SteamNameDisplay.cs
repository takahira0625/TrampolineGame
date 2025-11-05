using Steamworks;
using TMPro;
using UnityEngine;

public class SteamNameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private string format = "ようこそ、{0} さん！";
    [SerializeField] private string notReadyText = "Steam未ログイン";
    [SerializeField] private SteamGatekeeper GateKeeper;

    // private int count = 0; // ★削除

    void OnEnable()
    {
        // Steamが既にReadyなら即座にRefresh
        if (SteamInit.IsReady)
        {
            Refresh();
        }
        else
        {
            // 未Readyなら未ログイン表示とイベント登録
            if (nameText) nameText.text = notReadyText;
            SteamInit.OnReady += HandleSteamReady; // ★初期化完了を待つ
        }
    }

    // private void Update() // ★削除
    // {
    //     // 削除
    // }

    void OnDisable()
    {
        // コンポーネントが無効になる際は、必ずイベント購読を解除
        SteamInit.OnReady -= HandleSteamReady;
    }

    void HandleSteamReady()
    {
        Refresh();
        // ★一度表示できたら購読を解除
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