using UnityEngine;
using System.Collections;

/// <summary>
/// WarpPair：指定したペアにのみワープするタイプ
/// </summary>
public class WarpPair : BaseBlock
{
    // ... (既存のフィールドは省略)

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

    [Header("出口専用設定")]
    [Tooltip("出口専用にする場合の色の暗さ (0.0=真っ暗, 1.0=通常の明るさ)")]
    [SerializeField] private float dimValue = 0.5f; // 暗くする割合
    [Tooltip("明るさを1.0に戻す時間")]
    [SerializeField] private float activeLightDuration = 2.0f; // 明るさを戻す時間 (追加)

    private bool isDisabled = false;
    private SpriteRenderer sr; // SpriteRendererの参照を保持 (追加)

    protected override void Awake()
    {
        base.Awake();

        // SpriteRendererの参照をAwakeで取得しておく
        sr = GetComponent<SpriteRenderer>();

        // 個別スプライトが設定されていればそれを優先
        Sprite targetSprite = (customSprite != null) ? customSprite : parameter.WarpPairSprite;
        SetSprite(targetSprite);

        // 初期の明るさ設定を呼び出す
        UpdateLightState(false);
    }

    // --- ?? 追加メソッド：色の設定を分離 ---
    /// <summary>
    /// ワープブロックの明るさを更新する
    /// </summary>
    /// <param name="isActive">true: 1.0の明るさ (活動中)、false: dimValueの明るさ (通常時)</param>
    public void UpdateLightState(bool isActive)
    {
        if (sr == null) return; // SpriteRendererがなければ終了

        // ペアが設定されている場合は常時1.0 (通常の明るさ)
        if (pairedWarp != null)
        {
            sr.color = Color.white;
            return;
        }

        // ペアが設定されていない（出口専用）の場合
        if (isActive)
        {
            // 活動中: 明るさ1.0に戻す
            sr.color = Color.white;
        }
        else
        {
            // 通常時: dimValueの明るさに暗くする
            sr.color = Color.white * dimValue;
        }
    }
    // ------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDisabled) return;
        if (pairedWarp == null)
        {
            // 出口専用ブロックはワープ元にはなれない
            return;
        }

        StartCoroutine(WarpPlayer(other.transform, pairedWarp));
    }

    private IEnumerator WarpPlayer(Transform player, WarpPair destination)
    {
        // ... (省略: ワープ処理前半)
        isDisabled = true;
        destination.isDisabled = true;
        Time.timeScale = 0.7f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(teleportEffectPrefab, player.position, Quaternion.identity);
            Destroy(effect, 1.0f);
        }

        // --- ③ ワープ実行 ---
        player.position = destination.transform.position + Vector3.up * offsetY;

        // --- ?? 追加部分：ワープ先に明るくする処理を指示 ---
        if (destination.pairedWarp == null)
        {
            destination.StartCoroutine(destination.ActivateLightTemporarily());
        }
        // ----------------------------------------------------

        // --- ④ ワープ後エフェクト ---
        if (destination.teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(destination.teleportEffectPrefab, destination.transform.position, Quaternion.identity);
            Destroy(effect, 1.0f);
        }
        // ... (省略: ワープ処理後半)

        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        yield return new WaitForSeconds(WaitTime);

        isDisabled = false;
        destination.isDisabled = false;
    }

    // --- ?? 追加コルーチン：一時的に明るさを戻す ---
    private IEnumerator ActivateLightTemporarily()
    {
        // 明るさを1.0に戻す
        UpdateLightState(true);

        // activeLightDurationだけ待機
        yield return new WaitForSeconds(activeLightDuration);

        // 明るさを元に戻す (dimValue)
        UpdateLightState(false);
    }
    // ----------------------------------------------------

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