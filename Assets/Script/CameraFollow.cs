using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // �Ǐ]����^�[�Q�b�g�i�v���C���[�j
    public Transform target;

    // �J�����̒Ǐ]�̊��炩���i�l���������قǊ��炩�j
    public float smoothSpeed = 0.125f;

    // �J�����ƃ^�[�Q�b�g�̏�������
    private Vector3 offset;

    void Start()
    {
        // �Q�[���J�n���̃J�����ƃ^�[�Q�b�g�̈ʒu�֌W���L��
        offset = transform.position - target.position;
    }

    // �S�Ă�Update�������I�������ɌĂ΂��
    void LateUpdate()
    {
        // �^�[�Q�b�g���ݒ肳��Ă��Ȃ���Ή������Ȃ�
        if (target == null)
        {
            return;
        }

        // �^�[�Q�b�g�̈ʒu�ɏ����̋����𑫂��āA�J�����̖ڕW�ʒu���v�Z
        Vector3 desiredPosition = target.position + offset;

        // ���݈ʒu����ڕW�ʒu�֊��炩�Ɉړ�������
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // �J�����̈ʒu���X�V
        transform.position = smoothedPosition;
    }
}