using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    [SerializeField] private float minLength = 0.5f;
    [SerializeField] private float maxLength = 5f;
    private SpriteRenderer sr;
    private float originalSpriteWidth;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // �X�v���C�g�̌��̃T�C�Y(�X�P�[��1�̎�)��ۑ�
        originalSpriteWidth = sr.sprite.bounds.size.x;
    }

    public void UpdateArrow(Vector2 barPos, Vector2 ballPos, float ratio)
    {
        Vector2 dir = (barPos - ballPos).normalized;

        // ��]
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // ����
        float newLength = Mathf.Lerp(minLength, maxLength, ratio);
        transform.localScale = new Vector3(newLength, 1, 1);

        // �����i���[�j���{�[���̒��S�ɔz�u
        // �X�v���C�g�̍��[���璆�S�܂ł̋��� = ���̃T�C�Y�̔��� �~ ���݂̃X�P�[��
        float halfWidth = originalSpriteWidth / 2f * newLength;
        Vector2 offsetToRoot = -dir * halfWidth; // �����̋t�����ɃI�t�Z�b�g

        transform.position = ballPos + offsetToRoot;
    }
}