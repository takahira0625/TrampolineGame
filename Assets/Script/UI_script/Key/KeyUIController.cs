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

    private int requiredPartsCount = 0;
    private int collectedPartsCount = 0;
    private ParticleSystem currentEffect;

    // ���ׂĂ̌����������Ƃ��̃C�x���g
    public static event System.Action OnAllKeysCollected;

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
    }

    private void HandleKeyPartCollected(int partIndex)
    {
        if (partIndex >= 0 && partIndex < keyPartImages.Count)
        {
            Image uiImage = keyPartImages[partIndex];
            if (uiImage != null && !uiImage.enabled)
            {
                uiImage.enabled = true;
                collectedPartsCount++;

                Debug.Log($"�����i {partIndex} �����W ({collectedPartsCount}/{requiredPartsCount})");

                CheckAllKeysCollected();
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