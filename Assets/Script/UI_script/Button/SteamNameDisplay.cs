using UnityEngine;
using TMPro;
using Steamworks;
using System.Collections;

public class SteamNameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private string format = "�悤�����A{0} ����I";
    [SerializeField] private string notReadyText = "Steam�����O�C��";
    [SerializeField] private SteamGatekeeper gateKeeper;

    private bool notifiedPersonaReady = false;
    private Coroutine waitCo;

    // �� Persona�X�V�̃R�[���o�b�N
    private Callback<PersonaStateChange_t> personaChanged;

    void OnEnable()
    {
        if (waitCo != null) StopCoroutine(waitCo);

        if (SteamInit.IsReady)
        {
            HookPersonaCallback();                // ���w��
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
        personaChanged = null; // Steamworks.NET �͔j���ōw�ǉ���
    }

    private void HandleSteamReady()
    {
        HookPersonaCallback();                    // ���w��
        if (waitCo != null) StopCoroutine(waitCo);
        waitCo = StartCoroutine(WaitPersonaAndNotify());
    }

    private void HookPersonaCallback()
    {
        if (personaChanged == null)
        {
            personaChanged = Callback<PersonaStateChange_t>.Create((ev) =>
            {
                // �����̏�񂪍X�V���ꂽ�H
                if (!SteamInit.IsReady) return;
                var me = SteamUser.GetSteamID();
                if (ev.m_ulSteamID == me.m_SteamID)
                {
                    Debug.Log("[SteamNameDisplay] PersonaStateChange for me �� retry refresh");
                    TryRefreshAndNotify(); // ���X�V�̂��тɍĎ��s
                }
            });
        }
    }

    private IEnumerator WaitPersonaAndNotify()
    {
        const int MAX_TRIES = 60;  // 0.5s���݂Ŗ�30�b
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
                // ���v�����o���Ă���Ď��s
                SteamFriends.RequestUserInformation(me, true);
                if (nameText) nameText.text = notReadyText;
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // �擾�ł���
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
        // �P�����s�iInit�����▢���O�C�����͉������Ȃ��j
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
