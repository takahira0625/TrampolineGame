using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private float vfxLifetime = 2f;
    [SerializeField] private float chainDelay = 0.1f;

    private float radius;
    private BombBlock originBomb;
    private List<string> destroyableTags; // Å© BombBlockÇ©ÇÁéÛÇØéÊÇÈ

    public void Initialize(float radius, BombBlock originBomb = null, List<string> destroyableTags = null)
    {
        this.radius = radius;
        this.originBomb = originBomb;
        this.destroyableTags = destroyableTags ?? new List<string>();

        PlayEffect();
        DestroyBlocks();
        TriggerChainExplosions();
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

    private void TriggerChainExplosions()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            BombBlock bomb = hit.GetComponent<BombBlock>();
            if (bomb != null && bomb != originBomb)
            {
                StartCoroutine(DelayedExplosion(bomb));
            }
        }
    }

    private IEnumerator DelayedExplosion(BombBlock bomb)
    {
        yield return new WaitForSeconds(chainDelay);
        if (bomb != null)
        {
            bomb.TriggerExplosion();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
