using Steamworks;
using System;
using UnityEngine;

public class SteamInit : MonoBehaviour
{
    public static bool IsReady { get; private set; }
    public static event Action OnReady;

    private bool inited;

    void Awake()
    {
        try
        {
            inited = SteamAPI.Init();   // ���������͂�������
            IsReady = inited;
            if (inited)
            {
                Debug.Log("[SteamInit] SteamAPI.Init OK");
                OnReady?.Invoke();
            }
            else
            {
                Debug.LogError("[SteamInit] SteamAPI.Init ���s");
            }
        }
        catch (DllNotFoundException e)
        {
            Debug.LogError("[SteamInit] Steamworks DLL ��������܂���: " + e);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (inited) SteamAPI.RunCallbacks(); // ��RunCallbacks �͂�������
    }

    void OnApplicationQuit()
    {
        if (inited) SteamAPI.Shutdown();
        inited = false;
        IsReady = false;
    }
}
