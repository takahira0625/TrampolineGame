using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool canMove = true;

    [Header("画面外判定")]
    [Tooltip("画面外に出てからゲームオーバーになるまでの猶予秒数")]
    public float outTimeToLose = 0.5f;
    private float outTimer = 0f;
    private SpriteRenderer sr;

    [Header("速度制限")]
    public float maxSpeed = 30f;
    [SerializeField, Tooltip("スロー時の速度の大きさ")]
    private float slowSpeed = 5f;
    public Vector2 savedVelocity = Vector2.zero;

    //矢印表示用
    private ArrowDirection arrow;
    private float entrySpeedRatio = 0f;

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

    private AIE2D.DynamicAfterImageEffect2DPlayer afterImagePlayer;
    private bool isHighSpeed = false;
    private ParticleSystem currentSpeedEffect;
    
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        arrow = GetComponentInChildren<ArrowDirection>(true);
        if (arrow != null)
            arrow.gameObject.SetActive(false);

        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        afterImagePlayer = GetComponent<AIE2D.DynamicAfterImageEffect2DPlayer>();
        if (afterImagePlayer == null)
        {
            Debug.LogWarning("DynamicAfterImageEffect2DPlayerが見つかりません");
        }
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
            outTimer += Time.deltaTime;
            if (outTimer >= outTimeToLose)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                GameManager.instance.GameOver();
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

        if (isInSlowZone && Input.GetMouseButton(0) && !rightClick.IsMoving)
        {
            Vector2 velocity = rb.velocity;
            rb.velocity = velocity.normalized * slowSpeed;
            isActive = true;
            DisplayArrow();

        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
        // 残像の色を速度に応じて変更

        UpdateAfterImageColor();
        UpdateHighSpeedEffect();

    }
    void DisplayArrow()
    {
        if (arrow == null) return;

        arrow.gameObject.SetActive(true);

        Vector2 barPos = rightClick.transform.position;
        Vector2 ballPos = transform.position;
        float ratio = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);

        arrow.UpdateArrow(barPos, ballPos, ratio);
        
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
            // ①スローゾーン内に入った時点での速度を取得
            isInSlowZone = true;
            savedVelocity = rb.velocity;

            // ②最大速度に対する割合（0～1）
            float currentSpeed = rb.velocity.magnitude;
            entrySpeedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInSlowZone = false;
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
}