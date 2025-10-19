using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarpRandom: BaseBlock
{
    private static WarpRandom[] allWarps;
    private bool isDisabled = false;

    [SerializeField] private float disableDuration = 0.5f; // ���[�v���E����ꎞ������
    [SerializeField] private float offsetY = 1.0f;         // �v���C���[��������ɏo��

    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.WarpRandomSprite);

        // �SWarp���L���b�V��
        allWarps = FindObjectsOfType<WarpRandom>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDisabled) return; // ���������͉������Ȃ�

        WarpRandom destination = GetRandomWarpExcludingSelf();
        if (destination == null) return;

        StartCoroutine(WarpPlayer(other.transform, destination));
    }

    private WarpRandom GetRandomWarpExcludingSelf()
    {
        if (allWarps == null || allWarps.Length <= 1) return null;

        List<WarpRandom> list = new List<WarpRandom>();
        foreach (var w in allWarps)
        {
            if (w != this && !w.isDisabled)
                list.Add(w);
        }

        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
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
