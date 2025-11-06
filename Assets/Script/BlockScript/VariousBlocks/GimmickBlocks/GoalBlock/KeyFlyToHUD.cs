using UnityEngine;
using System.Collections;

// 鍵オブジェクトにアタッチし、飛翔と消滅を制御する
public class KeyFlyToHUD : MonoBehaviour
{
    [Header("飛翔設定")]
    [SerializeField] private float flightDuration = 0.5f; // 飛翔にかかる時間
    [SerializeField] private float curveHeight = 5f;       // 曲線を描くための高さ（調整可能）

    private Vector3 startPosition;
    private Vector3 endPosition;

    // 鍵の取得処理と連動させるための公開メソッド
    public void StartFlight(Vector3 targetPosition)
    {
        startPosition = transform.position;
        endPosition = targetPosition;
        StartCoroutine(FlyToTarget());
    }

    private IEnumerator FlyToTarget()
    {
        Debug.Log("FlyToTargetコルーチン開始。");
        float elapsed = 0f;

        // 飛翔開始前に、コライダーとリジッドボディを無効化する
        // KeyオブジェクトにRigidbody2Dが付いている場合
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) Destroy(rb); // 物理演算を停止

        while (elapsed < flightDuration)
        {
            float t = elapsed / flightDuration;

            //放物線移動の計算
            // 1. 線形補間（直線移動）
            Vector3 linearPos = Vector3.Lerp(startPosition, endPosition, t);

            // 2. 曲線補間（t=0.5で最大高さになる）
            float curve = Mathf.Sin(t * Mathf.PI) * curveHeight;

            // 3. 最終位置を計算 (カーブはY軸方向のみに適用)
            Vector3 finalPos = linearPos + Vector3.up * curve;

            transform.position = finalPos;

            //スケールを徐々に小さくする
            float scale = Mathf.Lerp(30f, 10f, t); // 1.0から0.1まで縮小
            transform.localScale = Vector3.one * scale;

            Debug.Log($"経過時間: {elapsed:F2}, 現在位置: {transform.position}");
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 最終位置に配置し、オブジェクトを破壊
        transform.position = endPosition;
        transform.localScale = Vector3.zero;

        Destroy(gameObject);
        Debug.Log("飛翔完了、鍵オブジェクトを破棄します。");
    }
}