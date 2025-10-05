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
    [SerializeField] private float rotationSpeed = 1f;

    [Header("�ᑬ�v���C���[�o�E���h�ݒ�")]
    [Tooltip("�ᑬ�̃v���C���[��e���@�\ ON/OFF")]
    [SerializeField] private bool enableLowSpeedBounce = true;
    [Tooltip("���ꖢ���̑��x�ŏՓ˂�����o�E���h������臒l (m/s)")]
    [SerializeField] private float speedThreshold = 2f;
    [Tooltip("�e���ۂɗ^����@���������x (m/s)")]
    [SerializeField] private float bounceSpeed = 6f;
    [Tooltip("�ڐG�_�̖@�����g�p�iOFF�Ȃ� Bar �̌������琄��j")]
    [SerializeField] private bool useContactNormal = true;
    [Tooltip("Bar �̌������琄�肷��ꍇ�� transform.right ��@���Ƃ݂Ȃ��i�ʏ�� up�j")]
    [SerializeField] private bool useRightAsNormal = false;
    [Tooltip("�Z�o�����@���𔽓]�i�������t�ɂȂ����ꍇ�̐ؑ֗p�j")]
    [SerializeField] private bool invertNormal = false;
    [Tooltip("���ɗ��������֓����Ă���ꍇ�͒e���Ȃ�")]
    [SerializeField] private bool skipIfAlreadyMovingAway = true;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enableLowSpeedBounce) return;
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.rigidbody;
        if (playerRb == null) return;

        float currentSpeed = playerRb.velocity.magnitude;
        if (currentSpeed >= speedThreshold) return; // ���ȏ�Ȃ牽�����Ȃ�

        // �@���v�Z
        Vector2 normal;
        if (useContactNormal)
        {
            // Bar ���R���C�_�[ �� Player ���R���C�_�[�����̖@���i�v���C���[�𗣂������ɂقڈ�v�j
            ContactPoint2D cp = collision.GetContact(0);
            normal = cp.normal;
        }
        else
        {
            // Bar �̌����ڂ́g�ʂ̌����h����Z�o
            normal = useRightAsNormal ? (Vector2)transform.right : (Vector2)transform.up;

            // �v���C���[�̈ʒu�����֌����Ă��Ȃ���Δ��]�i���������𐳂Ɂj
            Vector2 toPlayer = (Vector2)playerRb.position - (Vector2)transform.position;
            if (Vector2.Dot(normal, toPlayer) < 0f)
                normal = -normal;
        }

        if (invertNormal) normal = -normal;
        normal.Normalize();

        // ���ɗ��������֏\�������Ă���Ȃ�X�L�b�v�i�C�Ӂj
        if (skipIfAlreadyMovingAway && Vector2.Dot(playerRb.velocity, normal) > 0f)
        {
            return;
        }

        playerRb.velocity = normal * bounceSpeed;

        Debug.Log($"Bar LowSpeedBounce: speed={currentSpeed:F2} -> {bounceSpeed:F2}, normal={normal}");
    }
}
