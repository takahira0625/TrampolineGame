using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
        LoadWarpSE();
        // �SWarp���L���b�V��
        allWarps = FindObjectsOfType<Warp>();
    }
    /// <summary>
    /// ���擾���̌��ʉ���ǂݍ���
    /// </summary>
    private void LoadWarpSE()
    {
        // �J�X�^��SE���ݒ肳��Ă��Ȃ��ꍇ�̂ݎ������[�h
        if (warpSE == null)
        {
            // Resources/Audio/SE/Key ����ǂݍ���
            warpSE = Resources.Load<AudioClip>("Audio/SE/Block/WarpBlock");

            if (warpSE == null)
            {
                Debug.LogWarning("����SE��������܂���: Resources/Audio/SE/Block/WarpBlock");
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDisabled) return; // ���������͉������Ȃ�

        Warp destination = GetRandomWarpExcludingSelf();
        if (destination == null) return;
        SEManager.Instance.PlayOneShot(warpSE);
        StartCoroutine(WarpPlayer(other.transform, destination));
    }

    private Warp GetRandomWarpExcludingSelf()
    {
        if (allWarps == null || allWarps.Length <= 1) return null;

        List<Warp> list = new List<Warp>();
        foreach (var w in allWarps)
        {
            if (w != this && !w.isDisabled)
                list.Add(w);
        }

        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
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
