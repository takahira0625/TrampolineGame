using UnityEngine;
using UnityEngine.UI;          // Button
using Steamworks;              // SteamAPI.IsSteamRunning()

public class SteamGatekeeper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject steamGatePanel;  // Steamに促すパネル（未ログイン時だけ表示）
    [SerializeField] Button startButton;      // 「スタート」ボタン

    [Header("Options")]
    [SerializeField] bool autoHideWhenSteamReady = true; // 起動時にSteamOKなら自動で閉じる

    void Start()
    {
        ApplyState(SteamAPI.IsSteamRunning());
    }

    // 「Steamでログイン」ボタンから呼ぶ想定（任意）
    public void OnClickLoginSteam() => ApplyState(SteamAPI.IsSteamRunning());

    // 「ゲストで始める」ボタンは別途シーン遷移へ繋いでOK

    void ApplyState(bool steamReady)
    {
        // Panelの表示/非表示
        if (steamGatePanel)
            steamGatePanel.SetActive(!steamReady || !autoHideWhenSteamReady);

        // Startボタンの無効化/有効化
        if (startButton)
            startButton.interactable = steamReady;
    }
}
