using UnityEngine;

public class SkyBeans : MonoBehaviour
{
    [Header("�ݒ�")]
    public float activationRadius = 1f;                    // �N���b�N�����͈́i���a�j
    public GameObject rectanglePrefab;                     // �������钷���`�v���n�u
    public GameObject rangeIndicatorPrefab;                // �͈͕\���p�̉~�X�v���C�g�v���n�u

    private GameObject currentRectangle = null;            // ���ݕ\������Ă��钷���`�i1�����j
    private GameObject rangeIndicatorInstance = null;      // �\�����͈̔̓C���W�P�[�^�[�i�~�j
    private SpriteRenderer spriteRenderer;                 // SkyBeans���g�̃X�v���C�g�����_���[

    /// <summary>
    /// �����������BSpriteRenderer�擾�Ɣ͈͕\���̏��������s���B
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ShowRangeIndicator();
    }

    /// <summary>
    /// �w�肳�ꂽ�͈̓C���W�P�[�^�[��\�����A�e�X�P�[���̉e�����󂯂Ȃ��悤�␳���ăX�P�[�����O����B
    /// </summary>
    private void ShowRangeIndicator()
    {
        if (rangeIndicatorPrefab == null) return;

        rangeIndicatorInstance = Instantiate(
            rangeIndicatorPrefab,
            transform.position,
            Quaternion.identity,
            transform // SkyBeans �̎q�ɂ���
        );

        float diameter = activationRadius * 2f;

        // �e�X�P�[���̋t�����|���ĕ␳�i���[���h�X�P�[���Ő�����������j
        Vector3 parentScale = transform.lossyScale;
        Vector3 inverseScale = new Vector3(
            1f / parentScale.x,
            1f / parentScale.y,
            1f / parentScale.z
        );

        // �X�P�[����␳���Č����ڂ̃T�C�Y�����킹��
        rangeIndicatorInstance.transform.localScale = Vector3.Scale(
            new Vector3(diameter, diameter, 1f),
            inverseScale
        );
    }

    /// <summary>
    /// �w�肳�ꂽ���W������SkyBeans�̃N���b�N�͈͓��ɂ��邩�𔻒肷��B
    /// </summary>
    public bool IsWithinRange(Vector2 clickPosition)
    {
        return Vector2.Distance(transform.position, clickPosition) <= activationRadius;
    }

    /// <summary>
    /// ���݁A�����`���\������Ă��邩�ǂ�����Ԃ��B
    /// </summary>
    public bool IsRectangleActive()
    {
        return currentRectangle != null;
    }

    /// <summary>
    /// ���݂̒����`�icurrentRectangle�j���폜����B
    /// </summary>
    public void DestroyCurrentRectangle()
    {
        if (currentRectangle != null)
        {
            Destroy(currentRectangle);
            currentRectangle = null;
        }
    }

    /// <summary>
    /// �N���b�N���W�Ɍ������Ē����`�𐶐�����B���ɂ���΍폜���Ēu��������B
    /// </summary>
    public void GenerateNewRectangle(Vector2 targetPosition)
    {
        DestroyCurrentRectangle(); // �Â������`������

        Vector2 origin = transform.position;
        Vector2 direction = (targetPosition - origin);
        float length = direction.magnitude;
        Vector2 midPoint = origin + direction * 0.5f; // ���S�ʒu���v�Z

        // �����`�𐶐����Č����E�傫����ݒ�
        currentRectangle = Instantiate(rectanglePrefab, midPoint, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentRectangle.transform.rotation = Quaternion.Euler(0, 0, angle);
        currentRectangle.transform.localScale = new Vector3(length, 0.2f, 1f);
    }

    /// <summary>
    /// SkyBeans�{�̂̌����ڂ������\������i�n�C���C�g�j�B
    /// </summary>
    public void SetHighlight(bool highlight)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = highlight ? Color.red : Color.white;
    }

    /// <summary>
    /// Scene�r���[�ŃI�u�W�F�N�g��I�𒆂ɁA�N���b�N�͈͂�Ԃ��~�ŉ�������B
    /// �� ���s���łȂ��Ă��\�������B�f�o�b�O�p�B
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}