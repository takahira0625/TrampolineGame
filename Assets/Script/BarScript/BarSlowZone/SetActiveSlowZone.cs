using UnityEngine;

public class SetActiveSlowZone : MonoBehaviour
{
    private Renderer[] renderers;

    void Awake()
    {
        // 自分自身と全ての子オブジェクトのRendererを取得
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SetRenderersEnabled(true);
        }
        else
        {
            SetRenderersEnabled(false);
        }
    }

    // Rendererを一括で有効/無効にする関数
    private void SetRenderersEnabled(bool enabled)
    {
        foreach (var rend in renderers)
        {
            rend.enabled = enabled;
        }
    }
}
