using UnityEngine;

public class SkyBeans : MonoBehaviour
{
    [Header("設定")]
    public float activationRadius = 1f;                    // クリック反応範囲（半径）
    public GameObject rectanglePrefab;                     // 生成する長方形プレハブ
    public GameObject rangeIndicatorPrefab;                // 範囲表示用の円スプライトプレハブ

    private GameObject currentRectangle = null;            // 現在表示されている長方形（1つだけ）
    private GameObject rangeIndicatorInstance = null;      // 表示中の範囲インジケーター（円）
    private SpriteRenderer spriteRenderer;                 // SkyBeans自身のスプライトレンダラー

    /// <summary>
    /// 初期化処理。SpriteRenderer取得と範囲表示の初期化を行う。
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ShowRangeIndicator();
    }

    /// <summary>
    /// 指定された範囲インジケーターを表示し、親スケールの影響を受けないよう補正してスケーリングする。
    /// </summary>
    private void ShowRangeIndicator()
    {
        if (rangeIndicatorPrefab == null) return;

        rangeIndicatorInstance = Instantiate(
            rangeIndicatorPrefab,
            transform.position,
            Quaternion.identity,
            transform // SkyBeans の子にする
        );

        float diameter = activationRadius * 2f;

        // 親スケールの逆数を掛けて補正（ワールドスケールで正しく見せる）
        Vector3 parentScale = transform.lossyScale;
        Vector3 inverseScale = new Vector3(
            1f / parentScale.x,
            1f / parentScale.y,
            1f / parentScale.z
        );

        // スケールを補正して見た目のサイズを合わせる
        rangeIndicatorInstance.transform.localScale = Vector3.Scale(
            new Vector3(diameter, diameter, 1f),
            inverseScale
        );
    }

    /// <summary>
    /// 指定された座標がこのSkyBeansのクリック範囲内にあるかを判定する。
    /// </summary>
    public bool IsWithinRange(Vector2 clickPosition)
    {
        return Vector2.Distance(transform.position, clickPosition) <= activationRadius;
    }

    /// <summary>
    /// 現在、長方形が表示されているかどうかを返す。
    /// </summary>
    public bool IsRectangleActive()
    {
        return currentRectangle != null;
    }

    /// <summary>
    /// 現在の長方形（currentRectangle）を削除する。
    /// </summary>
    public void DestroyCurrentRectangle()
    {
        if (currentRectangle != null)
        {
            Destroy(currentRectangle);
            currentRectangle = null;
        }
    }

    /// <summary>
    /// クリック座標に向かって長方形を生成する。既にあれば削除して置き換える。
    /// </summary>
    public void GenerateNewRectangle(Vector2 targetPosition)
    {
        DestroyCurrentRectangle(); // 古い長方形を消す

        Vector2 origin = transform.position;
        Vector2 direction = (targetPosition - origin);
        float length = direction.magnitude;
        Vector2 midPoint = origin + direction * 0.5f; // 中心位置を計算

        // 長方形を生成して向き・大きさを設定
        currentRectangle = Instantiate(rectanglePrefab, midPoint, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentRectangle.transform.rotation = Quaternion.Euler(0, 0, angle);
        currentRectangle.transform.localScale = new Vector3(length, 0.2f, 1f);
    }

    /// <summary>
    /// SkyBeans本体の見た目を強調表示する（ハイライト）。
    /// </summary>
    public void SetHighlight(bool highlight)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = highlight ? Color.red : Color.white;
    }

    /// <summary>
    /// Sceneビューでオブジェクトを選択中に、クリック範囲を赤い円で可視化する。
    /// ※ 実行中でなくても表示される。デバッグ用。
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}