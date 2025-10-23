using UnityEngine;
using System.Collections;

/// <summary>
/// WarpPair�F�w�肵���y�A�ɂ̂݃��[�v����^�C�v
/// </summary>
public class WarpPair : BaseBlock
{
    [Header("�y�A�ݒ�")]
    [Tooltip("���[�v��ƂȂ�y�A��WarpPair�I�u�W�F�N�g")]
    [SerializeField] private WarpPair pairedWarp;

    [Header("�ݒ�")]
    [SerializeField] private float disableDuration = 0.5f; // ���[�v���E����ꎞ������
    [SerializeField] private float offsetY = 1.0f;         // �v���C���[��������ɏo��

    private bool isDisabled = false;

    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.WarpPairSprite); // �X�v���C�g�����ւ��\�Ȃ�
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDisabled) return;
        if (pairedWarp == null)
        {
            Debug.LogWarning($"{name} �Ƀy�A���ݒ肳��Ă��܂���B");
            return;
        }

        StartCoroutine(WarpPlayer(other.transform, pairedWarp));
    }

    private IEnumerator WarpPlayer(Transform player, WarpPair destination)
    {
        // ���[�v���E����ꎞ�I�ɖ������i�߂�h�~�j
        isDisabled = true;
        destination.isDisabled = true;

        // �v���C���[�����[�v
        player.position = destination.transform.position + Vector3.up * offsetY;

        // �����҂��čĂїL����
        yield return new WaitForSeconds(disableDuration);
        isDisabled = false;
        destination.isDisabled = false;
    }

#if UNITY_EDITOR
    // �V�[����Ńy�A�֌W�����₷������f�o�b�O�`��
    private void OnDrawGizmos()
    {
        if (pairedWarp != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, pairedWarp.transform.position);
            Gizmos.DrawSphere(pairedWarp.transform.position, 0.2f);
        }
    }
#endif
}
