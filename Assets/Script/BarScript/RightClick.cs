using System.Collections;
using UnityEngine;

public class RightClick : MonoBehaviour
{
    [SerializeField] private VirtualMouse virtualMouse;
    [SerializeField] private BarMovement barFollow;

    [Header("前進距離と時間")]
    [SerializeField] private float forwardDistance = 20.0f;
    [SerializeField] private float forwardTime = 0.1f;
    [SerializeField] private float waitTime = 0.05f;
    [SerializeField] private float returnTime = 0.3f;

    [Header("反射設定")]
    [SerializeField] private float reboundCoefficient = 0.9f; // 反発係数
    [SerializeField] private float pushPower = 30.0f; // 突き出し時の加速力

    private bool isMoving = false;
    public bool IsMoving => isMoving;
    private Vector2 originalPosition;
    private Quaternion startRotation;

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isMoving)//右クリック
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

        // --- 戻る ---
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
        if (virtualMouse != null) virtualMouse.SetMoving(true); // ←ここで再開
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (ballRb == null) return;

        // 衝突点の法線
        Vector2 normal = collision.contacts[0].normal;

        if (isMoving)
        {
            // 突き出し中：バーの前方向に強く飛ばす
            Vector2 forward = transform.up.normalized;
            ballRb.velocity = forward * pushPower;
            Debug.Log($"Pushed: forward={forward}, v={ballRb.velocity}");
        }
        else
        {
            // 静止中：自然反射（反発係数付き）
            Vector2 reflected = Vector2.Reflect(ballRb.velocity, normal);
            ballRb.velocity = reflected * reboundCoefficient;
            Debug.Log($"Reflected: normal={normal}, v={ballRb.velocity}");
        }
    }


}
