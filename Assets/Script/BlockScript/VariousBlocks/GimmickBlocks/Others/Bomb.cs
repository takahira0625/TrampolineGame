using UnityEngine;

public class BombBlock : GimmickBlock
{
    [SerializeField] private ExplosionEffect explosionEffectPrefab; // VFXÅ{îjâÛêßå‰ÇÃPrefab
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private AudioClip explosionSound;

    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
        
        // îöî≠SEÇé©ìÆÉçÅ[Éh
        if (explosionSound == null)
        {
            explosionSound = Resources.Load<AudioClip>("Audio/SE/Block/Bomb");
        }
    }

    protected override void SetActiveState()
    {
        SetSprite(parameter.BombSprite);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.collider.CompareTag("Player"))
        {
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        // îöî≠SEÇçƒê∂
        if (explosionSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(explosionSound);
        }

        // îöî≠PrefabÇê∂ê¨ÇµÇƒé¿çs
        if (explosionEffectPrefab != null)
        {
            ExplosionEffect effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            effect.Initialize(explosionRadius);
        }

        Destroy(gameObject);
    }
}
