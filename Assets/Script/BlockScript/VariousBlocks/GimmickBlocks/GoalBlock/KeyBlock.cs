using UnityEngine;
using System; // �C�x���g(Action)���g�����߂ɕK�v

public class KeyBlock : MonoBehaviour
{
    [SerializeField] private Vector2 targetSize = new Vector2(0.1f, 0.1f);
    [SerializeField] private AudioClip KeySE; // ���擾���̌��ʉ�

    [Header("�����i�ݒ�")]
    [Tooltip("���̃u���b�N���Ή����錮���i�̔ԍ� (0����n�܂�C���f�b�N�X)")]
    public int keyPartIndex; // �ǉ��F�C���X�y�N�^�[�� 0, 1, 2, 3 �̂ǂꂩ��ݒ�

    // �ǉ��FUI�Ɂu���̔ԍ��̕��i�����ꂽ�v�ƒʒm����C�x���g
    public static event Action<int> OnKeyPartCollected;

    protected void Awake()
    {
        SetSprite();
        LoadKeySE();
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

            // --- UI�p�̐V�����ʒm�i���i�ԍ���ʒm�j---
            OnKeyPartCollected?.Invoke(keyPartIndex); // UI�ɁukeyPartIndex�Ԃ����ꂽ�v�ƒʒm

            Destroy(gameObject);
        }
    }
}
