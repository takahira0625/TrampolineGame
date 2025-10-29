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
            inited = SteamAPI.Init();   // Åöèâä˙âªÇÕÇ±Ç±ÇæÇØ
            IsReady = inited;
            if (inited)
            {
                Debug.Log("[SteamInit] SteamAPI.Init OK");
                OnReady?.Invoke();
            }
            else
            {
                Debug.LogError("[SteamInit] SteamAPI.Init é∏îs");
            }
        }
        catch (DllNotFoundException e)
        {
            Debug.LogError("[SteamInit] Steamworks DLL Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒ: " + e);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (inited) SteamAPI.RunCallbacks(); // ÅöRunCallbacks ÇÕÇ±Ç±ÇæÇØ
    }

    void OnApplicationQuit()
    {
        if (inited) SteamAPI.Shutdown();
        inited = false;
        IsReady = false;
    }
}
