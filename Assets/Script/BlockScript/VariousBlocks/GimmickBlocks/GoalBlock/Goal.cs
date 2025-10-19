using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : BaseBlock
{
    [SerializeField] private int requiredKeys = 3; // �K�v�L�[��
    [Header("�S�[�������ڐݒ�")]
    [SerializeField] private Sprite lockedSprite;   // �����擾���̌�����
    [SerializeField] private Sprite unlockedSprite; // �S�擾��̌�����

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
            Debug.Log("�S�[�����J������܂����I");
        }
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
