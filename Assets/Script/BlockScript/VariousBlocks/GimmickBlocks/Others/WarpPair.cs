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

    [Header("�G�t�F�N�g�ݒ�")]
    [SerializeField] private GameObject teleportEffectPrefab;

    [Header("�X�v���C�g�ݒ�")]
    [Tooltip("�C���X�y�N�^�[�ŌʂɌ����ڂ��w��ł��܂��B�w�肵�Ȃ���� parameter.WarpPairSprite ���g�p���܂��B")]
    [SerializeField] private Sprite customSprite; //�ʃX�v���C�g�ݒ�

    private bool isDisabled = false;

    protected override void Awake()
    {
        base.Awake();
        // �ʃX�v���C�g���ݒ肳��Ă���΂����D��
        if (customSprite != null)
        {
            SetSprite(customSprite);
        }
        else
        {
            SetSprite(parameter.WarpPairSprite);
        }
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
        Time.timeScale = 0.7f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // --- �A ���[�v�O�G�t�F�N�g ---
        if (teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(teleportEffectPrefab, player.position, Quaternion.identity);
            Destroy(effect, 1.0f);
        }

        // --- �B ���[�v���s ---
        player.position = destination.transform.position + Vector3.up * offsetY;

        // --- �C ���[�v��G�t�F�N�g ---
        if (destination.teleportEffectPrefab != null)
        {
            GameObject effect = Instantiate(destination.teleportEffectPrefab, destination.transform.position, Quaternion.identity);
            Destroy(effect, 1.0f);
        }

        // �����҂i���A���^�C���Łj
        yield return new WaitForSecondsRealtime(0.1f);

        // --- �D �X���[���[�V�������� ---
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        yield return new WaitForSeconds(WaitTime);

        isDisabled = false;
        destination.isDisabled = false;
    }

#if UNITY_EDITOR
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
