using UnityEngine;
using TMPro;
using Steamworks;

public class SteamNameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private string format = "�悤�����A{0} ����I";
    [SerializeField] private string notReadyText = "Steam�����O�C��";

    void OnEnable()
    {
        if (SteamInit.IsReady) { Refresh(); }
        else
        {
            if (nameText) nameText.text = notReadyText;
            SteamInit.OnReady += HandleSteamReady; // ��������������҂�
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
        if (nameText == null) { Debug.LogWarning("[SteamNameDisplay] Name Text ������"); return; }
        // ������ Init �ς݂ŌĂ΂��z��
        string persona = SteamFriends.GetPersonaName();
        nameText.text = string.Format(format, persona);
    }
}
