using UnityEngine;
using System.Collections;

public class BombBlock : GimmickBlock
{
    [SerializeField] private ExplosionEffect explosionEffectPrefab;
    [SerializeField] private GameObject explosionEffectPrefabShake;
    [SerializeField] private float explosionRadius = 3f;
    private bool hasExploded = false; // �����ς݃t���O
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

    public void TriggerExplosion()
    {

        if (hasExploded) return;
        hasExploded = true;
        // ����SE���Đ�
        if (explosionSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(explosionSound);
        }

        // ����Prefab�𐶐����Ď��s
        if (explosionEffectPrefab != null)
        {
            ExplosionEffect effect = Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            effect.Initialize(explosionRadius, this);
        }
        if (explosionEffectPrefabShake != null)
        {
            GameObject shake = Instantiate(
                explosionEffectPrefabShake,
                transform.position,
                Quaternion.identity
            );

        }
        Destroy(gameObject);
    }
}