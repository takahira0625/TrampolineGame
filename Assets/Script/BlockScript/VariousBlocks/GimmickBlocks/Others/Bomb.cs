using UnityEngine;
using System.Collections;

public class BombBlock : GimmickBlock
{
    [SerializeField] private ExplosionEffect explosionEffectPrefab;
    [SerializeField] private float explosionRadius = 3f;
    private bool hasExploded = false; // �����ς݃t���O

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

    public void TriggerExplosion()
    {
        // ���ɔ����ς݂Ȃ�X�L�b�v
        if (hasExploded) return;
        hasExploded = true;

        // ����Prefab�𐶐����Ď��s
        if (explosionEffectPrefab != null)
        {
            ExplosionEffect effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            effect.Initialize(explosionRadius, this);
        }
        Destroy(gameObject);
    }
}