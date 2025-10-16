using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private GameObject vfxPrefab; // 実際のエフェクト（例：CFXR Impact Glowing HDR）
    [SerializeField] private float vfxLifetime = 2f;
    [SerializeField] private List<string> destroyableTags = new List<string>
    {
        "Block",
        "NormalBlock",
        "SpeedHalfBlock",
        "DoubleBlock",
        "SpeedDoubleBlock",
        "WarpBlock"
    };
    private float radius;

    public void Initialize(float radius)
    {
        this.radius = radius;
        PlayEffect();
        DestroyBlocks();
    }

    private void PlayEffect()
    {
        if (vfxPrefab != null)
        {
            GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, vfxLifetime);
        }
    }

    private void DestroyBlocks()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (destroyableTags.Contains(hit.tag))
            {
                Destroy(hit.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}