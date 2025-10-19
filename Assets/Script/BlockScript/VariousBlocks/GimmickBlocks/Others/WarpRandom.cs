using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpRandom: BaseBlock
{
    private static WarpRandom[] allWarps;
    private bool isDisabled = false;
    [SerializeField] private AudioClip warpSE;        // ���[�v��
    [SerializeField] private float disableDuration = 0.5f; // ���[�v���E����ꎞ������
    [SerializeField] private float offsetY = 1.0f;         // �v���C���[��������ɏo��

    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.WarpRandomSprite);

        // �SWarp���L���b�V��
        allWarps = FindObjectsOfType<WarpRandom>();

        
        if (warpSE == null)
        {
            warpSE = Resources.Load<AudioClip>("Audio/SE/Block/WarpBlock");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isDisabled) return;

        // SE�Đ�
        if (warpSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(warpSE);
        }

        WarpRandom destination = GetRandomWarpExcludingSelf();
        if (destination == null) return;

        StartCoroutine(WarpPlayer(other.transform, destination));
    }

    private WarpRandom GetRandomWarpExcludingSelf()
    {
        if (allWarps == null || allWarps.Length <= 1) return null;

        List<WarpRandom> list = new List<WarpRandom>();

        foreach (var warp in allWarps)
        {
            if (warp != this && !warp.isDisabled)
            {
                list.Add(warp);
            }
        }

        return list.Count > 0 ? list[Random.Range(0, list.Count)] : null;
    }

    private IEnumerator WarpPlayer(Transform player, WarpRandom destination)
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
