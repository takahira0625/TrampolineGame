using UnityEngine;

public class BombBlock : GimmickBlock
{
    [SerializeField] private ExplosionEffect explosionEffectPrefab; // VFX�{�j�󐧌��Prefab
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private AudioClip explosionSound;

    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
        
        // ����SE���������[�h
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
        // ����SE���Đ�
        if (explosionSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(explosionSound);
        }

        // ����Prefab�𐶐����Ď��s
        if (explosionEffectPrefab != null)
        {
            ExplosionEffect effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            effect.Initialize(explosionRadius);
        }

        Destroy(gameObject);
    }
}
