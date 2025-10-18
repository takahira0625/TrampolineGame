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

    [Header("残像色変更設定")]
    [Tooltip("残像の色を変更する速度の閾値 (m/s)")]
    [SerializeField] private float speedThresholdForAfterImage = 20f;
    [Tooltip("通常時の残像の色（水色）")]
    [SerializeField] private Color normalAfterImageColor = new Color(0.3f, 0.8f, 1f, 0.6f);
    [Tooltip("高速時の残像の色（赤色）")]
    [SerializeField] private Color highSpeedAfterImageColor = new Color(1f, 0.3f, 0.3f, 0.7f);

    [Header("高速エフェクト設定")]
    [Tooltip("高速時に再生するパーティクルエフェクト")]
    [SerializeField] private ParticleSystem highSpeedEffectPrefab;
    [Tooltip("エフェクトをプレイヤーの子オブジェクトとして配置")]
    [SerializeField] private bool attachEffectToPlayer = true;
    [Tooltip("エフェクトのローカル位置オフセット")]
    [SerializeField] private Vector3 effectOffset = Vector3.zero;

    private AIE2D.DynamicAfterImageEffect2DPlayer afterImagePlayer;
    private bool isHighSpeed = false;
    private ParticleSystem currentSpeedEffect;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        // 残像コンポーネントを取得
        afterImagePlayer = GetComponent<AIE2D.DynamicAfterImageEffect2DPlayer>();
        if (afterImagePlayer == null)
        {
            Debug.LogWarning("DynamicAfterImageEffect2DPlayerが見つかりません");
        }
    }
    void Start()
    {
        // 初期色を設定（通常時は水色）
        if (afterImagePlayer != null)
        {
            afterImagePlayer.SetColorIfneeded(normalAfterImageColor);
        }
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

        // 残像の色を速度に応じて変更
        UpdateAfterImageColor();

        // 高速エフェクトの制御
        UpdateHighSpeedEffect();
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

    /// <summary>
    /// 速度に応じて残像の色を更新する
    /// </summary>
    private void UpdateAfterImageColor()
    {
        if (afterImagePlayer == null || rb == null) return;

        float currentSpeed = rb.velocity.magnitude;

        // 速度が閾値を超えているかチェック
        bool shouldBeHighSpeed = currentSpeed >= speedThresholdForAfterImage;

        // 状態が変わった場合のみ色を更新
        if (shouldBeHighSpeed != isHighSpeed)
        {
            isHighSpeed = shouldBeHighSpeed;
            Color targetColor = isHighSpeed ? highSpeedAfterImageColor : normalAfterImageColor;
            afterImagePlayer.SetColorIfneeded(targetColor);
            Debug.Log($"残像色変更: {(isHighSpeed ? "高速（赤）" : "通常（水色）")} 速度={currentSpeed:F1}m/s");
        }
    }

    /// <summary>
    /// 速度に応じて高速エフェクトを制御する
    /// </summary>
    private void UpdateHighSpeedEffect()
    {
        if (highSpeedEffectPrefab == null || rb == null) return;

        float currentSpeed = rb.velocity.magnitude;
        bool shouldPlayEffect = currentSpeed >= speedThresholdForAfterImage;

        if (shouldPlayEffect && currentSpeedEffect == null)
        {
            // エフェクトを生成して再生
            if (attachEffectToPlayer)
            {
                // プレイヤーの子オブジェクトとして配置
                currentSpeedEffect = Instantiate(highSpeedEffectPrefab, transform);
                currentSpeedEffect.transform.localPosition = effectOffset;
            }
            else
            {
                // ワールド座標に配置
                Vector3 effectPosition = transform.position + effectOffset;
                currentSpeedEffect = Instantiate(highSpeedEffectPrefab, effectPosition, Quaternion.identity);
            }

            currentSpeedEffect.Play();
            Debug.Log("高速エフェクト開始");
        }
        else if (!shouldPlayEffect && currentSpeedEffect != null)
        {
            // エフェクトを停止して削除
            currentSpeedEffect.Stop();
            Destroy(currentSpeedEffect.gameObject, currentSpeedEffect.main.duration);
            currentSpeedEffect = null;
            Debug.Log("高速エフェクト停止");
        }
        else if (shouldPlayEffect && currentSpeedEffect != null && !attachEffectToPlayer)
        {
            // エフェクトがプレイヤーに追従しない場合は位置を更新
            currentSpeedEffect.transform.position = transform.position + effectOffset;
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
        // エフェクトのクリーンアップ
        if (currentSpeedEffect != null)
        {
            Destroy(currentSpeedEffect.gameObject);
        }

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