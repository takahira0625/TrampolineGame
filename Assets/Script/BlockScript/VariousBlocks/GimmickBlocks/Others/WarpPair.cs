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

    [SerializeField] private float WaitTime = 0.5f;

    [Header("エフェクト設定")]
    [SerializeField] private GameObject teleportEffectPrefab;

    [Header("スプライト設定")]
    [Tooltip("インスペクターで個別に見た目を指定できます。指定しなければ parameter.WarpPairSprite を使用します。")]
    [SerializeField] private Sprite customSprite; //個別スプライト設定

    private bool isDisabled = false;

    protected override void Awake()
    {
        base.Awake();
        // 個別スプライトが設定されていればそれを優先
        if (customSprite != null)
        {
            SetSprite(customSprite);
        }
        else
        {
            SetSprite(parameter.WarpPairSprite);
        }
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
        isDisabled = true;
        destination.isDisabled = true;

        // --- ① スローモーション開始 ---
        Time.timeScale = 0.7f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // --- ② ワープ前エフェクト ---
        if (teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(teleportEffectPrefab, player.position, Quaternion.identity);
            Destroy(effect, 1.0f);
        }

        // --- ③ ワープ実行 ---
        player.position = destination.transform.position + Vector3.up * offsetY;

        // --- ④ ワープ後エフェクト ---
        if (destination.teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(destination.teleportEffectPrefab, destination.transform.position, Quaternion.identity);
            Destroy(effect, 1.0f);
        }

        // 少し待つ（リアルタイムで）
        yield return new WaitForSecondsRealtime(0.1f);

        // --- ⑤ スローモーション解除 ---
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        yield return new WaitForSeconds(WaitTime);

        isDisabled = false;
        destination.isDisabled = false;
    }

#if UNITY_EDITOR
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
