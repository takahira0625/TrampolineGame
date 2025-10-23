using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KeyUIController : MonoBehaviour
{
    [Header("�X�e�[�W�ݒ�")]
    [Tooltip("���̃X�e�[�W�Ŏg�p���錮�ݒ�t�@�C��")]
    public StageKeyConfig currentStageConfig;

    [Header("UI�Q��")]
    [Tooltip("UI�̌����i�C���[�W��Image�R���|�[�l���g (�ő�4��)")]
    public List<Image> keyPartImages = new List<Image>();

    [Header("�G�t�F�N�g�ݒ�")]
    [Tooltip("���ׂĂ̌����������Ƃ��ɐ�������G�t�F�N�g")]
    [SerializeField] private ParticleSystem completeEffectPrefab;
    [Tooltip("�G�t�F�N�g�̐����ʒu�I�t�Z�b�g")]
    [SerializeField] private Vector3 effectOffset = Vector3.zero;
    [Tooltip("�G�t�F�N�g�������폜����")]
    [SerializeField] private bool autoDestroyEffect = true;

    [Header("�G�t�F�N�g")]
    [Tooltip("���������ɕ\������G�t�F�N�g�I�u�W�F�N�g")]
    public GameObject keyCompletionEffect;

    private ParticleSystem keyEffectParticles;

    private int requiredPartsCount = 0;
    private int collectedPartsCount = 0;
    private ParticleSystem currentEffect;

    // ���ׂĂ̌����������Ƃ��̃C�x���g
    public static event System.Action OnAllKeysCollected;

    // �ǉ��F�擾�󋵂�ǐՂ���z��
    private bool[] hasCollectedPart;

    // �ǉ��F�����t���O�i�G�t�F�N�g�̏d�����s�h�~�j
    private bool isComplete = false;

    // �C�x���g�w��
    private void OnEnable()
    {
        KeyBlock.OnKeyPartCollected += HandleKeyPartCollected;
    }

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

        SetupUISprites();
        ResetUI();
        collectedPartsCount = 0;
    }

    private void SetupUISprites()
    {
        requiredPartsCount = currentStageConfig.keyPartUISprites.Count;

        // �ǉ��F�擾�󋵔z����A�K�v�ȕ��i���ŏ�����
        hasCollectedPart = new bool[requiredPartsCount];

        // UI�X���b�g�i�ő�4�j�����[�v
        for (int i = 0; i < keyPartImages.Count; i++)
        {
            Image uiImage = keyPartImages[i];
            if (uiImage == null) continue;

            if (i < requiredPartsCount)
            {
                uiImage.sprite = currentStageConfig.keyPartUISprites[i];
                uiImage.gameObject.SetActive(true);
            }
            else
            {
                uiImage.gameObject.SetActive(false);
            }
        }
    }

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

    /// <summary>
    /// ���ׂĂ̌������������`�F�b�N
    /// </summary>
    private void CheckAllKeysCollected()
    {
        if (collectedPartsCount >= requiredPartsCount)
        {
            Debug.Log("���ׂĂ̌����i�������܂����I");
            
            // �G�t�F�N�g�𐶐�
            SpawnCompleteEffect();
            
            // �C�x���g����
            OnAllKeysCollected?.Invoke();
        }
    }

    /// <summary>
    /// �����G�t�F�N�g�𐶐�
    /// </summary>
    private void SpawnCompleteEffect()
    {
        if (completeEffectPrefab == null)
        {
            Debug.LogWarning("�����G�t�F�N�g���ݒ肳��Ă��܂���");
            return;
        }

        // �����̃G�t�F�N�g������΍폜
        if (currentEffect != null)
        {
            Destroy(currentEffect.gameObject);
        }

        // ���̃I�u�W�F�N�g�̎q�Ƃ��ăG�t�F�N�g�𐶐�
        currentEffect = Instantiate(completeEffectPrefab, transform);
        currentEffect.transform.localPosition = effectOffset;
        currentEffect.transform.localRotation = Quaternion.identity;
        currentEffect.transform.localScale = Vector3.one;

        // �Đ�
        currentEffect.Play();

        Debug.Log("�������G�t�F�N�g�𐶐����܂���");

        // �����폜
        if (autoDestroyEffect)
        {
            float duration = currentEffect.main.duration + currentEffect.main.startLifetime.constantMax;
            Destroy(currentEffect.gameObject, duration);
        }
    }

    /// <summary>
    /// �G�t�F�N�g���蓮�ō폜
    /// </summary>
    public void DestroyEffect()
    {
        if (currentEffect != null)
        {
            Destroy(currentEffect.gameObject);
            currentEffect = null;
        }
    }

    /// <summary>
    /// ���ׂĂ̌��������Ă��邩�m�F
    /// </summary>
    public bool AreAllKeysCollected()
    {
        return collectedPartsCount >= requiredPartsCount;
    }

    /// <summary>
    /// ���W�i�����擾 (0.0 ~ 1.0)
    /// </summary>
    public float GetCollectionProgress()
    {
        if (requiredPartsCount == 0) return 0f;
        return (float)collectedPartsCount / requiredPartsCount;
    }

    private void OnDestroy()
    {
        // �N���[���A�b�v
        DestroyEffect();
    }
}