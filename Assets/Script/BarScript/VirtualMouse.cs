using UnityEngine;

/// <summary>
/// Input.GetAxis ���g���ĉ��z�}�E�X�𐧌�
/// ��ʒ[�ł��}�E�X���͂�����
/// </summary>
public class VirtualMouse : MonoBehaviour
{
    [Header("�\���ݒ�")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("�}�E�X���x")]
    [SerializeField] private float mouseSensitivity = 1f; // �}�E�X���͂̊��x

    [Header("�Ǐ]���x")]
    [SerializeField, Range(0f, 1f)] private float normalFollowSpeed = 1f; // �ʏ�̒Ǐ]���x
    [SerializeField, Range(0f, 1f)] private float leftClickFollowSpeed = 0.1f; // ���N���b�N���̑��x

    private Vector2 virtualPos; // ���[���h���W�ł̉��z�}�E�X�ʒu
    private Camera cam;
    private bool isUpdating = true;

    void Awake()
    {
        cam = Camera.main;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // �X�v���C�g��O�ʂɕ\��
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Foreground";
            spriteRenderer.sortingOrder = 100;
        }

        // �}�E�X�J�[�\�����\��
        Cursor.visible = false;
    }

    void Start()
    {
        // �����ʒu���}�E�X�̌��݈ʒu�ɍ��킹��
        virtualPos = GetMouseWorldPosition();
        transform.position = virtualPos;
    }

    void Update()
    {
        if (!isUpdating) return;
        // Input.GetAxis �͉�ʒ[�ł��l������
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        Vector2 mouseDelta = new Vector2(mouseX, mouseY);

        // ���N���b�N���͂������ړ�
        if (Input.GetMouseButton(0))
        {
            virtualPos += mouseDelta * leftClickFollowSpeed;
        }
        else
        {
            // �ʏ�̓}�E�X���͂����̂܂ܓK�p
            virtualPos += mouseDelta * normalFollowSpeed;
        }

        // ���[���h���W���X�N���[�����W�͈͓̔��ɐ����i��ʊO�ɏo�Ȃ��悤�ɂ���j
        ClampPositionToScreen();

        transform.position = virtualPos;
    }

    public void SetMoving(bool moving)
    {
        isUpdating = moving;
    }
    /// <summary>
    /// ���z�}�E�X�̈ʒu����ʓ��ɐ���
    /// </summary>
    private void ClampPositionToScreen()
    {
        if (cam == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(virtualPos);

        // ��ʂ̒[�ŃN�����v
        screenPos.x = Mathf.Clamp(screenPos.x, 0, cam.pixelWidth);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, cam.pixelHeight);
        screenPos.z = Mathf.Abs(cam.transform.position.z);

        virtualPos = cam.ScreenToWorldPoint(screenPos);
    }

    /// <summary>
    /// ���ۂ̃}�E�X�̃��[���h���W���擾�i�������p�j
    /// </summary>
    private Vector2 GetMouseWorldPosition()
    {
        if (cam == null) return Vector2.zero;

        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(cam.transform.position.z);
        Vector2 worldPos = cam.ScreenToWorldPoint(mp);

        return worldPos;
    }

    /// <summary>
    /// ���z�}�E�X�̌��݈ʒu���擾
    /// </summary>
    public Vector2 GetVirtualMousePosition()
    {
        return virtualPos;
    }
}