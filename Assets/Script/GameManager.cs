using UnityEngine;
using TMPro; // TextMeshProを扱うために必要
using System.Collections.Generic;//PlayerList管理用
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // このクラスの唯一のインスタンスを保持する（シングルトン）
    public static GameManager instance;

    // インスペクターから設定する項目
    public int requiredCoins = 5; // ゴールに必要なコインの数
    public GameObject goalTextObject; // ゴールテキストのUIオブジェクト
    /*public TextMeshProUGUI coinCounterText;*/ // コインカウンターのUIテキスト
    public PlayerController playerController; // プレイヤーのスクリプト

    private int currentCoins = 0; // 現在のコイン取得数
    
    // ==== Doubleブロックによるプレイヤー管理用 ====
    private List<PlayerController> activePlayers = new List<PlayerController>();

    // ==== タイマー関連 ====
    [SerializeField] private bool autoStartTimer = false; // ゲーム開始で自動計測するか
    private bool isTiming = false;
    private bool hasStarted = false;        // ← 追記：一度だけ開始管理
    private float elapsedTime = 0f;
    public float FinalTime { get; private set; } = -1f; // ゴール時の確定タイム
    void Awake()
    {
        // シングルトンの設定
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いで保持
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // ゲーム開始時にUIを初期化
        /*UpdateCoinCounter();*/
        goalTextObject.SetActive(false); // ゴールテキストを非表示に
        if (goalTextObject != null) goalTextObject.SetActive(false);
        if (autoStartTimer) StartTimer();

        //DoubleBlock用：シーン開始時にプレイヤー登録
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        if (playerController != null)
        {
            RegisterPlayer(playerController);
        }
    }

    private void Update()
    {
        if (isTiming)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    // ==== タイマー操作 ====
    public void StartTimer()
    {
        elapsedTime = 0f;
        FinalTime = -1f;
        isTiming = true;
        hasStarted = true;
    }

    // ← 追記：二重起動防止用のラッパー
    public void StartTimerOnce()
    {
        if (!hasStarted) StartTimer();
    }

    public void StopTimer()
    {
        isTiming = false;
        FinalTime = elapsedTime;
    }
    public float ElapsedTime => elapsedTime; // 現在の経過秒を外部から参照
    // もしリトライで再計測したい場合に呼ぶ用（任意）
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
    public void AddCoin()
    {
        currentCoins++;
        /*UpdateCoinCounter();*/

        // ゴール条件を満たしたかチェック
        if (currentCoins >= requiredCoins)
        {
            Goal();
        }
    }

    // コインカウンターUIを更新する関数
    /*void UpdateCoinCounter()
    {
        if (coinCounterText != null)
        {
            coinCounterText.text = "Coin: " + currentCoins + " / " + requiredCoins;
        }
    }*/

    // ==== 以下、Doubleブロックによるプレイヤー管理用 ====
    // プレイヤーを登録（シーン開始時に呼ぶ）
    public void RegisterPlayer(PlayerController player)
    {
        if (!activePlayers.Contains(player))
        {
            activePlayers.Add(player);
        }
    }   
    // プレイヤーが死んだときに呼ぶ
    public void UnregisterPlayer(PlayerController player)
    {
        if (activePlayers.Contains(player))
        {
            activePlayers.Remove(player);
        }

        // 全員が死んだらゲームオーバー
        if (activePlayers.Count == 0)
        {
            Debug.Log("全プレイヤーが死亡しました。GameOver。");
            GameOver();
        }
    }
    public void SpawnAdditionalPlayer(Transform originalPlayer)
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController が未設定のため複製できません");
            return;
        }

        // 元プレイヤーの位置を基準に新しいプレイヤーを生成
        GameObject clone = Instantiate(playerController.gameObject, originalPlayer.position, Quaternion.identity);

        // 少しずらして重ならないように
        Vector3 offset = new Vector3(Random.Range(-1.0f, 1.0f), 0.5f, 0f);
        clone.transform.position += offset;

        // clone も PlayerController を持つので独立して動く
        PlayerController cloneController = clone.GetComponent<PlayerController>();
        if (cloneController != null)
        {
            cloneController.canMove = true;
            RegisterPlayer(cloneController);
        }

        Debug.Log($"プレイヤーを分裂させました！ 現在のプレイヤー数: {activePlayers.Count}");
    }

    // ==== ゴール・ゲームオーバー処理 ====
    // ゴール処理を行う関数
    void Goal()
    {
        StopTimer(); // ← タイマー確定
        SceneManager.LoadScene("ResultScene");

        // プレイヤーの動きを止める
        if (playerController != null)
        {
            playerController.canMove = false;
        }
    }
    public void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene("GameOverScene");
    }
}