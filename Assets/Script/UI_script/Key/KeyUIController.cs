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

    [Header("エフェクト")]
    [Tooltip("鍵完成時に表示するエフェクトオブジェクト")]
    public GameObject keyCompletionEffect;

    private ParticleSystem keyEffectParticles;

    private int requiredPartsCount = 0;

    // 追加：取得状況を追跡する配列
    private bool[] hasCollectedPart;

    // 追加：完了フラグ（エフェクトの重複実行防止）
    private bool isComplete = false;

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

        // 追加：取得状況配列を、必要な部品数で初期化
        hasCollectedPart = new bool[requiredPartsCount];

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

        // 追加：エフェクトを非表示に
        if (keyCompletionEffect != null)
        {
            keyCompletionEffect.SetActive(false);
            keyEffectParticles = keyCompletionEffect.GetComponentInChildren<ParticleSystem>();
        }

        // 追加：取得状況と完了フラグをリセット
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

    // KeyBlock.OnKeyPartCollected から呼ばれる関数
    private void HandleKeyPartCollected(int partIndex)
    {
        // 既に完了しているか、インデックスが範囲外なら何もしない
        if (isComplete || partIndex < 0 || partIndex >= requiredPartsCount)
        {
            return;
        }

        // 既に取得済みなら何もしない (重複防止)
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

        // 3. 完了したかチェックする
        CheckCompletion();
    }

    // 追加：完了チェック用メソッド
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

        // --- この行に来た = すべて true = 完了！ ---

        isComplete = true; // 完了フラグを立てる (重複実行防止)

        // エフェクトを表示
        if (keyCompletionEffect != null)
        {
            Debug.Log("鍵が完成！エフェクトを表示します。");
            keyCompletionEffect.SetActive(true);

            // 取得済みのパーティクルシステムコンポーネントで Play() を呼ぶ
            if (keyEffectParticles != null)
            {
                keyEffectParticles.Play();
            }
            // (もし取得できていなければ、念のため再度探してPlay)
            else if (keyCompletionEffect.GetComponentInChildren<ParticleSystem>() != null)
            {
                keyEffectParticles = keyCompletionEffect.GetComponentInChildren<ParticleSystem>();
                keyEffectParticles.Play();
            }
        }
    }
}