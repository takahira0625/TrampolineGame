using UnityEngine;
using System.Collections;

public class BombBlock : GimmickBlock
{
    [SerializeField] private ExplosionEffect explosionEffectPrefab;
    [SerializeField] private float explosionRadius = 3f;
    private bool hasExploded = false; // 爆発済みフラグ

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
        // 既に爆発済みならスキップ
        if (hasExploded) return;
        hasExploded = true;

        // 爆発Prefabを生成して実行
        if (explosionEffectPrefab != null)
        {
            ExplosionEffect effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            effect.Initialize(explosionRadius, this);
        }
        Destroy(gameObject);
    }
}