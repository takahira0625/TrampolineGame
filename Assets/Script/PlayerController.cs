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
    public float maxSpeed = 40f;

    [Header("スローモーション")]
    [Tooltip("スローモーション時の時間スケール（0.3 = 30%の速度）")]
    [SerializeField, Range(0.1f, 1f)] private float slowMotionTimeScale = 0.3f;
    private bool isSlowMotion = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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
                GameManager.instance.GameOver();
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

        // ★ 修正: 左クリックでスローモーション
        HandleSlowMotionInput();
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

    // ★ 修正: スローモーション入力処理（左クリックに変更）
    private void HandleSlowMotionInput()
    {
        // 左クリック押下（0 = 左クリック）
        if (Input.GetMouseButtonDown(0) && !isSlowMotion)
        {
            ActivateSlowMotion();
        }
        // 左クリック離す
        else if (Input.GetMouseButtonUp(0) && isSlowMotion)
        {
            DeactivateSlowMotion();
        }
    }

    // スローモーション開始
    private void ActivateSlowMotion()
    {
        isSlowMotion = true;
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // 物理演算も調整
        Debug.Log($"スローモーション開始: timeScale = {Time.timeScale}");
    }

    // スローモーション解除
    private void DeactivateSlowMotion()
    {
        isSlowMotion = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        Debug.Log("スローモーション解除");
    }

    // オブジェクト破棄時に時間スケールをリセット
    private void OnDestroy()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}