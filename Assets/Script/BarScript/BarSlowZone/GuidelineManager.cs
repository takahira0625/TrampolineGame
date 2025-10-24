using System.Collections.Generic;
using UnityEngine;

public class GuidelineManager : MonoBehaviour
{
    [Header("Raycast設定")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float maxRayDistance = 50f;

    [Header("配置設定")]
    [SerializeField] private GameObject guidelinePrefab;
    [SerializeField] private float placementInterval = 1f;
    [SerializeField] private int guidelineCount = 100;

    [Header("デバッグ")]
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private Color firstRayColor = Color.red;
    [SerializeField] private Color secondRayColor = Color.green;
    [SerializeField] private Color missColor = Color.blue;

    private List<GameObject> guidelines = new();

    void Awake()
    {
        InitializeGuidelines();
    }

    /// <summary>
    /// ガイドラインをまとめて生成
    /// </summary>
    void InitializeGuidelines()
    {
        for (int i = 0; i < guidelineCount; i++)
        {
            GameObject obj = Instantiate(guidelinePrefab, transform);
            obj.SetActive(false);
            obj.name = $"Guideline_{i + 1}";
            guidelines.Add(obj);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
            CastRay();
        else         {
            // マウスボタンを離したら全てのガイドラインを非表示にする
            foreach (var g in guidelines)
            {
                g.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 1回目のRayを発射し、ヒットしたら2回目を呼び出す
    /// </summary>
    private void CastRay()
    {
        Vector2 rayOrigin = (Vector2)transform.position + (Vector2)transform.up * 9f;
        Vector2 rayDirection = transform.up;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, maxRayDistance, targetLayer);

        Vector2 endPoint = hit.collider ? hit.point : rayOrigin + rayDirection * maxRayDistance;

        // ガイドラインをRay経路に配置
        PlaceGuidelines(rayOrigin - rayDirection * 5, endPoint, rayDirection);

        if (hit.collider != null)
        {
            if (showDebugRay)
                Debug.DrawLine(rayOrigin, hit.point, firstRayColor, 1f);

            Debug.Log($"[1st Ray] Hit: {hit.collider.name}, distance: {hit.distance}");

            // 反射方向を計算
            Vector2 reflectDir = Vector2.Reflect(rayDirection, hit.normal);
            Vector2 secondOrigin = hit.point + reflectDir * 0.01f;

            SecondCastRay(secondOrigin, reflectDir);
        }
        else
        {
            if (showDebugRay)
                Debug.DrawRay(rayOrigin, rayDirection * maxRayDistance, missColor, 1f);
        }
    }

    /// <summary>
    /// 2回目のRay（反射Ray）
    /// </summary>
    private void SecondCastRay(Vector2 origin, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxRayDistance, targetLayer);
        Vector2 endPoint = hit.collider ? hit.point : origin + direction * maxRayDistance;

        // 2回目もガイドラインを追加で並べる
        PlaceGuidelines(origin, endPoint, direction, startIndexOffset: Mathf.FloorToInt(Vector2.Distance(transform.position, origin) / placementInterval));

        if (hit.collider != null)
        {
            if (showDebugRay)
                Debug.DrawLine(origin, hit.point, secondRayColor, 1f);
            Debug.Log($"[2nd Ray] Hit: {hit.collider.name}, distance: {hit.distance}");
        }
        else if (showDebugRay)
        {
            Debug.DrawRay(origin, direction * maxRayDistance, missColor, 1f);
        }
    }

    /// <summary>
    /// Rayの経路に沿ってガイドラインPrefabを配置
    /// </summary>
    private void PlaceGuidelines(Vector2 start, Vector2 end, Vector2 direction, int startIndexOffset = 0)
    {
        float distance = Vector2.Distance(start, end);
        int count = Mathf.Min(guidelines.Count - startIndexOffset, Mathf.FloorToInt(distance / placementInterval));

        for (int i = 0; i < count; i++)
        {
            int index = startIndexOffset + i;
            if (index >= guidelines.Count) break;

            Vector2 pos = start + direction.normalized * (i * placementInterval);
            GameObject g = guidelines[index];
            g.transform.position = pos;

            // Z軸を180度回転させた向きに設定
            Quaternion baseRot = Quaternion.FromToRotation(Vector2.up, direction);
            g.transform.rotation = baseRot * Quaternion.Euler(0, 0, 180);

            g.SetActive(true);
        }

        // 余分なガイドラインを非表示
        for (int i = startIndexOffset + count; i < guidelines.Count; i++)
        {
            guidelines[i].SetActive(false);
        }
    }

}
