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

    [SerializeField] private float WaitTime = 0.5f;

    [SerializeField] private GameObject teleportEffectPrefab;

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
        isDisabled = true;
        destination.isDisabled = true;

        // --- �@ �X���[���[�V�����J�n ---
        Time.timeScale = 0.3f; // �S�̂��������Ɂi��F0.3�{�j
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // --- �A ���[�v�O�G�t�F�N�g ---
        if (teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(teleportEffectPrefab, player.position, Quaternion.identity);
            Destroy(effect, 1.0f); // �� 1�b��Ɏ����폜
        }

        // �����҂i�G�t�F�N�g�������ԕ��j
        yield return new WaitForSecondsRealtime(0.1f);

        // --- �B ���[�v���s ---
        player.position = destination.transform.position + Vector3.up * offsetY;

        // --- �C ���[�v��G�t�F�N�g ---
        if (destination.teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(destination.teleportEffectPrefab, destination.transform.position, Quaternion.identity);
            Destroy(effect, 1.0f);
        }

        // �����҂�
        yield return new WaitForSecondsRealtime(0.1f);

        // --- �D �X���[���[�V�������� ---
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        yield return new WaitForSeconds(WaitTime);

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
