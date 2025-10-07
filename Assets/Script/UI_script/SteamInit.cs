using UnityEngine;
using Steamworks;

public class SteamInit : MonoBehaviour
{
    public static bool IsReady { get; private set; }
    public static event System.Action OnReady;

    bool inited;

    void Awake()
    {
        try
        {
            inited = SteamAPI.Init();
            IsReady = inited;
            if (inited)
            {
                Debug.Log($"[Steam] OK Persona={SteamFriends.GetPersonaName()} / ID={SteamUser.GetSteamID().m_SteamID}");
                OnReady?.Invoke(); // ������������ʒm
            }
            else
            {
                Debug.LogError("SteamAPI.Init ���s�B");
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("Steamworks DLL ��������Ȃ�: " + e);
        }
    }

    void Update() { if (inited) SteamAPI.RunCallbacks(); }
    void OnApplicationQuit() { if (inited) SteamAPI.Shutdown(); }
}
