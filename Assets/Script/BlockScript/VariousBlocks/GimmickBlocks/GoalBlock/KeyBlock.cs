using UnityEngine;

public class KeyBlock : MonoBehaviour
{
    [SerializeField] private Vector2 targetSize = new Vector2(0.1f, 0.1f);
    [SerializeField] private AudioClip KeySE; // ���擾���̌��ʉ�

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

            Destroy(gameObject);
        }
    }
}
