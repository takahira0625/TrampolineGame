using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombBlock : GimmickBlock
{
    [Header("�����ݒ�")]
    [SerializeField] private ExplosionEffect explosionEffectPrefab;
    [SerializeField] private GameObject explosionEffectPrefabShake;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private AudioClip explosionSound;

    [Header("�����ڐݒ�")]
    [Tooltip("�ʂɃX�v���C�g��ς������ꍇ�Ɏw��B���ݒ�Ȃ� parameter.BombSprite ���g�p���܂��B")]
    [SerializeField] private Sprite customSprite;

    [Header("�j��Ώۃ^�O�ݒ�")]
    [Tooltip("�����Ŕj�󂷂�Ώۂ̃^�O�ꗗ")]
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

        // ���������[�h
        if (explosionSound == null)
        {
            explosionSound = Resources.Load<AudioClip>("Audio/SE/Block/Bomb");
        }
    }

    protected override void SetActiveState()
    {
        // �ʎw�肪����΂�������g��
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

        // ����SE�Đ�
        if (explosionSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(explosionSound);
        }

        // �����G�t�F�N�g����
        if (explosionEffectPrefab != null)
        {
            ExplosionEffect effect = Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );

            // destroyableTags ��n��
            effect.Initialize(explosionRadius, this, destroyableTags);
        }

        if (explosionEffectPrefabShake != null)
        {
            Instantiate(explosionEffectPrefabShake, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
