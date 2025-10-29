using UnityEngine;
using System; // �C�x���g(Action)���g�����߂ɕK�v

public class KeyBlock : MonoBehaviour
{
    [SerializeField] private Vector2 targetSize = new Vector2(0.1f, 0.1f);
    [SerializeField] private AudioClip KeySE; // ���擾���̌��ʉ�

    [Header("�����i�ݒ�")]
    [Tooltip("���̃u���b�N���Ή����錮���i�̔ԍ� (0����n�܂�C���f�b�N�X)")]
    public int keyPartIndex;

    [Header("�G�t�F�N�g�ݒ�")]
    [Tooltip("Player�Ƃ̐ڐG���ɃG�t�F�N�g���Đ�����")]
    [SerializeField] private bool playEffectOnHit = true;

    // �Փ˃G�t�F�N�g(�N���X�S�̂ŋ��L)
    private static ParticleSystem hitEffectPrefab;
    private static ParticleSystem getEffectPrefab;
    private static bool isEffectLoaded = false;

    // UI�Ɂu���̔ԍ��̕��i�����ꂽ�v�ƒʒm����C�x���g
    public static event Action<int> OnKeyPartCollected;

    protected void Awake()
    {
        SetSprite();
        LoadKeySE();
        PreloadHitEffect();
    }

    /// <summary>
    /// �G�t�F�N�g�����O�Ƀ��[�h(��x�̂�)
    /// </summary>
    private void PreloadHitEffect()
    {
        if (!isEffectLoaded)
        {
            hitEffectPrefab = Resources.Load<ParticleSystem>("Effects/CFXR3 Hit Fire B (Air)");
            getEffectPrefab = Resources.Load<ParticleSystem>("Effects/Holy hit");
            isEffectLoaded = true;

            if (hitEffectPrefab == null)
            {
                Debug.LogWarning("�擾�G�t�F�N�g��������܂���: Resources/Effects/CFXR3 Hit Fire B (Air)");
            }
        }
    }

    /// <summary>
    /// ���擾���̌��ʉ���ǂݍ���
    /// </summary>
    private void LoadKeySE()
    {
        // �J�X�^��SE���ݒ肳��Ă��Ȃ��ꍇ�̂ݎ������[�h
        if (KeySE == null)
        {
            // Resources/Audio/SE/Key ����ǂݍ���
            KeySE = Resources.Load<AudioClip>("Audio/SE/Block/Key");
            
            if (KeySE == null)
            {
                Debug.LogWarning("����SE��������܂���: Resources/Audio/SE/BlockKey");
            }
        }
    }

    private void SetSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
        {
            Debug.LogWarning("SpriteRenderer �܂��� Sprite ���ݒ肳��Ă��܂���");
            return;
        }

        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(targetSize.x, targetSize.y);
        // BoxCollider2D �̃T�C�Y�����킹��
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = sr.size;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �ڐG�ʒu���擾
            Vector2 contactPoint = collision.contacts[0].point;

            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey();
                
                // SE���Đ�
                if (KeySE != null && SEManager.Instance != null)
                {
                    SEManager.Instance.PlayOneShot(KeySE);
                }
            }

            // �G�t�F�N�g���Đ�
            PlayHitEffect(contactPoint);

            // UI�ɒʒm
            OnKeyPartCollected?.Invoke(keyPartIndex);

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �Փˈʒu�ɃG�t�F�N�g�𐶐�
    /// </summary>
    private void PlayHitEffect(Vector2 position)
    {
        if (!playEffectOnHit || hitEffectPrefab == null) return;

        // �G�t�F�N�g�𐶐����čĐ�
        //ParticleSystem effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        ParticleSystem getEffect = Instantiate(getEffectPrefab, this.transform.position, Quaternion.identity);
        // �����폜
        //Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        Destroy(getEffect.gameObject, getEffect.main.duration + getEffect.main.startLifetime.constantMax);
    }
}
