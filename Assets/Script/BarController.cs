using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    [Header("�}�E�X�Ǐ]�ݒ�")]
    [Tooltip("X���W���}�E�X�ɒǏ]������")]
    [SerializeField] private bool followX = true;
    [Tooltip("Y���W���}�E�X�ɒǏ]������")]
    [SerializeField] private bool followY = false;
    [Tooltip("Y ���Œ肷��ꍇ�̒l")]
    [SerializeField] private float fixedY;

    [Header("�ړ������i�C�Ӂj")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minPos = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 maxPos = new Vector2(8f, 4f);

    [Header("��]�ݒ�")]
    [Tooltip("�i�s�����ɉ�]������")]
    [SerializeField] private bool rotateToDirection = true;
    [Tooltip("��]�̊��炩���i0�ő����ɉ�]�j")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("�v���C���[�Փˎ��̗͐ݒ�")]
    [Tooltip("���̑��x�����̏ꍇ�ɗ͂�������")]
    [SerializeField] private float speedThreshold = 5f;
    [Tooltip("�@�������ɉ������")]
    [SerializeField] private float boostForce = 10f;

    private Camera mainCamera;
    private Vector3 lastPosition;

    void Start()
    {
        mainCamera = Camera.main;
        fixedY = transform.position.y; // ����Y���W��ێ�
        lastPosition = transform.position;
    }

    void Update()
    {
        if (mainCamera == null) return;

        // �}�E�X�ʒu �� ���[���h
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        Vector3 targetPos = transform.position;

        if (followX) targetPos.x = worldPos.x;
        if (followY) targetPos.y = worldPos.y; else targetPos.y = fixedY;

        if (useBounds)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minPos.x, maxPos.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minPos.y, maxPos.y);
        }

        if (rotateToDirection)
        {
            Vector3 direction = targetPos - lastPosition;
            if (direction.sqrMagnitude > 0.0001f)
            {
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (rotationSpeed > 0f)
                {
                    float currentAngle = transform.eulerAngles.z;
                    float newAngle = Mathf.LerpAngle(currentAngle, targetAngle - 90f, rotationSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, targetAngle - 90f);
                }
            }
        }

        lastPosition = transform.position;
        transform.position = targetPos;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // �v���C���[�Ƃ̏Փ˃`�F�b�N
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                float playerSpeed = playerRb.velocity.magnitude;

                // �v���C���[�̑��x��臒l�����̏ꍇ
                if (playerSpeed < speedThreshold)
                {
                    // �Փ˓_����@���������擾
                    Vector2 normal = collision.contacts[0].normal;
                    
                    // �o�[�̖@�������i�o�[���猩���v���C���[�����j�ɗ͂�������
                    Vector2 forceDirection = -normal;
                    playerRb.AddForce(forceDirection * boostForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
