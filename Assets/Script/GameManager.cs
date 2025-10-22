using UnityEngine;
using TMPro; // TextMeshProを扱うために必要
using System.Collections.Generic;//PlayerList管理用
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // このクラスの唯一のインスタンスを保持する（シングルトン）
    public static GameManager instance;

    // 複数のボールで鍵の共有を行うためのもの
    private int totalKeys = 0; // 全プレイヤー共通の鍵数
    public int TotalKeys => totalKeys; // 参照用プロパティ

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
    // BGM関連
    public AudioClip gameBGM;
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
        BGMManager.Instance.Play(gameBGM);

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
            Debug.Log($"【Register】プレイヤー登録: {player.name} / 現在のプレイヤー数: {activePlayers.Count}");
        }
        else
        {
            Debug.LogWarning($"【Register】既に登録済み: {player.name}");
        }
    }   
    // プレイヤーが死んだときに呼ぶ
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
            Debug.Log("全プレイヤーが死亡しました。GameOver。");
            GameOver();
        }
    }

    public PlayerController SpawnAdditionalPlayer(Transform originalPlayer, Vector2 velocity)
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController が未設定のため複製できません");
            return null;
        }

        GameObject clone = Instantiate(playerController.gameObject, originalPlayer.position, Quaternion.identity);

        Vector3 offset = new Vector3(Random.Range(-1.0f, 1.0f), 0.5f, 0f);
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

    public void AddKeyGlobal()
    {
        totalKeys++;
        Debug.Log($"鍵を取得しました（合計: {totalKeys}）");

        PlayerInventory.RaiseKeyCountChanged(totalKeys);
    }

    public void Goal()
    {
        StopTimer();
        SceneManager.LoadScene("ResultScene");

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