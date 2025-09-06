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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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
        // canMoveがtrueの時だけ移動処理を行う
        if (canMove)
        {
           
        }
        else
        {
            // 動きを止める
            rb.velocity = Vector2.zero;
        }
    }
}