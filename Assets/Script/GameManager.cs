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

    // 鍵/コイン関連（集計）
    private int totalKeys = 0;
    public int TotalKeys => totalKeys;

    public int requiredCoins = 5;
    public GameObject goalTextObject;
    public PlayerController playerController;

    private int currentCoins = 0;
    private List<PlayerController> activePlayers = new List<PlayerController>();

    // ==== タイマー関連 ====
    [Header("Timer")] // ヘッダーを Timer に変更
    [SerializeField] private bool autoStartTimer = false;
    private bool isTiming = false;
    private bool hasStarted = false;
    private float elapsedTime = 0f;
    public float FinalTime { get; private set; } = -1f;

    // ==== スコアランキング関連 ====
    [Header("Ranking")]
    [Tooltip("シーン名が Stage01..12 でない場合に上書き指定した場合に 1..12 を指定")]
    [SerializeField, Range(0, 12)] private int overrideStageNumber = 0; // 0 なら無視 : 0 なら無視
    [SerializeField] private ScoreSender scoreSenderPrefab; // スコア送信処理用
    private ScoreSender scoreSender; // 管理用（別シーン or DontDestroy） : 管理用（別シーン or DontDestroy）

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

    // コインが取得された時に呼ばれる関数
    //public void AddCoin() : コインが取得された時に呼ばれる関数
    //{
    //    currentCoins++;
    //    /*UpdateCoinCounter();*/

    //    // ゴール条件を満たしたかチェック
    //    if (currentCoins >= requiredCoins) : ゴール条件を満たしたかチェック
    //    {
    //        Goal();
    //    }
    //}

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

            Debug.Log($"【Unregister】プレイヤー削除: {player.name} / 残りプレイヤー数: {activePlayers.Count}");
        }
        else
        {
            Debug.LogWarning($"【Unregister】リストに存在しないプレイヤー: {player.name}");
        }

        if (activePlayers.Count == 0)
        {
            Debug.Log("全プレイヤーが全滅しました。GameOver。");
            GameOver();
        }
        if (activePlayers.Count == 0) GameOver();
    }

    public PlayerController SpawnAdditionalPlayer(Transform originalPlayer, Vector2 velocity)
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController が未設定のため生成できません");
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


    public void Goal()
    {
        StopTimer();
        if (playerController != null) playerController.canMove = false;
        if (goalTextObject != null) goalTextObject.SetActive(true);
        StartCoroutine(SubmitAndGotoRanking());
    }

    private System.Collections.IEnumerator SubmitAndGotoRanking()
    {
        if (scoreSender != null && FinalTime >= 0f)
        {
            int stage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
            scoreSender.StageNumber = stage;

            scoreSender.SendClearTimeSeconds(FinalTime);

            yield return new WaitForSeconds(0.5f);
        }

        int targetStage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
        string rankingScene = $"RankingScene{targetStage:00}";
        UnityEngine.SceneManagement.SceneManager.LoadScene(rankingScene);
    }


    public void AddKeyGlobal()
    {
        totalKeys++;
        Debug.Log($"鍵を取得しました（総計: {totalKeys}）");

        PlayerInventory.RaiseKeyCountChanged(totalKeys);
    }

    public void GameOver()
    {
        BGMManager.Instance.SetVolume(0.2f);
        Debug.Log("Game Over!");
        SceneManager.LoadScene("GameOverScene");
    }

    // ==== スコア送信の準備 ====
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

        // シーン移動後も破棄されないように保持
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
        // 例: "Stage01", "Stage12" から 1..12
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
        // 例: シーン名が "Stage01" 〜 "Stage12" の場合に抽出
        if (overrideStageNumber > 0) return overrideStageNumber;

        var name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var m = System.Text.RegularExpressions.Regex.Match(name, @"Stage\s*0?(\d{1,2})");
        if (m.Success && int.TryParse(m.Groups[1].Value, out int n))
            return Mathf.Clamp(n, 1, 12);
        return 1;
    }
}