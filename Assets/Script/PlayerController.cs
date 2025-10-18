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

    [Header("スローモーション設定")]
    [Tooltip("スローモーション時のタイムスケール")]
    [SerializeField, Range(0.1f, 1f)] private float slowMotionTimeScale = 0.3f;
    [Tooltip("Barに触れた後のスローモーション無効化時間（秒）　")]
    [SerializeField] private float slowMotionCooldownTime = 3f;
    
    private bool isInSlowZone = false;
    private int slowZoneCount = 0; // 複数のSlowZoneに対応
    private bool isSlowMotionOnCooldown = false; // クールダウン中フラグ
    private float slowMotionCooldownTimer = 0f; // クールダウンタイマー

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

    private void UpdateCooldown()
    {
        if (isSlowMotionOnCooldown)
        {
            slowMotionCooldownTimer -= Time.unscaledDeltaTime; // スローモーション中でも正確にカウント

            if (slowMotionCooldownTimer <= 0f)
            {
                isSlowMotionOnCooldown = false;
                slowMotionCooldownTimer = 0f;
                Debug.Log("スローモーションクールダウン終了");
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
        if (Input.GetMouseButton(1) && isInSlowZone)
        {
            if (Time.timeScale != slowMotionTimeScale)
            {
                Time.timeScale = slowMotionTimeScale;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                Debug.Log($"スローモーション開始: TimeScale = {Time.timeScale}");
            }
        }
        else
        {
            if (Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                Debug.Log("スローモーション解除: TimeScale = 1.0");
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
            slowZoneCount++;
            isInSlowZone = true;
            Debug.Log($"SlowZoneに入りました（カウント: {slowZoneCount}）");
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
                Debug.Log("SlowZoneから出ました");
                
                // スローモーションを即座に解除
                if (Time.timeScale != 1f)
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bar"))
        {
            // スローモーションを即座に解除
            if (Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }

            // クールダウン開始
            isSlowMotionOnCooldown = true;
            slowMotionCooldownTimer = slowMotionCooldownTime;
            Debug.Log($"Barに衝突！スローモーション {slowMotionCooldownTime}秒間無効化");
        }
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

    // 外部からクールダウン状態を確認するためのプロパティ
    public bool IsSlowMotionOnCooldown => isSlowMotionOnCooldown;
    public float SlowMotionCooldownRemaining => slowMotionCooldownTimer;

    // 現在の速度を取得するためのプロパティ
    public float CurrentSpeed => rb != null ? rb.velocity.magnitude : 0f;
}