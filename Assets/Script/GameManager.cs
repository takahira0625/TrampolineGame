using System;
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

    // 鍵・コイン関連（既存）
    private int totalKeys = 0;
    public int TotalKeys => totalKeys;

    public int requiredCoins = 5;
    public GameObject goalTextObject;
    public PlayerController playerController;

    private int currentCoins = 0;
    private List<PlayerController> activePlayers = new List<PlayerController>();

    // ==== タイマー関連 ====
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
    private ScoreSender scoreSender; // 実体（同シーン or DontDestroy）

    // BGM
    public AudioClip gameBGM;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }
    }

    void Start()
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

    void Update()
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

    // ==== コイン・鍵 ====
    public void AddCoin()
    {
        currentCoins++;
        if (currentCoins >= requiredCoins) { Goal(); }
    }

    public void AddKeyGlobal()
    {
        totalKeys++;
        PlayerInventory.RaiseKeyCountChanged(totalKeys);
    }

    // ==== プレイヤー管理 ====
    public void RegisterPlayer(PlayerController player)
    {
        if (!activePlayers.Contains(player))
        {
            activePlayers.Add(player);
            Debug.Log($"【Register】{player.name} / cnt={activePlayers.Count}");
        }
    }
    public void UnregisterPlayer(PlayerController player)
    {
        if (activePlayers.Contains(player))
        {
            activePlayers.Remove(player);
            Debug.Log($"【Unregister】{player.name} / cnt={activePlayers.Count}");
        }
        if (activePlayers.Count == 0) GameOver();
    }
    public void SpawnAdditionalPlayer(Transform originalPlayer)
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController 未設定で複製不可");
            return;
        }
        GameObject clone = Instantiate(playerController.gameObject, originalPlayer.position, Quaternion.identity);
        Vector3 offset = new Vector3(URandom.Range(-1.0f, 1.0f), 0.5f, 0f);
        clone.transform.position += offset;

        var cloneController = clone.GetComponent<PlayerController>();
        if (cloneController != null)
        {
            cloneController.canMove = true;
            RegisterPlayer(cloneController);
        }
    }

    // ==== ゴール・ゲームオーバー ====
    public void Goal()
    {
        StopTimer(); // タイム確定
        if (playerController != null) playerController.canMove = false;
        if (goalTextObject != null) goalTextObject.SetActive(true);

        // 送信 → ランキングシーンへ直行
        StartCoroutine(SubmitAndGotoRanking());
    }

    private System.Collections.IEnumerator SubmitAndGotoRanking()
    {
        // 送信（できるだけ完了を待つ）
        if (scoreSender != null && FinalTime >= 0f)
        {
            // シーン番号をScoreSenderへ（1..12）
            int stage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
            scoreSender.StageNumber = stage;

            // 送信開始（ScoreSender内でUnityWebRequestのコルーチンが走る）
            scoreSender.SendClearTimeSeconds(FinalTime);

            // シーン遷移で通信が切られにくいよう、短時間だけ待つ
            yield return new WaitForSeconds(0.5f); // 0.3～1.0秒で調整可
        }

        // ★ 遷移先を ResultScene → RankingsSceneXX に変更
        int targetStage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
        string rankingScene = $"RankingScene{targetStage:00}";
        UnityEngine.SceneManagement.SceneManager.LoadScene(rankingScene);
    }

    public void GameOver()
    {
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

        var name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var m = System.Text.RegularExpressions.Regex.Match(name, @"Stage\s*0?(\d{1,2})");
        if (m.Success && int.TryParse(m.Groups[1].Value, out int n))
            return Mathf.Clamp(n, 1, 12);
        return 1;
    }

}
