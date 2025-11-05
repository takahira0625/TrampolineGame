using UnityEngine;
using UnityEngine.UI; // Image のために必要

public class StageButtonBadge : MonoBehaviour
{
    [Header("ステージ設定")]
    [Tooltip("このボタンのステージ番号 (例: 1, 2, ... 12)")]
    [SerializeField] private int stageNumber;

    [Header("UI参照")]
    [Tooltip("バッジを表示する Image コンポーネント")]
    [SerializeField] private Image badgeImage;

    [Header("バッジ画像")]
    [Tooltip("Goldバッジのスプライト")]
    [SerializeField] private Sprite goldBadgeSprite;

    [Tooltip("Silverバッジのスプライト")]
    [SerializeField] private Sprite silverBadgeSprite;

    [Tooltip("Bronzeバッジのスプライト")]
    [SerializeField] private Sprite bronzeBadgeSprite;


    void Start()
    {
        if (badgeImage == null)
        {
            Debug.LogError($"ステージ {stageNumber} のボタンに badgeImage が設定されていません", this);
            return;
        }

        if (StageSelectManager.Instance == null)
        {
            Debug.LogError("StageSelectManager がシーンに存在しません");
            badgeImage.enabled = false;
            return;
        }

        // 1. Managerに自分のステージ番号のバッジタイプを問い合わせる
        BadgeType type = StageSelectManager.Instance.GetBadgeForStage(stageNumber);

        // 2. 問い合わせ結果に応じて、Image の内容を更新
        switch (type)
        {
            case BadgeType.Gold:
                badgeImage.sprite = goldBadgeSprite;
                badgeImage.enabled = true; // 表示する
                break;

            case BadgeType.Silver:
                badgeImage.sprite = silverBadgeSprite;
                badgeImage.enabled = true; // 表示する
                break;

            case BadgeType.Bronze:
                badgeImage.sprite = bronzeBadgeSprite;
                badgeImage.enabled = true; // 表示する
                break;

            case BadgeType.None:
            default:
                badgeImage.enabled = false; // 表示しない
                break;
        }
    }
}