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
    [SerializeField] private float rotationSpeed = 1f;

    [Header("低速プレイヤーバウンド設定")]
    [Tooltip("低速のプレイヤーを弾く機能 ON/OFF")]
    [SerializeField] private bool enableLowSpeedBounce = true;
    [Tooltip("これ未満の速度で衝突したらバウンドさせる閾値 (m/s)")]
    [SerializeField] private float speedThreshold = 2f;
    [Tooltip("弾く際に与える法線方向速度 (m/s)")]
    [SerializeField] private float bounceSpeed = 6f;
    [Tooltip("接触点の法線を使用（OFFなら Bar の向きから推定）")]
    [SerializeField] private bool useContactNormal = true;
    [Tooltip("Bar の向きから推定する場合に transform.right を法線とみなす（通常は up）")]
    [SerializeField] private bool useRightAsNormal = false;
    [Tooltip("算出した法線を反転（方向が逆になった場合の切替用）")]
    [SerializeField] private bool invertNormal = false;
    [Tooltip("既に離れる方向へ動いている場合は弾かない")]
    [SerializeField] private bool skipIfAlreadyMovingAway = true;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enableLowSpeedBounce) return;
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.rigidbody;
        if (playerRb == null) return;

        float currentSpeed = playerRb.velocity.magnitude;
        if (currentSpeed >= speedThreshold) return; // 一定以上なら何もしない

        // 法線計算
        Vector2 normal;
        if (useContactNormal)
        {
            // Bar 側コライダー → Player 側コライダー方向の法線（プレイヤーを離す方向にほぼ一致）
            ContactPoint2D cp = collision.GetContact(0);
            normal = cp.normal;
        }
        else
        {
            // Bar の見た目の“面の向き”から算出
            normal = useRightAsNormal ? (Vector2)transform.right : (Vector2)transform.up;

            // プレイヤーの位置方向へ向いていなければ反転（離れる方向を正に）
            Vector2 toPlayer = (Vector2)playerRb.position - (Vector2)transform.position;
            if (Vector2.Dot(normal, toPlayer) < 0f)
                normal = -normal;
        }

        if (invertNormal) normal = -normal;
        normal.Normalize();

        // 既に離れる方向へ十分動いているならスキップ（任意）
        if (skipIfAlreadyMovingAway && Vector2.Dot(playerRb.velocity, normal) > 0f)
        {
            return;
        }

        playerRb.velocity = normal * bounceSpeed;

        Debug.Log($"Bar LowSpeedBounce: speed={currentSpeed:F2} -> {bounceSpeed:F2}, normal={normal}");
    }
}
