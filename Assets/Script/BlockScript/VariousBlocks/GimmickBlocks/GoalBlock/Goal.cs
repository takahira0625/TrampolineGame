using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // 必要キー数
    
    [Header("ゴール見た目設定")]
    [SerializeField] private Sprite lockedSprite;   // 鍵未取得時の見た目
    [SerializeField] private Sprite unlockedSprite; // 全取得後の見た目
    
    [Header("解放エフェクト設定")]
    [SerializeField] private ParticleSystem unlockEffectPrefab;
    [SerializeField] private Vector3 effectOffset = Vector3.zero;

    private SpriteRenderer spriteRenderer;
    private bool isUnlocked = false;

    protected override void Awake()
    {
        base.Awake();

        // シーン内の KeyBlock の数を取得して requiredKeys に設定
        KeyBlock[] keyBlocks = FindObjectsOfType<KeyBlock>();
        requiredKeys = keyBlocks.Length;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = lockedSprite; // 初期状態はロック見た目
    }
    private void OnEnable()
    {
        // PlayerInventory のイベント購読
        PlayerInventory.OnKeyCountChanged += HandleKeyCountChanged;
    }

    private void OnDisable()
    {
        //イベント購読解除（メモリリーク防止）
        PlayerInventory.OnKeyCountChanged -= HandleKeyCountChanged;
    }

    //鍵の数が変わったときに呼ばれる関数
    private void HandleKeyCountChanged(int currentKeyCount)
    {
        if (!isUnlocked && currentKeyCount >= requiredKeys)
        {
            isUnlocked = true;
            spriteRenderer.sprite = unlockedSprite;
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            SpawnUnlockEffect();
            StartCoroutine(PlayUnlockAnimation());
            Debug.Log("ゴールが開放されました！");
        }
    }

    private IEnumerator PlayUnlockAnimation()
    {
        // --- 時間をゆっくりにする ---
        Time.timeScale = 0.5f;

        Vector3 originalScale = transform.localScale;
        Vector3 smallScale = originalScale * 0.1f;

        // --- 縮む（0.3秒）---
        float duration = 0.6f;
        float return_duration = 0.1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // ← TimeScaleの影響を受けない
            transform.localScale = Vector3.Lerp(originalScale, smallScale, elapsed / duration);
            yield return null;
        }

        // --- 元に戻る（0.3秒）---
        elapsed = 0f;
        while (elapsed < return_duration)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(smallScale, originalScale, elapsed / duration);
            yield return null;
        }

        // --- 時間の流れを元に戻す ---
        Time.timeScale = 1f;
    }

    private void SpawnUnlockEffect()
    {
        if (unlockEffectPrefab == null) return;

        ParticleSystem effect = Instantiate(unlockEffectPrefab, transform);
        effect.transform.localPosition = effectOffset;
        effect.Play();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.TotalKeys >= requiredKeys)
            {
                GameManager.instance.Goal();
                Debug.Log("Goal! " + GameManager.instance.TotalKeys);
            }
            else
            {
                Debug.Log("キーの数が足りません");
            }
        }
    }
}
