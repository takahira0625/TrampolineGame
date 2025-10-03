using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    // 外部から動きを制御するための変数
    public bool canMove = true;

    public float outTimeToLose = 0.01f; // 画面外に出てからゲームオーバーまでの猶予
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
            outTimer = 0f;
        }

        if (!canMove)
        {
            rb.velocity = Vector2.zero;
        }
        // 移動入力処理をここに書く場合は FixedUpdate ではなくこちらで入力のみ取得し、
        // 速度適用は FixedUpdate に回す設計が推奨
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        // ここで移動処理を行う場合は rb.velocity を設定した後に速度制限
        // 例（未実装）: rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

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