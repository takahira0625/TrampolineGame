using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombBlock : GimmickBlock
{
    [Header("爆発設定")]
    [SerializeField] private ExplosionEffect explosionEffectPrefab;
    [SerializeField] private GameObject explosionEffectPrefabShake;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private AudioClip explosionSound;

    [Header("見た目設定")]
    [Tooltip("個別にスプライトを変えたい場合に指定。未設定なら parameter.BombSprite を使用します。")]
    [SerializeField] private Sprite customSprite;

    [Header("破壊対象タグ設定")]
    [Tooltip("爆発で破壊する対象のタグ一覧")]
    [SerializeField]
    private List<string> destroyableTags = new List<string>
    {
        "Block",
        "NormalBlock",
        "SpeedHalfBlock",
        "DoubleBlock",
        "SpeedDoubleBlock"
    };

    private bool hasExploded = false;

    protected override void Awake()
    {
        base.Awake();
        SetActiveState();

        // 爆発音ロード
        if (explosionSound == null)
        {
            explosionSound = Resources.Load<AudioClip>("Audio/SE/Block/Bomb");
        }
    }

    protected override void SetActiveState()
    {
        // 個別指定があればそちらを使う
        if (customSprite != null)
        {
            SetSprite(customSprite);
        }
        else
        {
            SetSprite(parameter.BombSprite);
        }
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

        // 爆発SE再生
        if (explosionSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(explosionSound);
        }

        // 爆発エフェクト生成
        if (explosionEffectPrefab != null)
        {
            ExplosionEffect effect = Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );

            // destroyableTags を渡す
            effect.Initialize(explosionRadius, this, destroyableTags);
        }

        if (explosionEffectPrefabShake != null)
        {
            Instantiate(explosionEffectPrefabShake, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
