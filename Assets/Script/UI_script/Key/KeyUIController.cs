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

    [Header("�G�t�F�N�g")]
    [Tooltip("���������ɕ\������G�t�F�N�g�I�u�W�F�N�g")]
    public GameObject keyCompletionEffect;

    private ParticleSystem keyEffectParticles;

    private int requiredPartsCount = 0;

    // �ǉ��F�擾�󋵂�ǐՂ���z��
    private bool[] hasCollectedPart;

    // �ǉ��F�����t���O�i�G�t�F�N�g�̏d�����s�h�~�j
    private bool isComplete = false;

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

        // �ǉ��F�擾�󋵔z����A�K�v�ȕ��i���ŏ�����
        hasCollectedPart = new bool[requiredPartsCount];

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

        // �ǉ��F�G�t�F�N�g���\����
        if (keyCompletionEffect != null)
        {
            keyCompletionEffect.SetActive(false);
            keyEffectParticles = keyCompletionEffect.GetComponentInChildren<ParticleSystem>();
        }

        // �ǉ��F�擾�󋵂Ɗ����t���O�����Z�b�g
        isComplete = false;
        // hasCollectedPart��null�̏ꍇ�����邽�߁Anull�`�F�b�N��ǉ�
        if (hasCollectedPart != null)
        {
            for (int i = 0; i < hasCollectedPart.Length; i++)
            {
                hasCollectedPart[i] = false;
            }
        }
    }

    // KeyBlock.OnKeyPartCollected ����Ă΂��֐�
    private void HandleKeyPartCollected(int partIndex)
    {
        // ���Ɋ������Ă��邩�A�C���f�b�N�X���͈͊O�Ȃ牽�����Ȃ�
        if (isComplete || partIndex < 0 || partIndex >= requiredPartsCount)
        {
            return;
        }

        // ���Ɏ擾�ς݂Ȃ牽�����Ȃ� (�d���h�~)
        if (hasCollectedPart[partIndex])
        {
            return;
        }

        // 1. �擾�ς݂ɂ���
        hasCollectedPart[partIndex] = true;

        // 2. UI��\������
        if (partIndex < keyPartImages.Count && keyPartImages[partIndex] != null)
        {
            keyPartImages[partIndex].enabled = true;
        }

        // 3. �����������`�F�b�N����
        CheckCompletion();
    }

    // �ǉ��F�����`�F�b�N�p���\�b�h
    private void CheckCompletion()
    {
        // �擾�󋵔z����`�F�b�N
        for (int i = 0; i < requiredPartsCount; i++)
        {
            // 1�ł����擾(false)������΁A�܂������ł͂Ȃ�
            if (!hasCollectedPart[i])
            {
                return; // �`�F�b�N�I��
            }
        }

        // --- ���̍s�ɗ��� = ���ׂ� true = �����I ---

        isComplete = true; // �����t���O�𗧂Ă� (�d�����s�h�~)

        // �G�t�F�N�g��\��
        if (keyCompletionEffect != null)
        {
            Debug.Log("���������I�G�t�F�N�g��\�����܂��B");
            keyCompletionEffect.SetActive(true);

            // �擾�ς݂̃p�[�e�B�N���V�X�e���R���|�[�l���g�� Play() ���Ă�
            if (keyEffectParticles != null)
            {
                keyEffectParticles.Play();
            }
            // (�����擾�ł��Ă��Ȃ���΁A�O�̂��ߍēx�T����Play)
            else if (keyCompletionEffect.GetComponentInChildren<ParticleSystem>() != null)
            {
                keyEffectParticles = keyCompletionEffect.GetComponentInChildren<ParticleSystem>();
                keyEffectParticles.Play();
            }
        }
    }
}