using System.Collections;
using UnityEngine;

public class RightClick : MonoBehaviour
{
    [SerializeField] private VirtualMouse virtualMouse;
    [SerializeField] private BarMovement barFollow;

    [Header("�O�i�����Ǝ���")]
    [SerializeField] private float forwardDistance = 20.0f;
    [SerializeField] private float forwardTime = 0.1f;
    [SerializeField] private float waitTime = 0.05f;
    [SerializeField] private float returnTime = 0.3f;

    [Header("���ːݒ�")]
    [SerializeField] private float reboundCoefficient = 0.9f; // �����W��
    [SerializeField] private float pushPower = 30.0f; // �˂��o�����̉�����

    private bool isMoving = false;
    public bool IsMoving => isMoving;
    private Vector2 originalPosition;
    private Quaternion startRotation;

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isMoving)//�E�N���b�N
        {
            startRotation = transform.rotation;
            StartCoroutine(MoveForwardAndBack());
        }
    }

    private IEnumerator MoveForwardAndBack()
    {
        if (barFollow != null) barFollow.stopFollow = true;
        if (virtualMouse != null) virtualMouse.SetMoving(false);

        isMoving = true;
        originalPosition = transform.position;

        Vector2 forward = (startRotation * Vector2.up).normalized;
        Vector2 targetForwardPos = originalPosition + forward.normalized * forwardDistance;

        float elapsed = 0f;
        while (elapsed < forwardTime)
        {
            transform.position = Vector2.Lerp(originalPosition, targetForwardPos, elapsed / forwardTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetForwardPos;

        // --- �߂� ---
        elapsed = 0f;
        while (elapsed < returnTime)
        {
            transform.position = Vector2.Lerp(targetForwardPos, originalPosition, elapsed / returnTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        isMoving = false;
        if (barFollow != null) barFollow.stopFollow = false;
        if (virtualMouse != null) virtualMouse.SetMoving(true); // �������ōĊJ
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (ballRb == null) return;

        // �Փ˓_�̖@��
        Vector2 normal = collision.contacts[0].normal;

        if (isMoving)
        {
            // �˂��o�����F�o�[�̑O�����ɋ�����΂�
            Vector2 forward = transform.up.normalized;
            ballRb.velocity = forward * pushPower;
            Debug.Log($"Pushed: forward={forward}, v={ballRb.velocity}");
        }
        else
        {
            // �Î~���F���R���ˁi�����W���t���j
            Vector2 reflected = Vector2.Reflect(ballRb.velocity, normal);
            ballRb.velocity = reflected * reboundCoefficient;
            Debug.Log($"Reflected: normal={normal}, v={ballRb.velocity}");
        }
    }


}
