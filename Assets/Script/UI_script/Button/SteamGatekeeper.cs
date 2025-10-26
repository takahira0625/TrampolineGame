using Steamworks;
using System; // Guid用（今は未使用でもOK）
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Steam または ゲスト でゲーム開始を制御するクラス。
/// ・Steam利用時 → ランキング投稿など全機能有効
/// ・ゲスト利用時 → 一時プレイのみ（記録・保存なし）
/// </summary>
public class SteamGatekeeper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject steamGatePanel;  // ログイン案内パネル
    [SerializeField] private Button startButton;         // 「はじめる」ボタン

    [Header("Options")]
    [SerializeField] private bool autoHideWhenSteamReady = true; // SteamまたはGuestでOKなら自動で隠す

    [Header("Guest Login (Optional)")]
    [SerializeField] private Button guestLoginButton;          // ゲストボタン（任意）
    [SerializeField] private GameObject afterLoginHideTarget;  // ログイン後に隠すUI（任意）

    public bool IsSteamReady => SteamAPI.IsSteamRunning();
    private bool guestMode = false; // 現在ゲストモード中かどうか

    private bool AlreadyLogin = false;
    void Start()
    {
        // GuestLoginButtonが設定されていればイベント登録
        if (guestLoginButton != null)
        {
            guestLoginButton.onClick.RemoveAllListeners();
            guestLoginButton.onClick.AddListener(OnClickGuestLogin);
        }

        // 初期状態の反映
        ApplyState(SteamAPI.IsSteamRunning());
    }

    void Update()
    {
        if (IsSteamReady)
        {
            if (!AlreadyLogin)
            {
                ApplyState(SteamAPI.IsSteamRunning());
                AlreadyLogin = true;
            }
        }
    }

    /// <summary>
    /// Steamログイン誘導ボタンの処理
    /// </summary>
    public void OnClickLoginSteam()
    {
        bool isRunning = SteamAPI.IsSteamRunning();
        ApplyState(isRunning);

        if (!isRunning)
        {
            try
            {
                // Steamクライアントを起動
                Application.OpenURL("steam://open/main");
            }
            catch
            {
                // 失敗時はWebログインへフォールバック
                Application.OpenURL("https://store.steampowered.com/login/");
            }
        }
    }

    /// <summary>
    /// ゲストモードで即プレイ開始（名前・IDなし）
    /// </summary>
    public void OnClickGuestLogin()
    {
        guestMode = true;
        Debug.Log("[GuestLogin] ゲストモードで開始します（名前・IDは発行しません）。");

        // ログインパネルなどを非表示
        if (steamGatePanel != null) steamGatePanel.SetActive(false);

        if (afterLoginHideTarget != null) afterLoginHideTarget.SetActive(false);

        // スタートボタンを有効化
        ApplyState(SteamAPI.IsSteamRunning());
    }

    /// <summary>
    /// Steamまたはゲストモードで開始可能かを判定しUIを更新
    /// </summary>
    private void ApplyState(bool steamReady)
    {
        bool canStart = steamReady || guestMode;

        // パネルの表示/非表示
        if (steamGatePanel != null)
            steamGatePanel.SetActive(!canStart || !autoHideWhenSteamReady);

        // スタートボタンの有効/無効
        if (startButton != null)
            startButton.interactable = canStart;
    }

    // ====== 外部から呼び出す便利メソッド ======

    /// <summary>
    /// ランキング投稿が可能か？（Steamユーザーのみ）
    /// </summary>
    public static bool IsRankEligible()
    {
        return SteamAPI.IsSteamRunning();
    }

    /// <summary>
    /// 現在ゲストモードか？（一時セッション）
    /// </summary>
    public bool IsGuestMode()
    {
        return guestMode;
    }
}
