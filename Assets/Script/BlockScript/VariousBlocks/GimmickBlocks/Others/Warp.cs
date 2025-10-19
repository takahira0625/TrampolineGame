using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : BaseBlock
{
    private static Warp[] allWarps;
    private bool isDisabled = false;
    [SerializeField] private AudioClip warpSE;        // ���[�v��
    [SerializeField] private float disableDuration = 0.5f; // ���[�v���E����ꎞ������
    [SerializeField] private float offsetY = 1.0f;         // �v���C���[��������ɏo��

    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.WarpSprite);
        
        if (warpSE == null)
        {
            warpSE = Resources.Load<AudioClip>("Audio/SE/Block/WarpBlock");
        }
        
        allWarps = FindObjectsOfType<Warp>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isDisabled) return;

        // SE�Đ�
        if (warpSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(warpSE);
        }

        Warp destination = GetRandomWarpExcludingSelf();
        if (destination != null)
        {
            StartCoroutine(WarpPlayer(other.transform, destination));
        }
    }

    private Warp GetRandomWarpExcludingSelf()
    {
        if (allWarps == null || allWarps.Length <= 1) return null;

        List<Warp> availableWarps = new List<Warp>();
        foreach (var warp in allWarps)
        {
            if (warp != this && !warp.isDisabled)
            {
                availableWarps.Add(warp);
            }
        }

        return availableWarps.Count > 0 ? availableWarps[Random.Range(0, availableWarps.Count)] : null;
    }

    private IEnumerator WarpPlayer(Transform player, Warp destination)
    {
        // ���[�v���E����ꎞ�I�ɖ������i�߂�h�~�j
        isDisabled = true;
        destination.isDisabled = true;

        // ���[�v����
        player.position = destination.transform.position + Vector3.up * offsetY;

        // ���Ԍo�ߌ�ɗL����
        yield return new WaitForSeconds(disableDuration);
        isDisabled = false;
        destination.isDisabled = false;
    }
}
