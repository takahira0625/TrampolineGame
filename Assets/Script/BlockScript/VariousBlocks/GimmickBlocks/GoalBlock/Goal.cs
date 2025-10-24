using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // �K�v�L�[��
    
    [Header("�S�[�������ڐݒ�")]
    [SerializeField] private Sprite lockedSprite;   // �����擾���̌�����
    [SerializeField] private Sprite unlockedSprite; // �S�擾��̌�����
    
    [Header("����G�t�F�N�g�ݒ�")]
    [SerializeField] private ParticleSystem unlockEffectPrefab;
    [SerializeField] private Vector3 effectOffset = Vector3.zero;

    private SpriteRenderer spriteRenderer;
    private bool isUnlocked = false;

    protected override void Awake()
    {
        base.Awake();

        // �V�[������ KeyBlock �̐����擾���� requiredKeys �ɐݒ�
        KeyBlock[] keyBlocks = FindObjectsOfType<KeyBlock>();
        requiredKeys = keyBlocks.Length;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = lockedSprite; // ������Ԃ̓��b�N������
    }
    private void OnEnable()
    {
        // PlayerInventory �̃C�x���g�w��
        PlayerInventory.OnKeyCountChanged += HandleKeyCountChanged;
    }

    private void OnDisable()
    {
        //�C�x���g�w�ǉ����i���������[�N�h�~�j
        PlayerInventory.OnKeyCountChanged -= HandleKeyCountChanged;
    }

    //���̐����ς�����Ƃ��ɌĂ΂��֐�
    private void HandleKeyCountChanged(int currentKeyCount)
    {
        if (!isUnlocked && currentKeyCount >= requiredKeys)
        {
            isUnlocked = true;
            spriteRenderer.sprite = unlockedSprite;
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            SpawnUnlockEffect();
            StartCoroutine(PlayUnlockAnimation());
            Debug.Log("�S�[�����J������܂����I");
        }
    }

    private IEnumerator PlayUnlockAnimation()
    {
        // --- ���Ԃ��������ɂ��� ---
        Time.timeScale = 0.5f;

        Vector3 originalScale = transform.localScale;
        Vector3 smallScale = originalScale * 0.1f;

        // --- �k�ށi0.3�b�j---
        float duration = 0.6f;
        float return_duration = 0.1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // �� TimeScale�̉e�����󂯂Ȃ�
            transform.localScale = Vector3.Lerp(originalScale, smallScale, elapsed / duration);
            yield return null;
        }

        // --- ���ɖ߂�i0.3�b�j---
        elapsed = 0f;
        while (elapsed < return_duration)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(smallScale, originalScale, elapsed / duration);
            yield return null;
        }

        // --- ���Ԃ̗�������ɖ߂� ---
        Time.timeScale = 1f;
    }

    private void SpawnUnlockEffect()
    {
        if (unlockEffectPrefab == null) return;

        ParticleSystem effect = Instantiate(unlockEffectPrefab, transform);
        effect.transform.localPosition = effectOffset;
        effect.Play();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.TotalKeys >= requiredKeys)
            {
                GameManager.instance.Goal();
                Debug.Log("Goal! " + GameManager.instance.TotalKeys);
            }
            else
            {
                Debug.Log("�L�[�̐�������܂���");
            }
        }
    }
}
