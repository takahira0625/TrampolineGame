using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamGatekeeper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject steamGatePanel;
    [SerializeField] Button startButton;

    [Header("Options")]
    [SerializeField] bool autoHideWhenSteamReady = true;

    void Start()
    {
        ApplyState(SteamAPI.IsSteamRunning());
    }

    public void OnClickLoginSteam()
    {
        bool isRunning = SteamAPI.IsSteamRunning();
        ApplyState(isRunning);

        if (!isRunning)
        {
            try
            {
                // ① Steamアプリを開く（Steamクライアントがインストールされていれば起動）
                UnityEngine.Application.OpenURL("steam://open/main");
            }
            catch
            {
                // ② 万一エラー（Steamが未インストールなど）の場合はWebブラウザへフォールバック
                UnityEngine.Application.OpenURL("https://store.steampowered.com/login/");
            }
        }
    }


    void ApplyState(bool steamReady)
    {
        // パネルの表示／非表示
        if (steamGatePanel)
            steamGatePanel.SetActive(!steamReady || !autoHideWhenSteamReady);

        // スタートボタンの有効／無効
        if (startButton)
            startButton.interactable = steamReady;
    }
}
