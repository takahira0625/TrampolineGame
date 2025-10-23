using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KeyUIController : MonoBehaviour
{
    // [Header("ステージ設定")]
    [Header("ステージ設定")] 
    // [Tooltip("このステージで使用する鍵設定ファイル")]
    [Tooltip("このステージで使用する鍵設定ファイル")] 
    public StageKeyConfig currentStageConfig;

    // [Header("UI要素")]
    [Header("UI要素")] 
    // [Tooltip("UIの鍵要素（イメージのImageコンポーネント (最大4つ)")]
    [Tooltip("UIの鍵要素（イメージのImageコンポーネント (最大4つ)")] 
    public List<Image> keyPartImages = new List<Image>();

    // [Header("エフェクト設定")]
    [Header("エフェクト設定")] 
    // [Tooltip("全ての鍵を集めた時に中央に表示されるエフェクト")]
    [Tooltip("全ての鍵を集めた時に中央に表示されるエフェクト")] 
    [SerializeField] private ParticleSystem completeEffectPrefab;
    // [Tooltip("エフェクトの発生位置オフセット")]
    [Tooltip("エフェクトの発生位置オフセット")] 
    [SerializeField] private Vector3 effectOffset = Vector3.zero;
    // [Tooltip("エフェクトを自動で破棄する")]
    [Tooltip("エフェクトを自動で破棄する")] 
    [SerializeField] private bool autoDestroyEffect = true;

    // [Header("エフェクト")]
    [Header("エフェクト")] 
    // [Tooltip("鍵が完成した時に表示されるエフェクトオブジェクト")]
    [Tooltip("鍵が完成した時に表示されるエフェクトオブジェクト")] 
    public GameObject keyCompletionEffect;

    private ParticleSystem keyEffectParticles;

    private int requiredPartsCount = 0;
    private int collectedPartsCount = 0;
    private ParticleSystem currentEffect;

    // 全ての鍵を集めた時のイベント
    public static event System.Action OnAllKeysCollected; 

    // 鍵ピースの取得状況を記録する配列
    private bool[] hasCollectedPart; 

    // 鍵完成フラグ（エフェクトの重複実行防止）
    private bool isComplete = false; 

    // イベント登録
    private void OnEnable() 
    {
        KeyBlock.OnKeyPartCollected += HandleKeyPartCollected;
    }

    private void OnDisable()
    {
        KeyBlock.OnKeyPartCollected -= HandleKeyPartCollected;
    }

    void Start()
    {
        if (currentStageConfig == null)
        {
            // Debug.LogError("KeyUIControllerに StageKeyConfig が設定されていません！");
            Debug.LogError("KeyUIControllerに StageKeyConfig が設定されていません！"); 
            return;
        }

        SetupUISprites();
        ResetUI();
        collectedPartsCount = 0;
    }

    private void SetupUISprites()
    {
        requiredPartsCount = currentStageConfig.keyPartUISprites.Count;

        // 鍵ピース取得状況配列を、必要な数で初期化
        hasCollectedPart = new bool[requiredPartsCount]; 

        // UIスロット（最大4個）をループ
        for (int i = 0; i < keyPartImages.Count; i++) 
        {
            Image uiImage = keyPartImages[i];
            if (uiImage == null) continue;

            if (i < requiredPartsCount)
            {
                uiImage.sprite = currentStageConfig.keyPartUISprites[i];
                uiImage.gameObject.SetActive(true);
            }
            else
            {
                uiImage.gameObject.SetActive(false);
            }
        }
    }

    private void ResetUI()
    {
        foreach (Image img in keyPartImages)
        {
            if (img != null)
            {
                img.enabled = false;
            }
        }

        // 鍵完成エフェクトを非表示
        if (keyCompletionEffect != null) 
        {
            keyCompletionEffect.SetActive(false);
            keyEffectParticles = keyCompletionEffect.GetComponentInChildren<ParticleSystem>();
        }

        // 鍵ピース取得状況と完成フラグをリセット
        isComplete = false; 
        // hasCollectedPartがnullの場合があるため、nullチェックを追加
        if (hasCollectedPart != null) 
        {
            for (int i = 0; i < hasCollectedPart.Length; i++)
            {
                hasCollectedPart[i] = false;
            }
        }
    }

    private void HandleKeyPartCollected(int partIndex)
    {
        // 既に完成しているか、インデックスが範囲外なら無視
        if (isComplete || partIndex < 0 || partIndex >= requiredPartsCount) 
        {
            return;
        }

        // 既に取得済みなら無視 (重複防止)
        if (hasCollectedPart[partIndex]) 
        {
            return;
        }

        // 1. 取得済みにする
        hasCollectedPart[partIndex] = true;

        // 2. UIを表示する
        if (partIndex < keyPartImages.Count && keyPartImages[partIndex] != null)
        {
            keyPartImages[partIndex].enabled = true;
        }

        // 3. 完成したかチェック
        CheckCompletion();
    }

    // 鍵完成チェック用メソッド
    private void CheckCompletion() 
    {
        // 取得状況配列をチェック
        for (int i = 0; i < requiredPartsCount; i++) 
        {
            // 1つでも未取得(false)があれば、まだ完了ではない
            if (!hasCollectedPart[i]) 
            {
                return; // チェック終了
            }
        }

        // --- この行以降に来る = 全て true = 完成！ ---

        isComplete = true; // 完成フラグを立てる (重複防止)

        // エフェクトを表示
        if (keyCompletionEffect != null) 
        {
            Debug.Log("鍵が完成！エフェクトを表示します。"); 
            keyCompletionEffect.SetActive(true);

            // 取得済みのパーティクルシステムコンポーネントを Play()
            if (keyEffectParticles != null) 
            {
                keyEffectParticles.Play();
            }
            // (もし取得できていなかったら、改めて探してPlay)
            else if (keyCompletionEffect.GetComponentInChildren<ParticleSystem>() != null) 
            {
                keyEffectParticles = keyCompletionEffect.GetComponentInChildren<ParticleSystem>();
                keyEffectParticles.Play();
            }
        }
    }

    /// <summary>
    /// 全ての鍵が揃ったかチェック
    /// </summary>
    private void CheckAllKeysCollected() 
    {
        if (collectedPartsCount >= requiredPartsCount)
        {
            // Debug.Log("全ての鍵ピースが集まりました！");
            Debug.Log("全ての鍵ピースが集まりました！"); 

            // エフェクトを生成
            SpawnCompleteEffect();

            // イベント発火
            OnAllKeysCollected?.Invoke();
        }
    }

    /// <summary>
    /// 完成エフェクトを生成
    /// </summary>
    private void SpawnCompleteEffect() 
    {
        if (completeEffectPrefab == null)
        {
            // Debug.LogWarning("完成エフェクトが設定されていません");
            Debug.LogWarning("完成エフェクトが設定されていません"); 
            return;
        }

        // 既存のエフェクトがあれば破棄
        if (currentEffect != null) 
        {
            Destroy(currentEffect.gameObject);
        }

        // このオブジェクトの子としてエフェクトを生成
        currentEffect = Instantiate(completeEffectPrefab, transform); 
        currentEffect.transform.localPosition = effectOffset;
        currentEffect.transform.localRotation = Quaternion.identity;
        currentEffect.transform.localScale = Vector3.one;

        // 再生
        currentEffect.Play(); 

        // Debug.Log("完成エフェクトを生成しました");
        Debug.Log("完成エフェクトを生成しました"); 

        // 自動破棄
        if (autoDestroyEffect) 
        {
            float duration = currentEffect.main.duration + currentEffect.main.startLifetime.constantMax;
            Destroy(currentEffect.gameObject, duration);
        }
    }

    /// <summary>
    /// エフェクトを即座に破棄
    /// </summary>
    public void DestroyEffect() 
    {
        if (currentEffect != null)
        {
            Destroy(currentEffect.gameObject);
            currentEffect = null;
        }
    }

    /// <summary>
    /// 全ての鍵が揃っているか確認
    /// </summary>
    public bool AreAllKeysCollected() 
    {
        return collectedPartsCount >= requiredPartsCount;
    }

    /// <summary>
    /// 進行度合いを取得 (0.0 ~ 1.0)
    /// </summary>
    public float GetCollectionProgress() 
    {
        if (requiredPartsCount == 0) return 0f;
        return (float)collectedPartsCount / requiredPartsCount;
    }

    private void OnDestroy()
    {
        // クリーンアップ
        DestroyEffect(); 
    }
}