using UnityEngine;

public class SkyBeansManager : MonoBehaviour
{
    //ステージ上のすべてのSkyBeans
    private SkyBeans[] allSkyBeans;
    //現在有効なSkyBeans一つ
    private SkyBeans currentHovered = null;

    private void Start()
    {
        allSkyBeans = FindObjectsOfType<SkyBeans>();
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SkyBeans hovered = GetNearestSkyBeans(mousePos);

        // ハイライト切り替え
        if (hovered != currentHovered)
        {
            if (currentHovered != null)
                currentHovered.SetHighlight(false);

            currentHovered = hovered;

            if (currentHovered != null)
                currentHovered.SetHighlight(true);
        }

        // 左クリックで長方形生成
        if (Input.GetMouseButtonDown(0) && currentHovered != null)
        {
            currentHovered.GenerateNewRectangle(mousePos);
        }
    }

    /// <summary>
    /// 範囲内のSkyBeansの中から最も近いものを返す
    /// </summary>
    private SkyBeans GetNearestSkyBeans(Vector2 clickPosition)
    {
        SkyBeans closest = null;
        float minDistance = float.MaxValue;

        foreach (SkyBeans sb in allSkyBeans)
        {
            if (!sb.IsWithinRange(clickPosition)) continue;

            float dist = Vector2.Distance(sb.transform.position, clickPosition);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = sb;
            }
        }

        return closest;
    }
}