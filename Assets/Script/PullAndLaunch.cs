using UnityEngine;
using TMPro;

public class PullAndLaunch : MonoBehaviour
{
    [Header("力の調整")]
    [SerializeField] private float launchPowerMultiplier = 5f;
    [SerializeField] private float maxDragDistance = 3f;

    [Header("物理設定")]
    [SerializeField] private float gravityValue = 1f;

    [Header("矢印の見た目調整")]
    [SerializeField] private float arrowLengthMultiplier = 1.0f;
    [SerializeField] private float arrowThickness = 1.0f;

    [Header("ゴール設定")]
    [SerializeField] private GameObject goalTextObject;

    [Header("オブジェクト参照")]
    [SerializeField] private Transform arrow;

    // ▼▼▼ この行を追加 ▼▼▼
    [SerializeField] public LineDrawer lineDrawer;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 startPosition;
    private Vector2 dragVector;
    private bool isDragging = false;
    private bool isGrounded = true;
    private bool isGoal = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        arrow.gameObject.SetActive(false);
        rb.gravityScale = 0;
        goalTextObject.SetActive(false);

        // ▼▼▼ この行を追加 ▼▼▼
        // ゲーム開始時は線を引けないようにする
        if (lineDrawer != null) lineDrawer.enabled = false;
    }

    void Update()
    {
        if (isGoal) return;

        // プレイヤーが停止していて、地面に接触しているとき
        if (rb.velocity.magnitude < 0.1f && isGrounded)
        {
            // 線を引けないようにして重力も戻す
            if (lineDrawer != null && lineDrawer.enabled)
            {
                lineDrawer.enabled = false;
                rb.gravityScale = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                arrow.gameObject.SetActive(true);
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                arrow.gameObject.SetActive(false);
                Launch();
            }
        }

        if (isDragging)
        {
            UpdateArrow();
        }
    }

    private void Launch()
    {
        rb.gravityScale = gravityValue;

        // 発射後に線を引けるようにする
        if (lineDrawer != null) lineDrawer.enabled = true;

        Vector2 launchVector = startPosition - (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // 引っ張り距離が最大値を超えたら制限
        if (launchVector.magnitude > maxDragDistance)
        {
            launchVector = launchVector.normalized * maxDragDistance;
        }

        // Rigidbody2D に力を加えて発射
        rb.AddForce(launchVector * launchPowerMultiplier, ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("普通の床に着地！");
        }
        else if (collision.gameObject.CompareTag("Trampoline"))
        {
            Debug.Log("トランポリンに着地！");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Trampoline"))
        {
            isGrounded = false;
        }
    }

    // 他メソッドは変更なし
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGoal) return;
        if (other.gameObject.CompareTag("GoalTop"))
        {
            AchieveGoal();
        }
    }

    private void AchieveGoal()
    {
        isGoal = true;
        goalTextObject.SetActive(true);
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        UnityEngine.Debug.Log("GOAL!");
    }

    private void UpdateArrow()
    {
        Vector2 currentMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        dragVector = currentMousePosition - startPosition;

        if (dragVector.magnitude > maxDragDistance)
        {
            dragVector = dragVector.normalized * maxDragDistance;
        }

        arrow.position = rb.position;
        float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle);
        arrow.localScale = new Vector3(dragVector.magnitude * arrowLengthMultiplier, arrowThickness, 1);
    }
}