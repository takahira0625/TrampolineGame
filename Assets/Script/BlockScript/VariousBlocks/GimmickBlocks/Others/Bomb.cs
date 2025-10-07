using UnityEngine;

public class BombBlock : GimmickBlock
{
    [SerializeField] private ExplosionEffect explosionEffectPrefab; // VFXÅ{îjâÛêßå‰ÇÃPrefab
    [SerializeField] private float explosionRadius = 3f;

    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
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
        // ?? îöî≠PrefabÇê∂ê¨ÇµÇƒé¿çs
        if (explosionEffectPrefab != null)
        {
            ExplosionEffect effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            effect.Initialize(explosionRadius);
        }

        Destroy(gameObject);
    }
}
