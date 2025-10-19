using UnityEngine;
using UnityEngine.UI; // Image����������
using System.Collections.Generic;

public class KeyUIController : MonoBehaviour
{
    [Header("�X�e�[�W�ݒ�")]
    [Tooltip("���̃X�e�[�W�Ŏg�p���錮�ݒ�t�@�C��")]
    public StageKeyConfig currentStageConfig; // �X�e�b�v6�Őݒ�

    [Header("UI�Q��")]
    [Tooltip("UI�̌����i�C���[�W��Image�R���|�[�l���g (�ő�4��)")]
    public List<Image> keyPartImages = new List<Image>(); // �X�e�b�v6�Őݒ�

    private int requiredPartsCount = 0;

    // �C�x���g�w��
    private void OnEnable()
    {
        // KeyBlock��������u���i�ԍ��v�̃C�x���g���w�ǂ���
        KeyBlock.OnKeyPartCollected += HandleKeyPartCollected;
    }

    // �C�x���g�w�ǉ����i�K�{�j
    private void OnDisable()
    {
        KeyBlock.OnKeyPartCollected -= HandleKeyPartCollected;
    }

    void Start()
    {
        if (currentStageConfig == null)
        {
            Debug.LogError("KeyUIController�� StageKeyConfig ���ݒ肳��Ă��܂���I");
            return;
        }

        // 1. UI�̃X�v���C�g�ݒ�
        SetupUISprites();

        // 2. ������Ԃł͂��ׂĔ�\��
        ResetUI();
    }

    // Config�Ɋ�Â��AUI Image�ɐ�����Sprite�����蓖�āA�s�v�ȃX���b�g���\���ɂ���
    private void SetupUISprites()
    {
        // Config����K�v�����擾
        requiredPartsCount = currentStageConfig.keyPartUISprites.Count;

        // UI�X���b�g�i�ő�4�j�����[�v
        for (int i = 0; i < keyPartImages.Count; i++)
        {
            Image uiImage = keyPartImages[i];
            if (uiImage == null) continue;

            // ���̃X�e�[�W�Ŏg���X���b�g���H (i < requiredPartsCount)
            if (i < requiredPartsCount)
            {
                // Config�ɐݒ肳�ꂽSprite�����蓖��
                uiImage.sprite = currentStageConfig.keyPartUISprites[i];
                uiImage.gameObject.SetActive(true); // �X���b�g���̂͗L��
            }
            else
            {
                // ���̃X�e�[�W�ł͎g��Ȃ��X���b�g
                uiImage.gameObject.SetActive(false);
            }
        }
    }

    // UI��������ԁi���ׂĔ�\���j�ɂ���
    private void ResetUI()
    {
        foreach (Image img in keyPartImages)
        {
            if (img != null)
            {
                img.enabled = false;
            }
        }
    }

    // KeyBlock.OnKeyPartCollected ����Ă΂��֐�
    private void HandleKeyPartCollected(int partIndex)
    {
        // �C���f�b�N�X��UI���X�g�͈͓̔����`�F�b�N
        if (partIndex >= 0 && partIndex < keyPartImages.Count)
        {
            Image uiImage = keyPartImages[partIndex];
            if (uiImage != null)
            {
                // �Y������ԍ��̕��iUI������L���ɂ���
                uiImage.enabled = true;
            }
        }
    }
}