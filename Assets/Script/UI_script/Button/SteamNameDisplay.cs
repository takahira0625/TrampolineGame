using UnityEngine;
using TMPro;
using Steamworks;
using System.Collections;

public class SteamNameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private string format = "ようこそ、{0} さん！";
    [SerializeField] private string notReadyText = "Steam未ログイン";
    [SerializeField] private SteamGatekeeper gateKeeper;

    private bool notifiedPersonaReady = false;
    private Coroutine waitCo;

    // ★ Persona更新のコールバック
    private Callback<PersonaStateChange_t> personaChanged;

    void OnEnable()
    {
        if (waitCo != null) StopCoroutine(waitCo);

        if (SteamInit.IsReady)
        {
            HookPersonaCallback();                // ★購読
            waitCo = StartCoroutine(WaitPersonaAndNotify());
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
        if (waitCo != null) { StopCoroutine(waitCo); waitCo = null; }
        personaChanged = null; // Steamworks.NET は破棄で購読解除
    }

    private void HandleSteamReady()
    {
        HookPersonaCallback();                    // ★購読
        if (waitCo != null) StopCoroutine(waitCo);
        waitCo = StartCoroutine(WaitPersonaAndNotify());
    }

    private void HookPersonaCallback()
    {
        if (personaChanged == null)
        {
            personaChanged = Callback<PersonaStateChange_t>.Create((ev) =>
            {
                // 自分の情報が更新された？
                if (!SteamInit.IsReady) return;
                var me = SteamUser.GetSteamID();
                if (ev.m_ulSteamID == me.m_SteamID)
                {
                    Debug.Log("[SteamNameDisplay] PersonaStateChange for me → retry refresh");
                    TryRefreshAndNotify(); // ★更新のたびに再試行
                }
            });
        }
    }

    private IEnumerator WaitPersonaAndNotify()
    {
        const int MAX_TRIES = 60;  // 0.5s刻みで約30秒
        int tries = 0;

        while (tries++ < MAX_TRIES && !notifiedPersonaReady)
        {
            if (!SteamInit.IsReady || !SteamAPI.IsSteamRunning() || !SteamUser.BLoggedOn())
            {
                if (nameText) nameText.text = notReadyText;
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            var me = SteamUser.GetSteamID();
            string persona = SteamFriends.GetPersonaName();

            if (string.IsNullOrEmpty(persona) || persona == "[unknown]")
            {
                // 情報要求を出してから再試行
                SteamFriends.RequestUserInformation(me, true);
                if (nameText) nameText.text = notReadyText;
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // 取得できた
            if (nameText) nameText.text = string.Format(format, persona);
            if (!notifiedPersonaReady)
            {
                notifiedPersonaReady = true;
                gateKeeper?.NotifySteamPersonaReady();
            }
            yield break;
        }

        if (!notifiedPersonaReady && nameText) nameText.text = notReadyText;
    }

    public void TryRefreshAndNotify()
    {
        // 単発実行（Init未完や未ログイン時は何もしない）
        if (!SteamInit.IsReady || !SteamAPI.IsSteamRunning() || !SteamUser.BLoggedOn())
        {
            if (nameText) nameText.text = notReadyText;
            return;
        }

        string persona = SteamFriends.GetPersonaName();
        if (string.IsNullOrEmpty(persona) || persona == "[unknown]")
        {
            SteamFriends.RequestUserInformation(SteamUser.GetSteamID(), true);
            if (nameText) nameText.text = notReadyText;
            return;
        }

        if (nameText) nameText.text = string.Format(format, persona);
        if (!notifiedPersonaReady)
        {
            notifiedPersonaReady = true;
            gateKeeper?.NotifySteamPersonaReady();
        }
    }
}
