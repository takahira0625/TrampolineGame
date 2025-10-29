using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // �K�v�L�[��
    
    [Header("�S�[�������ڐݒ�")]
    [SerializeField] private Sprite lockedSprite;   // �����擾���̌�����
    [SerializeField] private Sprite unlockedSprite; // �S�擾��̌�����
    [SerializeField] private GameObject ShieldEffect; // �����擾���Ă��Ȃ����ɂԂ������Ƃ��̃G�t�F�N�g
    [SerializeField] private GameObject ShieldEffectHit; // �����擾���Ă��Ȃ����ɂԂ������Ƃ��̃G�t�F�N�g

    [Header("����G�t�F�N�g�ݒ�")]
    [SerializeField] private ParticleSystem unlockEffectPrefab;
    [SerializeField] private Vector3 effectOffset = Vector3.zero;

    private SpriteRenderer spriteRenderer;
    private bool isUnlocked = false;
    private bool hasGoaled = false; // �S�[�������ς݃t���O��ǉ�

    [Header("�T�E���h�ݒ�")]
    [SerializeField] private AudioClip goalSE;
    [SerializeField, Header("�V�[���h�q�b�g��")]
    private AudioClip shieldHitSE;
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
        //�����Ɍ������鉉�o�����

        // --- �k�ށi0.3�b�j---
        float duration = 0.6f;
        float return_duration = 0.1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
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
            if (GameManager.instance.TotalKeys >= requiredKeys && !hasGoaled)
            {
                hasGoaled = true; // �S�[�������ς݃t���O�𗧂Ă�
                BGMManager.Instance.Stop();
                SEManager.Instance.PlayOneShot(goalSE);
                GameManager.instance.Goal();
                Debug.Log("Goal! " + GameManager.instance.TotalKeys);
            }
            else
            {
                ContactPoint2D contact = collision.GetContact(0);
                if (ShieldEffect != null)
                {
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

                    // �S�[���̈ʒu�ŃG�t�F�N�g�𐶐�
                    GameObject effect = Instantiate(
                        ShieldEffect,
                        transform.position,
                        rotation
                    );

                    if (shieldHitSE != null && SEManager.Instance != null && !hasGoaled)
                    {
                        SEManager.Instance.PlayOneShot(shieldHitSE);
                    }


                    Destroy(effect, 2f);
                }
                if (ShieldEffectHit != null)
                {
                    // �Փˈʒu�ŃG�t�F�N�g�𐶐�

                    GameObject effectHit = Instantiate(
                        ShieldEffectHit,
                        contact.point,
                        Quaternion.identity
                    );
                    Destroy(effectHit, 1.5f);
                }

            }
        }
    }
}
