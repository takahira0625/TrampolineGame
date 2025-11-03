using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using URandom = UnityEngine.Random;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private List<string> noGameOverScenes = new List<string>
    {
        "NormalBlockScene",
        "KeyGoalBlockScene",
        "SpeedUpBlockScene",
        "SpeedDownBlockScene",
        "SpeedReqBlockScene",
        "BombBlockScene",
        "DoubleBlockScene",
        "WarpBlockScene",
        "KingBombBlockScene"
    };
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
    public bool hasStarted = false;
    private float elapsedTime = 0f;
    public float FinalTime { get; private set; } = -1f;

    // ==== ランキング送信 ====
    [Header("Ranking")]
    [Tooltip("シーン名 Stage01..12 から自動抽出。手動で固定したい場合は 1..12 を指定")]
    [SerializeField, Range(0, 12)] private int overrideStageNumber = 0; // 0 なら自動抽出
    [SerializeField] private ScoreSender scoreSenderPrefab; // 無ければ自動生成用
    private ScoreSender scoreSender; // 実体

    // ==== 個人ランキング用の変数 ====
    private string personalSaveFilePath; // PC固定のセーブファイルパス
    private PersonalRankings currentPersonalRankings; // メモリ上のランキングデータ

    // BGM
    public AudioClip gameBGM;

    [SerializeField] Fade fade; // Fadeコンポーネントへの参照
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

        // ==== 個人ランキングの初期化処理 ====
        InitializePersonalRanking();
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

            string currentSceneName = SceneManager.GetActiveScene().name;

            if (noGameOverScenes.Contains(currentSceneName))
            {
                Debug.Log($"シーン '{currentSceneName}' はゲームオーバー無効リストに含まれているため、待機します。");
            }
            else
            {
                Debug.Log($"シーン '{currentSceneName}' は通常のゲームシーンのため、GameOver処理を実行します。");
                GameOver();
            }
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
        SaveCurrentStageName();
        //FinalTimerの保持
        PlayerPrefs.SetFloat("finaltimer", FinalTime);
        // スコア送信
        int stage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
        Debug.Log($"[GameManager.Goal] GetCurrentStageNumber() が返した値: {GetCurrentStageNumber()}");
        Debug.Log($"[GameManager.Goal] PlayerPrefsに保存する stage 番号: {stage}");
        PlayerPrefs.SetInt("LastClearedStageNumber", stage);
        PlayerPrefs.Save();
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

        // ==== 個人ランキングの保存処理 ====
        string stageIdStr = $"Stage-{stage}";
        bool isNewRecord = AddNewPersonalScore(stageIdStr, FinalTime);
        if (isNewRecord)
        {
            Debug.Log($"[PersonalRanking] ステージ {stageIdStr} のトップ3にランクインしました！");
        }

        if (LoadLastStageName() == "KeyGoalBlockScene")
        {
            fade.FadeIn(6f, () => {
                SceneManager.LoadScene("BlockCatalogScene01");
            });
            return;
        }
        // ランキングへ遷移（少し待つ場合は WaitForSeconds を延ばす）
        fade.FadeIn(0.5f, () => {
            StartCoroutine(GotoRanking(stage));
        });
        
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
        SaveCurrentStageName();

        if (BGMManager.Instance != null) BGMManager.Instance.SetVolume(0.2f);

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
    // 現在のシーンの名前を保存
    public void SaveCurrentStageName()
    {
        string stageName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastStageName", stageName);
        PlayerPrefs.Save();
        Debug.Log($"ステージ名 {stageName} を保存しました");
    }
    // 最後に保存したシーンの名前を取得
    public string LoadLastStageName()
    {
        string stageName = PlayerPrefs.GetString("LastStageName", "Stage01");
        Debug.Log($"最後に保存したステージ名を読み込みました: {stageName}");
        return stageName;
    }

    // ====== 以下は個人ランキングに関する部分 ======
    // 個人ランキングのファイルパスを決定し、データを読み込む
    private void InitializePersonalRanking()
    {
        // 常にPC固定のファイル名にする
        personalSaveFilePath = Path.Combine(Application.persistentDataPath, "personal_rankings.json");

        Debug.Log($"[PersonalRanking] 個人ランキングの保存先: {personalSaveFilePath}");

        // ファイルからデータを読み込む
        LoadPersonalRankings();
    }

    // ファイルから個人ランキングを読み込む
    private void LoadPersonalRankings()
    {
        if (!File.Exists(personalSaveFilePath))
        {
            currentPersonalRankings = new PersonalRankings();
            return;
        }
        try
        {
            string json = File.ReadAllText(personalSaveFilePath);
            currentPersonalRankings = JsonUtility.FromJson<PersonalRankings>(json);

            if (currentPersonalRankings == null)
            {
                currentPersonalRankings = new PersonalRankings();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[PersonalRanking] 読み込み失敗: {e.Message}");
            currentPersonalRankings = new PersonalRankings();
        }
    }

    // メモリ上の現在の個人ランキングをファイルに保存する
    private void SavePersonalRankings()
    {
        if (currentPersonalRankings == null) return;
        try
        {
            string json = JsonUtility.ToJson(currentPersonalRankings, true);
            File.WriteAllText(personalSaveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[PersonalRanking] 保存失敗: {e.Message}");
        }
    }

    // 指定したステージの個人ランキングリスト（上位3件）を取得する
    // (ランキング表示UIなどで使用)
    public List<ScoreEntry> GetPersonalRankingsForStage(string stageId)
    {
        if (currentPersonalRankings == null)
        {
            Debug.LogWarning("[PersonalRanking] データがまだロードされていません。");
            return new List<ScoreEntry>();
        }

        StageRanking stageRank = currentPersonalRankings.allStageRankingsList
            .FirstOrDefault(r => r.stageId == stageId);

        if (stageRank != null)
        {
            return stageRank.scores;
        }

        return new List<ScoreEntry>(); // 該当ステージの記録なし
    }

    // 個人ランキングに新しいスコアを追加する (Goal() から呼ばれる)
    private bool AddNewPersonalScore(string stageId, float clearTime)
    {
        if (currentPersonalRankings == null)
        {
            Debug.LogError("[PersonalRanking] ランキングデータが初期化されていません。");
            return false;
        }

        StageRanking stageRank = currentPersonalRankings.allStageRankingsList
            .FirstOrDefault(r => r.stageId == stageId);

        if (stageRank == null)
        {
            stageRank = new StageRanking(stageId);
            currentPersonalRankings.allStageRankingsList.Add(stageRank);
        }

        List<ScoreEntry> stageScores = stageRank.scores;

        // トップ3に入るかチェック
        if (stageScores.Count < 3 || clearTime < stageScores.Last().time)
        {
            ScoreEntry newEntry = new ScoreEntry
            {
                time = clearTime,
                replayFileName = null
            };
            stageScores.Add(newEntry);
            stageScores.Sort((a, b) => a.time.CompareTo(b.time)); // 昇順ソート

            if (stageScores.Count > 3)
            {
                stageScores.RemoveRange(3, stageScores.Count - 3); // 4位以下を削除
            }

            SavePersonalRankings(); // ファイルに保存
            return true; // ランクインした
        }

        return false; // ランクインしなかった
    }
}
