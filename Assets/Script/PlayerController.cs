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
    private float slowSpeed = 5f;
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

        if (isInSlowZone && Input.GetMouseButton(0) )
        {
            isActive = true;
            if (!rightClick.IsMoving)
            {

                Vector2 velocity = rb.velocity;
                rb.velocity = velocity.normalized * slowSpeed;

            }
        }
        else
        {
            isActive = false;
        }

        UpdateAfterImageColor();
        UpdateHighSpeedEffect();
        UpdateLightningAuraEffect();

    }

    private void UpdateLightningAuraEffect()
    {
        if (isActive)
        {
            // まだエフェクトが存在しないなら生成
            if (currentAuraEffect == null && lightningAuraPrefab != null)
            {
                // 親を指定して生成（ズレ防止）
                currentAuraEffect = Instantiate(lightningAuraPrefab, transform);
                currentAuraEffect.transform.localPosition = Vector3.zero;
                currentAuraEffect.transform.localRotation = Quaternion.identity;
                currentAuraEffect.transform.localScale = Vector3.one;

                // 描画設定
                var renderers = currentAuraEffect.GetComponentsInChildren<ParticleSystemRenderer>();
                foreach (var r in renderers)
                {
                    r.sortingLayerName = "Player";
                    r.sortingOrder = 10;
                }
            }

            // アクティブにする
            if (currentAuraEffect != null && !currentAuraEffect.activeSelf)
            {
                currentAuraEffect.SetActive(true);
            }
        }
        else
        {
            // 🔻 isActiveがfalseなら非表示にするだけ（削除しない）
            if (currentAuraEffect != null && currentAuraEffect.activeSelf)
            {
                currentAuraEffect.SetActive(false);
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

        }
        if (collision.CompareTag("Bar"))
        {
            GameManager.instance.StartTimerOnce();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SlowZone"))
        {
            isInSlowZone = false;

            //SlowZoneエフェクトが存在すれば削除
            if (currentAuraEffect != null)
            {
                Destroy(currentAuraEffect);
                currentAuraEffect = null;
            }

            // 念のため isActive もリセット
            isActive = false;
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
            GameManager.instance.StartTimerOnce();
        }
    }
}