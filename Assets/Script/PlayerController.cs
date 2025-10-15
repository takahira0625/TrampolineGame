using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    // 外部から動きを制御するための変数
    public bool canMove = true;

    [Header("画面外判定")]
    [Tooltip("画面外に出てからゲームオーバーになるまでの猶予秒数")]
    public float outTimeToLose = 0.1f;
    private float outTimer = 0f;

    private SpriteRenderer sr;

    [Header("速度制限")]
    [Tooltip("プレイヤー速度の上限 (m/s)。0以下で無制限")]
    public float maxSpeed = 60f;

    [Header("スローモーション設定")]
    [Tooltip("スローモーション時のタイムスケール")]
    [SerializeField, Range(0.1f, 1f)] private float slowMotionTimeScale = 0.3f;
    [Tooltip("Barに触れた後のスローモーション無効化時間（秒）　")]
    [SerializeField] private float slowMotionCooldownTime =0f;
    public bool IsInSlowMotion => Time.timeScale != 1f; // スローモーション中かどうかを取得


    private bool isInSlowZone = false;
    private int slowZoneCount = 0; // 複数のSlowZoneに対応
    private bool isSlowMotionOnCooldown = false; // クールダウン中フラグ
    private float slowMotionCooldownTimer = 0f; // クールダウンタイマー

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 画面外判定
        if (!sr.isVisible)
        {
            outTimer += Time.deltaTime;
            if (outTimer >= outTimeToLose)
            {
                // GameOverに移行する前にTimeScaleをリセット
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                GameManager.instance.GameOver();
                // ゲームマネージャーに死亡を通知して自分を破壊
                GameManager.instance.UnregisterPlayer(this);
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            outTimer = 0f;
        }

        // canMove が false のときは停止
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
        }

        // クールダウンタイマーの更新
        UpdateCooldown();

        // スローモーション制御
        UpdateSlowMotion();
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        // 速度上限クランプ
        if (maxSpeed > 0f)
        {
            float maxSq = maxSpeed * maxSpeed;
            Vector2 v = rb.velocity;
            if (v.sqrMagnitude > maxSq)
            {
                rb.velocity = v.normalized * maxSpeed;
            }
        }
    }

    private void UpdateCooldown()
    {
        if (isSlowMotionOnCooldown)
        {
            slowMotionCooldownTimer -= Time.unscaledDeltaTime; // スローモーション中でも正確にカウント

            if (slowMotionCooldownTimer <= 0f)
            {
                isSlowMotionOnCooldown = false;
                slowMotionCooldownTimer = 0f;
            }
        }
    }

    private void UpdateSlowMotion()
    {
        // クールダウン中はスローモーション無効
        if (isSlowMotionOnCooldown)
        {
            if (Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }
            return;
        }

        // 右クリック押下中 かつ SlowZone内にいる場合
        if (Input.GetMouseButton(0) && isInSlowZone)
        {
            if (Time.timeScale != slowMotionTimeScale)
            {
                Time.timeScale = slowMotionTimeScale;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
        }
        else
        {
            if (Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowZone"))
        {
            slowZoneCount++;
            isInSlowZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowZone"))
        {
            slowZoneCount--;
            if (slowZoneCount <= 0)
            {
                slowZoneCount = 0;
                isInSlowZone = false;
                
                // スローモーションを即座に解除
                if (Time.timeScale != 1f)
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;
                }
            }
        }
    }

    private void OnDestroy()
    {
        // オブジェクト破棄時に必ずTimeScaleをリセット
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    private void OnApplicationQuit()
    {
        // アプリケーション終了時にもリセット
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    // ★ 追加: 外部からクールダウン状態を確認するためのプロパティ
    public bool IsSlowMotionOnCooldown => isSlowMotionOnCooldown;
    public float SlowMotionCooldownRemaining => slowMotionCooldownTimer;
}