using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SteamGatekeeper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject steamGatePanel;
    [SerializeField] private Button startButton;
    [SerializeField] private SteamNameDisplay nameDisplay;

    [Header("Options")]
    [SerializeField] private bool autoHideWhenSteamReady = true;

    [Header("Guest Login (Optional)")]
    [SerializeField] private Button guestLoginButton;
    [SerializeField] private GameObject afterLoginHideTarget;

    private bool guestMode = false;
    private bool personaReady = false;   // ★NameDisplayからの通知でtrue

    void Start()
    {
        if (guestLoginButton != null)
        {
            guestLoginButton.onClick.RemoveAllListeners();
            guestLoginButton.onClick.AddListener(OnClickGuestLogin);
        }
        ApplyState();

        StartCoroutine(AutoCloseWhenAlreadyLoggedIn());
    }

    public void OnClickLoginSteam()
    {
        guestMode = false;

        bool running = SteamAPI.IsSteamRunning();
        bool logged = false;

        // Steamworks 初期化済みのときだけ BLoggedOn を参照
        if (SteamInit.IsReady && running)
            logged = SteamUser.BLoggedOn();

        // ★ 未起動 または 未ログイン の場合は Steam を前面に出す
        if (!running || !logged)
        {
            try
            {
                UnityEngine.Application.OpenURL("steam://open/main"); // クライアントを前面化
            }
            catch
            {
                // 最悪ブラウザのログインでも
                UnityEngine.Application.OpenURL("https://store.steampowered.com/login/");
            }
        }

        // ★ ここで Display 側の再チェックを即スタート（待機コルーチン&コールバックが走る）
        nameDisplay?.TryRefreshAndNotify();

        UnityEngine.Debug.Log("[SteamGatekeeper] Login clicked; waiting persona name...");
    }



    public void OnClickGuestLogin()
    {
        guestMode = true;
        Debug.Log("[SteamGatekeeper] Guest mode");
        ApplyState();
    }

    /// <summary>SteamNameDisplay から呼ばれる。ユーザー名が表示できた＝準備完了。</summary>
    public void NotifySteamPersonaReady()
    {
        personaReady = true;
        Debug.Log("[SteamGatekeeper] Persona ready → close gate");
        ApplyState();
    }

    private void ApplyState()
    {
        bool canStart = personaReady || guestMode;

        if (startButton) startButton.interactable = canStart;
        if (steamGatePanel) steamGatePanel.SetActive(!(canStart && autoHideWhenSteamReady));
        if (afterLoginHideTarget) afterLoginHideTarget.SetActive(!canStart);

        UnityEngine.Debug.Log($"[Gatekeeper] ApplyState: personaReady={personaReady}, guestMode={guestMode}, canStart={canStart}");
    }


    // SteamGatekeeper.cs の中に追加
    private IEnumerator AutoCloseWhenAlreadyLoggedIn()
    {
        // SteamInit 初期化完了まで待機
        while (!SteamInit.IsReady) yield return null;

        // 最大 3 秒ほど様子を見る（0.25s × 12）
        for (int i = 0; i < 12 && !personaReady; i++)
        {
            // クライアント起動 & ログイン済みか
            if (SteamAPI.IsSteamRunning() && SteamUser.BLoggedOn())
            {
                string persona = SteamFriends.GetPersonaName();
                if (!string.IsNullOrEmpty(persona) && persona != "[unknown]")
                {
                    UnityEngine.Debug.Log($"[Gatekeeper] auto-close: persona={persona}");
                    personaReady = true;
                    ApplyState(); // ★ここで確実にパネルを閉じる
                    yield break;
                }
                else
                {
                    // まだ空なら情報要求を出して少し待つ
                    SteamFriends.RequestUserInformation(SteamUser.GetSteamID(), true);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
        UnityEngine.Debug.Log("[Gatekeeper] auto-close: not ready yet");
    }

}
