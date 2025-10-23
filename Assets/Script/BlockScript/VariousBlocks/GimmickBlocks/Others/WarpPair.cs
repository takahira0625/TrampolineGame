using UnityEngine;
using System.Collections;

/// <summary>
/// WarpPair：指定したペアにのみワープするタイプ
/// </summary>
public class WarpPair : BaseBlock
{
    [Header("ペア設定")]
    [Tooltip("ワープ先となるペアのWarpPairオブジェクト")]
    [SerializeField] private WarpPair pairedWarp;

    [Header("設定")]
    [SerializeField] private float disableDuration = 0.5f; // ワープ元・先を一時無効化
    [SerializeField] private float offsetY = 1.0f;         // プレイヤーを少し上に出す

    private bool isDisabled = false;

    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.WarpPairSprite); // スプライト差し替え可能なら
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDisabled) return;
        if (pairedWarp == null)
        {
            Debug.LogWarning($"{name} にペアが設定されていません。");
            return;
        }

        StartCoroutine(WarpPlayer(other.transform, pairedWarp));
    }

    private IEnumerator WarpPlayer(Transform player, WarpPair destination)
    {
        // ワープ元・先を一時的に無効化（戻り防止）
        isDisabled = true;
        destination.isDisabled = true;

        // プレイヤーをワープ
        player.position = destination.transform.position + Vector3.up * offsetY;

        // 少し待って再び有効化
        yield return new WaitForSeconds(disableDuration);
        isDisabled = false;
        destination.isDisabled = false;
    }

#if UNITY_EDITOR
    // シーン上でペア関係を見やすくするデバッグ描画
    private void OnDrawGizmos()
    {
        if (pairedWarp != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, pairedWarp.transform.position);
            Gizmos.DrawSphere(pairedWarp.transform.position, 0.2f);
        }
    }
#endif
}
