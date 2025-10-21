using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KeyUIController : MonoBehaviour
{
    [Header("ステージ設定")]
    [Tooltip("このステージで使用する鍵設定ファイル")]
    public StageKeyConfig currentStageConfig;

    [Header("UI参照")]
    [Tooltip("UIの鍵部品イメージのImageコンポーネント (最大4つ)")]
    public List<Image> keyPartImages = new List<Image>();

    [Header("エフェクト設定")]
    [Tooltip("すべての鍵が揃ったときに生成するエフェクト")]
    [SerializeField] private ParticleSystem completeEffectPrefab;
    [Tooltip("エフェクトの生成位置オフセット")]
    [SerializeField] private Vector3 effectOffset = Vector3.zero;
    [Tooltip("エフェクトを自動削除する")]
    [SerializeField] private bool autoDestroyEffect = true;

    private int requiredPartsCount = 0;
    private int collectedPartsCount = 0;
    private ParticleSystem currentEffect;

    // すべての鍵が揃ったときのイベント
    public static event System.Action OnAllKeysCollected;

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
    }

    private void HandleKeyPartCollected(int partIndex)
    {
        if (partIndex >= 0 && partIndex < keyPartImages.Count)
        {
            Image uiImage = keyPartImages[partIndex];
            if (uiImage != null && !uiImage.enabled)
            {
                uiImage.enabled = true;
                collectedPartsCount++;

                Debug.Log($"鍵部品 {partIndex} を収集 ({collectedPartsCount}/{requiredPartsCount})");

                CheckAllKeysCollected();
            }
        }
    }

    /// <summary>
    /// すべての鍵が揃ったかチェック
    /// </summary>
    private void CheckAllKeysCollected()
    {
        if (collectedPartsCount >= requiredPartsCount)
        {
            Debug.Log("すべての鍵部品が揃いました！");
            
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
            Debug.LogWarning("完成エフェクトが設定されていません");
            return;
        }

        // 既存のエフェクトがあれば削除
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

        Debug.Log("鍵完成エフェクトを生成しました");

        // 自動削除
        if (autoDestroyEffect)
        {
            float duration = currentEffect.main.duration + currentEffect.main.startLifetime.constantMax;
            Destroy(currentEffect.gameObject, duration);
        }
    }

    /// <summary>
    /// エフェクトを手動で削除
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
    /// すべての鍵が揃っているか確認
    /// </summary>
    public bool AreAllKeysCollected()
    {
        return collectedPartsCount >= requiredPartsCount;
    }

    /// <summary>
    /// 収集進捗を取得 (0.0 ~ 1.0)
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