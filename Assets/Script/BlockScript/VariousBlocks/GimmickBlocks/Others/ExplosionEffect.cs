using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private float vfxLifetime = 2f;
    [SerializeField] private float chainDelay = 0.1f; // �A�������̃f�B���C
    [SerializeField]
    private List<string> destroyableTags = new List<string>
    {
        "Block",
        "NormalBlock",
        "SpeedHalfBlock",
        "DoubleBlock",
        "SpeedDoubleBlock"
    };

    private float radius;
    private BombBlock originBomb; // ���̔��e

    public void Initialize(float radius, BombBlock originBomb = null)
    {
        this.radius = radius;
        this.originBomb = originBomb;
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
            // �����ȊO��BombBlock�����o
            if (bomb != null && bomb != originBomb)
            {
                // chainDelay�̎��Ԍ�ɔ������N��
                StartCoroutine(DelayedExplosion(bomb));
            }
        }
    }

    private IEnumerator DelayedExplosion(BombBlock bomb)
    {
        yield return new WaitForSeconds(chainDelay);
        if (bomb != null) // ���ɔj�󂳂�Ă��Ȃ����`�F�b�N
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