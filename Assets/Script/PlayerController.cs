using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool canMove = true;

    [Header("画面外判定")]
    [Tooltip("画面外に出てからゲームオーバーになるまでの猶予秒数")]
    public float outTimeToLose = 1f;
    private float outTimer = 0f;
    private SpriteRenderer sr;

    [Header("速度制限")]
    public float maxSpeed = 30f;
    [SerializeField, Tooltip("スロー時の速度の大きさ")]
    private float slowSpeed = 0.1f;

    public Vector2 savedVelocity = Vector2.zero;
    //Bar関連
    [SerializeField] private RightClickTriggerOn rightClick;
    [Header("SlowZone")]
    private bool isInSlowZone = false;
    [HideInInspector] public bool isActive = false;


    [SerializeField] int afterImageOrderOffset = 1;
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


    [Header("スローゾーン中エフェクト設定")]
    [SerializeField] private GameObject lightningAuraPrefab;  // エフェクトPrefab
    private GameObject currentAuraEffect;

    private AIE2D.DynamicAfterImageEffect2DPlayer afterImagePlayer;
    private bool isHighSpeed = false;
    private ParticleSystem currentSpeedEffect;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        afterImagePlayer = GetComponent<AIE2D.DynamicAfterImageEffect2DPlayer>();
        if (afterImagePlayer == null)
        {
            Debug.LogWarning("DynamicAfterImageEffect2DPlayerが見つかりません");
        }

        // エフェクトを事前生成
        InitializeLightningAuraEffect();
    }

    void Start()
    {
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
            if (GameManager.instance.hasStarted)
            {
                outTimer += Time.deltaTime;
            }
            
            Debug.Log("画面外タイマー: " + outTimer.ToString("F2") + "秒");
            if (outTimer >= outTimeToLose)
            {
                Debug.Log("画面外に出ました");
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                GameManager.instance.UnregisterPlayer(this);
                Destroy(gameObject);
                return;
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
        //ここから
        //押し出してない時、
        if (!rightClick.IsMoving)
        {
            if (isInSlowZone && InputManager.IsLeftClickPressed())
            {
                isActive = true;
                Vector2 velocity = rb.velocity;
                rb.velocity = velocity.normalized * slowSpeed;
            }
            else
            {
                isActive = false;
            }
        }

        UpdateAfterImageColor();
        UpdateHighSpeedEffect();
        UpdateLightningAuraEffect();
    }

    private void InitializeLightningAuraEffect()
    {
        if (lightningAuraPrefab != null)
        {
            currentAuraEffect = Instantiate(lightningAuraPrefab, transform.position, Quaternion.identity);
            currentAuraEffect.transform.SetParent(transform);
            currentAuraEffect.transform.localPosition = Vector3.zero;

            // SortingLayerを設定
            //var renderers = currentAuraEffect.GetComponentsInChildren<ParticleSystemRenderer>();
            //foreach (var r in renderers)
            //{
            //    r.sortingLayerName = "Player";
            //    r.sortingOrder = 3;
            //}

            // 初期状態は非表示
            currentAuraEffect.SetActive(false);
        }
    }

    private void UpdateLightningAuraEffect()
    {
        if (currentAuraEffect != null)
        {
            // isActiveの状態に応じて表示を切り替え
            if (currentAuraEffect.activeSelf != isActive)
            {
                currentAuraEffect.SetActive(isActive);
            }
        }
    }
    void FixedUpdate()
    {
        if (!canMove) return;

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
    /// ブロック側から呼び出され、即座に速度を変更する
    /// （GimmickBlock側でスロー中でないことを確認してから呼ばれる）
    public void RequestSpeedChange(float multiplier)
    {
        if (rb != null)
        {
            // 反射直後の速度 (rb.velocity) に倍率をかける
            rb.velocity *= multiplier;
        }
    }

    private void UpdateAfterImageColor()
    {
        if (afterImagePlayer == null || rb == null) return;

        float currentSpeed = rb.velocity.magnitude;
        bool shouldBeHighSpeed = currentSpeed >= speedThresholdForAfterImage;

        if (shouldBeHighSpeed != isHighSpeed)
        {
            isHighSpeed = shouldBeHighSpeed;
            Color targetColor = isHighSpeed ? highSpeedAfterImageColor : normalAfterImageColor;
            afterImagePlayer.SetColorIfneeded(targetColor);
        }
    }

    private void UpdateHighSpeedEffect()
    {
        if (highSpeedEffectPrefab == null || rb == null) return;

        float currentSpeed = rb.velocity.magnitude;
        bool shouldPlayEffect = currentSpeed >= speedThresholdForAfterImage;

        if (shouldPlayEffect && currentSpeedEffect == null)
        {
            if (attachEffectToPlayer)
            {
                currentSpeedEffect = Instantiate(highSpeedEffectPrefab, transform);
                currentSpeedEffect.transform.localPosition = effectOffset;
            }
            else
            {
                Vector3 effectPosition = transform.position + effectOffset;
                currentSpeedEffect = Instantiate(highSpeedEffectPrefab, effectPosition, Quaternion.identity);
            }

            currentSpeedEffect.Play();
        }
        else if (!shouldPlayEffect && currentSpeedEffect != null)
        {
            currentSpeedEffect.Stop();
            Destroy(currentSpeedEffect.gameObject, currentSpeedEffect.main.duration);
            currentSpeedEffect = null;
        }
        else if (shouldPlayEffect && currentSpeedEffect != null && !attachEffectToPlayer)
        {
            currentSpeedEffect.transform.position = transform.position + effectOffset;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowZone"))
        {
            if (!rightClick.IsMoving)
            {
                // ①スローゾーン内に入った時点での速度を取得
                isInSlowZone = true;
                savedVelocity = rb.velocity;
            }

        }
        if (collision.CompareTag("Bar"))
        {
            Debug.Log("タイマースタート");
            GameManager.instance.StartTimerOnce();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowZone"))
        {
            isInSlowZone = false;
        }

    }
    private void OnDestroy()
    {
        if (currentSpeedEffect != null)
        {
            Destroy(currentSpeedEffect.gameObject);
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    private void OnApplicationQuit()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bar"))
        {
            Debug.Log("タイマースタート");
            GameManager.instance.StartTimerOnce();
        }
    }
}