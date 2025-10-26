using UnityEngine;
using System.Collections;

public class SpiralAbsorption : MonoBehaviour
{
    [Header("�z�����ݐݒ�")]
    [SerializeField] private float rotationSpeed = 100f; // ��]���x
    [SerializeField] private float moveSpeed = 0.01f; // ���S�ɋ߂Â����x

    private Vector3 center;
    private bool isAbsorbing = false;

    

    void Update()
    {
        if (isAbsorbing)
        {
            PerformSpiralAbsorption();
        }
    }

    /// <summary>
    /// �Q�������Ȃ��璆�S�ɋz�����܂�鏈��
    /// </summary>
    private void PerformSpiralAbsorption()
    {
        // 2D���ʁiZ����]�j�ŉ�]
        transform.RotateAround(center, Vector3.forward, rotationSpeed * Time.deltaTime);

        // ���S�Ɍ������Ĉړ�
        Vector3 direction = (center - transform.position).normalized;
        transform.position += direction * moveSpeed;

        // ���S�ɓ��B������폜
        if (Vector3.Distance(transform.position, center) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �O������Ăяo���\�ȋz�����݊J�n�֐�
    /// </summary>
    public void StartAbsorption(Vector2 targetCenter)
    {
        center = targetCenter;
        isAbsorbing = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }
}