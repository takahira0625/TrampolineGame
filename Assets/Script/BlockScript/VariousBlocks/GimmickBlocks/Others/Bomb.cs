using UnityEngine;
using System.Collections;

public class BombBlock : GimmickBlock
{
    [SerializeField] private ExplosionEffect explosionEffectPrefab;
    [SerializeField] private GameObject explosionEffectPrefabShake;
    [SerializeField] private float explosionRadius = 3f;
    private bool hasExploded = false; // 爆発済みフラグ
    [SerializeField] private AudioClip explosionSound;

    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
        
        // 爆発SEを自動ロード
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
        // 爆発SEを再生
        if (explosionSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(explosionSound);
        }

        // 爆発Prefabを生成して実行
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