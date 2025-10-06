using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    [Header("マウス追従設定")]
    [Tooltip("X座標をマウスに追従させる")]
    [SerializeField] private bool followX = true;
    [Tooltip("Y座標をマウスに追従させる")]
    [SerializeField] private bool followY = false;
    [Tooltip("Y を固定する場合の値")]
    [SerializeField] private float fixedY;

    [Header("移動制限（任意）")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minPos = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 maxPos = new Vector2(8f, 4f);

    [Header("回転設定")]
    [Tooltip("進行方向に回転させる")]
    [SerializeField] private bool rotateToDirection = true;
    [Tooltip("回転の滑らかさ（0で即座に回転）")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("プレイヤー衝突時の力設定")]
    [Tooltip("この速度未満の場合に力を加える")]
    [SerializeField] private float speedThreshold = 5f;
    [Tooltip("法線方向に加える力")]
    [SerializeField] private float boostForce = 10f;

    private Camera mainCamera;
    private Vector3 lastPosition;

    void Start()
    {
        mainCamera = Camera.main;
        fixedY = transform.position.y; // 初期Y座標を保持
        lastPosition = transform.position;
    }

    void Update()
    {
        if (mainCamera == null) return;

        // マウス位置 → ワールド
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
        // プレイヤーとの衝突チェック
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                float playerSpeed = playerRb.velocity.magnitude;

                // プレイヤーの速度が閾値未満の場合
                if (playerSpeed < speedThreshold)
                {
                    // 衝突点から法線方向を取得
                    Vector2 normal = collision.contacts[0].normal;
                    
                    // バーの法線方向（バーから見たプレイヤー方向）に力を加える
                    Vector2 forceDirection = -normal;
                    playerRb.AddForce(forceDirection * boostForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
