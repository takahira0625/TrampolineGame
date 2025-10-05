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
            outTimer = 0f; // 画面内に戻ったらリセット
        }

        // canMove が false のときは停止
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
        }

        // ここで入力のみ取得し、物理的な速度適用は FixedUpdate に分離するのが推奨
        // 例）horizontal = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        // 必要ならここで rb.velocity を設定した後に速度制限を適用
        // 例）rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

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
}