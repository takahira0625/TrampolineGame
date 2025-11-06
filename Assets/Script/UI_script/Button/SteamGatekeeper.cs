using Steamworks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement; // ★シーン遷移に必要
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

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
    //[SerializeField] private bool autoHideWhenSteamReady = true; // SteamまたはGuestでOKなら自動で隠す（※今回の仕様変更では非表示が主なので注意）

    [Header("Guest Login (Optional)")]
    [SerializeField] private Button guestLoginButton;          // ゲストボタン（任意）
    [SerializeField] private GameObject afterLoginHideTarget;  // ログイン後に隠すUI（任意）

    public bool IsSteamReady => SteamAPI.IsSteamRunning();
    private bool guestMode = false; // 現在ゲストモード中かどうか

    private bool AlreadyLogin = false; // プレイ可能状態を追跡する

    void Start()
    {
        // ... (イベント登録のロジックはそのまま) ...
        if (guestLoginButton != null)
        {
            guestLoginButton.onClick.RemoveAllListeners();
            guestLoginButton.onClick.AddListener(OnClickGuestLogin);
        }

        // SteamInitのイベントに登録
        SteamInit.OnReady += OnSteamInitReady;

        // ★修正: 初期起動時に自動でUIを隠す処理はしない
        // ApplyState(SteamAPI.IsSteamRunning()); // コメントアウトまたは削除
    }

    void OnDestroy()
    {
        SteamInit.OnReady -= OnSteamInitReady;
    }

    /// <summary>
    /// SteamAPI.Initが成功したときに呼ばれる（後からSteamログインした場合を含む）
    /// </summary>
    private void OnSteamInitReady()
    {
        Debug.Log("[SteamGatekeeper] SteamInit OnReadyを受信しました。");
        // Steamが初期化されたら、Steamモードで遷移可能状態をチェックする
        ApplyState(true);
    }

    /// <summary>
    /// Steamログイン誘導ボタンの処理 (外部から呼ばれる「Steamでログイン」ボタンを想定)
    /// </summary>
    public void OnClickLoginSteam()
    {
        // 1. Steamが既に初期化済みであれば、即座に遷移
        if (SteamInit.IsReady)
        {
            ApplyState(true);
            return;
        }

        // 2. Steamが起動していない場合、クライアント起動を促す
        bool isRunning = SteamAPI.IsSteamRunning();
        if (!isRunning)
        {
            try
            {
                UnityEngine.Application.OpenURL("steam://open/main");
                // クライアント起動後はSteamInit.csが再初期化を試み、成功すればOnSteamInitReady()が呼ばれ、そこから遷移する
            }
            catch
            {
                UnityEngine.Application.OpenURL("https://store.steampowered.com/login/");
            }
        }
    }

    /// <summary>
    /// ゲストモードで即プレイ開始（名前・IDなし）(外部から呼ばれる「ゲストで開始」ボタンを想定)
    /// </summary>
    public void OnClickGuestLogin()
    {
        guestMode = true;
        Debug.Log("[GuestLogin] ゲストモードで開始します（名前・IDは発行しません）。");

        // ゲストモードは確認不要なので、即座に遷移
        ApplyState(SteamInit.IsReady);
    }

    /// <summary>
    /// Steamまたはゲストモードで開始可能かを判定しUIを更新し、シーン遷移
    /// </summary>
    private void ApplyState(bool steamReady)
    {
        bool canStart = steamReady || guestMode;

        // ログイン可能になったらパネルを非表示
        if (canStart)
        {
            if (steamGatePanel != null)
            {
                steamGatePanel.SetActive(false);
            }

            if (afterLoginHideTarget != null)
            {
                afterLoginHideTarget.SetActive(false);
            }

            // ★最重要: StageSelectScene1_6に遷移（初回ログイン時のみ）
            if (!AlreadyLogin)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelectScene1_6");
            }
        }

        // ログイン状態を記録
        AlreadyLogin = canStart;
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