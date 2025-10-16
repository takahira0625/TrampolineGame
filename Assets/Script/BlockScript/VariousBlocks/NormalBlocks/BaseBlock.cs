using UnityEngine;
using System.Collections.Generic;

public class BaseBlock : MonoBehaviour
{
    [SerializeField] protected ParameterConfig parameter;
    [SerializeField] protected BreakBlock breakBlock; // ���o�S���̃X�N���v�g

    [Header("���ʉ��ݒ�")]
    [Tooltip("Player�Ƃ̐ڐG���Ɍ��ʉ����Đ�����")]
    [SerializeField] protected bool playSoundOnHit = true;
    [Tooltip("���̃u���b�N��p�̌��ʉ��i�ݒ肵���ꍇ�̓^�O�Ɋ֌W�Ȃ����̉����Đ��j")]
    [SerializeField] protected AudioClip customHitSound;

    [Header("�G�t�F�N�g�ݒ�")]
    [Tooltip("Player�Ƃ̐ڐG���ɃG�t�F�N�g���Đ�����")]
    [SerializeField] protected bool playEffectOnHit = true;

    protected int health;

    // �� �ǉ�: ���ʉ��L���b�V���i�N���X�S�̂ŋ��L�j
    private static Dictionary<string, AudioClip> soundCache = new Dictionary<string, AudioClip>();
    private static AudioClip defaultSound;
    private static bool isSoundCacheInitialized = false;

    // �� �ǉ�: ���ʃG�t�F�N�g�i�N���X�S�̂ŋ��L�j
    private static ParticleSystem hitEffectPrefab;
    private static bool isEffectLoaded = false;

    // �� �ǉ�: ���̃C���X�^���X�Ŏg�p������ʉ��iAwake�Ŏ��O���[�h�j
    private AudioClip cachedHitSound;

    protected virtual void Awake()
    {
        Physics.bounceThreshold = 0.0f;
        //�@�R���|�[�l���g��ǉ��iBreakBlock.cs,ParameterConfig.cs�j

        // -------------------------------
        // BreakBlock �������擾�E�ǉ�
        // -------------------------------
        if (breakBlock == null)
        {
            breakBlock = GetComponent<BreakBlock>();
            if (breakBlock == null)
            {
                breakBlock = gameObject.AddComponent<BreakBlock>();
            }
        }
        // -------------------------------
        // ParameterConfig �������擾
        // -------------------------------
        if (parameter == null)
        {
            // Resources �t�H���_�� ParameterConfig ��u���z��
            parameter = Resources.Load<ParameterConfig>("ParameterConfig");
        }
        // health �ݒ�
        if (parameter != null)
            health = parameter.Health;

        SetSprite(parameter.baseSprite);

        // �� �ǉ�: ���ʉ��̎��O���[�h
        PreloadHitSound();
        
        // �� �ǉ�: �G�t�F�N�g�̎��O���[�h�i����̂݁j
        PreloadHitEffect();
    }

    // �� �V�K�ǉ�: ���ʉ������O�Ƀ��[�h���ăL���b�V��
    private void PreloadHitSound()
    {
        // �J�X�^�����ʉ����ݒ肳��Ă���ꍇ
        if (customHitSound != null)
        {
            cachedHitSound = customHitSound;
            return;
        }

        // ����̂݃f�t�H���g���ʉ������[�h
        if (!isSoundCacheInitialized)
        {
            defaultSound = Resources.Load<AudioClip>("Audio/SE/NormalBlock");
            isSoundCacheInitialized = true;
        }

        // �^�O�ɉ��������ʉ����擾
        string blockTag = gameObject.tag;

        // �L���b�V���ɑ��݂��邩�m�F
        if (soundCache.ContainsKey(blockTag))
        {
            cachedHitSound = soundCache[blockTag];
        }
        else
        {
            // �V�K���[�h���ăL���b�V���ɕۑ�
            string soundPath = "Audio/SE/" + blockTag;
            AudioClip clip = Resources.Load<AudioClip>(soundPath);

            if (clip != null)
            {
                soundCache[blockTag] = clip;
                cachedHitSound = clip;
            }
            else
            {
                // �f�t�H���g���ʉ����g�p
                cachedHitSound = defaultSound;
                soundCache[blockTag] = defaultSound; // �L���b�V���ɕۑ�
            }
        }
    }

    // �� �V�K�ǉ�: �G�t�F�N�g�����O�Ƀ��[�h�i����̂݁A�S�u���b�N���ʁj
    private void PreloadHitEffect()
    {
        if (!isEffectLoaded)
        {
            hitEffectPrefab = Resources.Load<ParticleSystem>("Effects/CFXR3 Hit Fire B (Air)");
            isEffectLoaded = true;

            if (hitEffectPrefab == null)
            {
                Debug.LogWarning("HitSpark�G�t�F�N�g��������܂���: Resources/Effects/CFXR3 Hit Fire B (Air)");
            }
        }
    }

    // �X�v���C�g��ύX
    protected virtual void SetSprite(Sprite sprite)
    {
        if (sprite == null || parameter == null) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // �X�v���C�g��ݒ�
        sr.sprite = sprite;

        // Sliced���[�h�ɐݒ�
        sr.drawMode = SpriteDrawMode.Sliced;

        // �T�C�Y��K�p
        sr.size = new Vector2(parameter.Width, parameter.Height);

        // BoxCollider2D�̃T�C�Y�����킹��
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(parameter.Width, parameter.Height);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �� �ǉ�: �ڐG�ʒu���擾
            Vector2 contactPoint = collision.contacts[0].point;

            // �� �C��: ���ʉ����Đ�
            PlayHitSound();

            // �� �ǉ�: �G�t�F�N�g���Đ�
            PlayHitEffect(contactPoint);

            // �̗͂����炷
            TakeDamage(1);
        }
    }

    protected virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            // �j�󏈗���BreakBlock�ɒʒm
            if (breakBlock != null)
                breakBlock.OnBreak();
        }
    }

    // �� �C��: �L���b�V�����ꂽ���ʉ��𑦍��ɍĐ�
    protected virtual void PlayHitSound()
    {
        if (!playSoundOnHit) return;

        // �L���b�V�����ꂽ���ʉ����Đ��i�����ɍĐ������j
        if (cachedHitSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(cachedHitSound);
        }
    }

    // �� �V�K�ǉ�: �Փˈʒu�ɃG�t�F�N�g�𐶐��i�V���v�������j
    protected virtual void PlayHitEffect(Vector2 position)
    {
        if (!playEffectOnHit || hitEffectPrefab == null) return;

        // �G�t�F�N�g�𐶐����čĐ�
        ParticleSystem effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        
        // �����폜
        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
    }
}
