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
    public float maxSpeed = 30f;
    [SerializeField, Tooltip("スロー時の速度の大きさ")]
    private float slowSpeed = 5f;
    public Vector2 savedVelocity = Vector2.zero;

    [SerializeField] private RightClickTriggerOn rightClick;

    [Header("SlowZone")]
    private bool isInSlowZone = false;
    [HideInInspector] public bool isActive = false;

    //ここまで
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

        //②スローゾーン内に入っていて,左クリックが押されていたら
        //さらにRightClick.csのIsMovingがFalseの場合
        //一定の速度（ゆっくり）になり、エフェクトが付与される。
        if (isInSlowZone && Input.GetMouseButton(0) && !rightClick.IsMoving)
        {
            Vector2 velocity = rb.velocity;
            rb.velocity = velocity.normalized * slowSpeed;
            //Active化
            isActive = true;
        }
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowZone"))
        {
            //①スローゾーン内に入った時点での速度を取得
            isInSlowZone = true;
            savedVelocity = rb.velocity;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isInSlowZone = false;
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
}