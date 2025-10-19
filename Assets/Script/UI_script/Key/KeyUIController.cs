using UnityEngine;
using UnityEngine.UI; // Imageを扱うため
using System.Collections.Generic;

public class KeyUIController : MonoBehaviour
{
    [Header("ステージ設定")]
    [Tooltip("このステージで使用する鍵設定ファイル")]
    public StageKeyConfig currentStageConfig; // ステップ6で設定

    [Header("UI参照")]
    [Tooltip("UIの鍵部品イメージのImageコンポーネント (最大4つ)")]
    public List<Image> keyPartImages = new List<Image>(); // ステップ6で設定

    private int requiredPartsCount = 0;

    // イベント購読
    private void OnEnable()
    {
        // KeyBlockが発する「部品番号」のイベントを購読する
        KeyBlock.OnKeyPartCollected += HandleKeyPartCollected;
    }

    // イベント購読解除（必須）
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

        // 1. UIのスプライト設定
        SetupUISprites();

        // 2. 初期状態ではすべて非表示
        ResetUI();
    }

    // Configに基づき、UI Imageに正しいSpriteを割り当て、不要なスロットを非表示にする
    private void SetupUISprites()
    {
        // Configから必要数を取得
        requiredPartsCount = currentStageConfig.keyPartUISprites.Count;

        // UIスロット（最大4つ）をループ
        for (int i = 0; i < keyPartImages.Count; i++)
        {
            Image uiImage = keyPartImages[i];
            if (uiImage == null) continue;

            // このステージで使うスロットか？ (i < requiredPartsCount)
            if (i < requiredPartsCount)
            {
                // Configに設定されたSpriteを割り当て
                uiImage.sprite = currentStageConfig.keyPartUISprites[i];
                uiImage.gameObject.SetActive(true); // スロット自体は有効
            }
            else
            {
                // このステージでは使わないスロット
                uiImage.gameObject.SetActive(false);
            }
        }
    }

    // UIを初期状態（すべて非表示）にする
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

    // KeyBlock.OnKeyPartCollected から呼ばれる関数
    private void HandleKeyPartCollected(int partIndex)
    {
        // インデックスがUIリストの範囲内かチェック
        if (partIndex >= 0 && partIndex < keyPartImages.Count)
        {
            Image uiImage = keyPartImages[partIndex];
            if (uiImage != null)
            {
                // 該当する番号の部品UIだけを有効にする
                uiImage.enabled = true;
            }
        }
    }
}