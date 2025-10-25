using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using URandom = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 鍵・コイン関連
    private int totalKeys = 0;
    public int TotalKeys => totalKeys;

    public int requiredCoins = 5;
    public GameObject goalTextObject;
    public PlayerController playerController;

    private int currentCoins = 0;
    private readonly List<PlayerController> activePlayers = new List<PlayerController>();

    // ==== タイマー ====
    [Header("Timer")]
    [SerializeField] private bool autoStartTimer = false;
    private bool isTiming = false;
    private bool hasStarted = false;
    private float elapsedTime = 0f;
    public float FinalTime { get; private set; } = -1f;

    // ==== ランキング送信 ====
    [Header("Ranking")]
    [Tooltip("シーン名 Stage01..12 から自動抽出。手動で固定したい場合は 1..12 を指定")]
    [SerializeField, Range(0, 12)] private int overrideStageNumber = 0; // 0 なら自動抽出
    [SerializeField] private ScoreSender scoreSenderPrefab; // 無ければ自動生成用
    private ScoreSender scoreSender; // 実体

    // BGM
    public AudioClip gameBGM;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (goalTextObject != null) goalTextObject.SetActive(false);
        if (autoStartTimer) StartTimer();
        if (BGMManager.Instance != null && gameBGM != null) BGMManager.Instance.Play(gameBGM);

        // プレイヤー登録
        if (playerController == null) playerController = FindObjectOfType<PlayerController>();
        if (playerController != null) RegisterPlayer(playerController);

        // スコア送信用コンポーネントの確保
        EnsureScoreSender();
        ApplyStageNumberToScoreSender();
    }

    private void Update()
    {
        if (isTiming) elapsedTime += Time.deltaTime;
    }

    // ==== タイマー操作 ====
    public void StartTimer()
    {
        elapsedTime = 0f;
        FinalTime = -1f;
        isTiming = true;
        hasStarted = true;
    }

    public void StartTimerOnce()
    {
        if (!hasStarted) StartTimer();
    }

    public void StopTimer()
    {
        isTiming = false;
        FinalTime = elapsedTime;
    }

    public float ElapsedTime => elapsedTime;

    public void ResetTimerForNewRun()
    {
        isTiming = false;
        hasStarted = false;
        elapsedTime = 0f;
        FinalTime = -1f;
    }

    public static string FormatTime(float t)
    {
        if (t < 0f) return "--:--.--";
        int minutes = (int)(t / 60f);
        float seconds = t - minutes * 60f;
        return $"{minutes:00}:{seconds:00.00}";
    }

    // ==== プレイヤー管理 ====
    public void RegisterPlayer(PlayerController player)
    {
        if (!activePlayers.Contains(player))
        {
            activePlayers.Add(player);
            Debug.Log($"[Register] {player.name} / cnt={activePlayers.Count}");
        }
    }

    public void UnregisterPlayer(PlayerController player)
    {
        if (activePlayers.Contains(player))
        {
            activePlayers.Remove(player);
            Debug.Log($"[Unregister] プレイヤー削除: {player.name} / 残りプレイヤー数: {activePlayers.Count}");
        }
        else
        {
            Debug.LogWarning($"[Unregister] リストに存在しないプレイヤー: {player.name}");
        }

        if (activePlayers.Count == 0)
        {
            Debug.Log("全プレイヤーが死亡しました。GameOver。");
            GameOver();
        }
    }

    public PlayerController SpawnAdditionalPlayer(Transform originalPlayer, Vector2 velocity)
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController が未設定のため複製できません。");
            return null;
        }

        GameObject clone = Instantiate(playerController.gameObject, originalPlayer.position, Quaternion.identity);

        Vector3 offset = new Vector3(URandom.Range(-1.0f, 1.0f), 0.5f, 0f);
        clone.transform.position += offset;

        PlayerController cloneController = clone.GetComponent<PlayerController>();
        if (cloneController != null)
        {
            cloneController.canMove = true;
            RegisterPlayer(cloneController);

            Rigidbody2D cloneRb = clone.GetComponent<Rigidbody2D>();
            if (cloneRb != null)
            {
                cloneRb.velocity = velocity;
            }
        }
        Debug.Log($"プレイヤーを分裂させました！ 現在のプレイヤー数: {activePlayers.Count}");

        return cloneController;
    }

    // ==== ゴール処理 ====
    public void Goal()
    {
        StopTimer();
        if (playerController != null) playerController.canMove = false;
        if (goalTextObject != null) goalTextObject.SetActive(true);
        SaveCurrentStageNumber();
        //FinalTimerの保持
        PlayerPrefs.SetFloat("finaltimer", FinalTime);
        // スコア送信
        int stage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
        int score = Mathf.RoundToInt(-FinalTime * 1000); // 負のミリ秒（大きいほど速い）

        if (scoreSender != null)
        {
            // Steam から取得（未初期化ならフォールバック）
            long steamId = 0;
            string playerName = "Unknown";
            try
            {
                // SteamLoginManager はあなたのプロジェクトの既存クラスを想定
                if (SteamLoginManager.Initialized)
                {
                    steamId = (long)SteamLoginManager.SteamId64;
                    playerName = SteamLoginManager.PersonaName;
                }
            }
            catch { /* 参照できない環境でも落ちないようにする */ }

            scoreSender.SubmitScoreAndGetBoard(steamId, playerName, "all_time", stage, score);
        }
        else
        {
            Debug.LogWarning("[GameManager] scoreSender が見つかりません。ランキング送信をスキップします。");
        }

        // ランキングへ遷移（少し待つ場合は WaitForSeconds を延ばす）
        StartCoroutine(GotoRanking(stage));
    }

    private IEnumerator GotoRanking(int stage)
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene($"RankingScene{stage:00}");
    }

    // ==== キー加算（グローバル） ====
    public void AddKeyGlobal()
    {
        totalKeys++;
        Debug.Log($"鍵を取得しました（合計: {totalKeys}）");
        PlayerInventory.RaiseKeyCountChanged(totalKeys);
    }

    // ==== ゲームオーバー ====
    public void GameOver()
    {
        SaveCurrentStageNumber();
        if (BGMManager.Instance != null) BGMManager.Instance.SetVolume(0.2f);
        Debug.Log("Game Over!");
        SceneManager.LoadScene("GameOverScene");
    }

    // ==== スコア送信の下準備 ====
    private void EnsureScoreSender()
    {
        if (scoreSender != null) return;

        scoreSender = FindObjectOfType<ScoreSender>();
        if (scoreSender == null)
        {
            if (scoreSenderPrefab != null)
                scoreSender = Instantiate(scoreSenderPrefab);
            else
                scoreSender = new GameObject("ScoreSender(Auto)").AddComponent<ScoreSender>();
        }

        // シーン跨ぎで送信が途切れないように保持
        DontDestroyOnLoad(scoreSender.gameObject);
    }

    private void ApplyStageNumberToScoreSender()
    {
        if (scoreSender == null) return;
        int stage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
        scoreSender.StageNumber = stage;
    }

    private int TryParseStageNumberFromSceneName()
    {
        // 例: "Stage01", "Stage12" → 1..12
        string name = SceneManager.GetActiveScene().name;
        var m = Regex.Match(name, @"Stage\s*0?(\d{1,2})");
        if (m.Success && int.TryParse(m.Groups[1].Value, out int n))
        {
            return Mathf.Clamp(n, 1, 12);
        }
        return 1; // デフォルト
    }

    private int GetCurrentStageNumber()
    {
        // 例: シーン名 "Stage01" ～ "Stage12" から自動抽出
        if (overrideStageNumber > 0) return overrideStageNumber;

        var name = SceneManager.GetActiveScene().name;
        var m = Regex.Match(name, @"Stage\s*0?(\d{1,2})");
        if (m.Success && int.TryParse(m.Groups[1].Value, out int n))
            return Mathf.Clamp(n, 1, 12);
        return 1;
    }
    // 現在のシーンの番号を保存
    public void SaveCurrentStageNumber()
    {
        int stageNumber = GetCurrentStageNumber();
        PlayerPrefs.SetInt("LastStageNumber", stageNumber);
        PlayerPrefs.Save();
        Debug.Log($"ステージ番号 {stageNumber} を保存しました");
    }
    // 最後に保存したシーンの番号を取得
    public int LoadLastStageNumber()
    {
        int stageNumber = PlayerPrefs.GetInt("LastStageNumber", 1);
        Debug.Log($"最後に保存したステージ番号を読み込みました: {stageNumber}");
        return stageNumber;
    }
}
