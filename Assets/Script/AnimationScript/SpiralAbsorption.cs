using UnityEngine;
using System.Collections;

public class SpiralAbsorption : MonoBehaviour
{
    [Header("吸い込み設定")]
    [SerializeField] private float rotationSpeed = 100f; // 回転速度
    [SerializeField] private float moveSpeed = 0.01f; // 中心に近づく速度

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
    /// 渦を巻きながら中心に吸い込まれる処理
    /// </summary>
    private void PerformSpiralAbsorption()
    {
        // 2D平面（Z軸回転）で回転
        transform.RotateAround(center, Vector3.forward, rotationSpeed * Time.deltaTime);

        // 中心に向かって移動
        Vector3 direction = (center - transform.position).normalized;
        transform.position += direction * moveSpeed;

        // 中心に到達したら削除
        if (Vector3.Distance(transform.position, center) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 外部から呼び出し可能な吸い込み開始関数
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