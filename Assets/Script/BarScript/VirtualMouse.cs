using UnityEngine;

/// <summary>
/// Input.GetAxis を使って仮想マウスを制御
/// 画面端でもマウス入力が続く
/// </summary>
public class VirtualMouse : MonoBehaviour
{
    [Header("表示設定")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("マウス感度")]
    [SerializeField] private float mouseSensitivity = 1f; // マウス入力の感度

    [Header("追従速度")]
    [SerializeField, Range(0f, 1f)] private float normalFollowSpeed = 1f; // 通常の追従速度
    [SerializeField, Range(0f, 1f)] private float leftClickFollowSpeed = 0.1f; // 左クリック中の速度

    private Vector2 virtualPos; // ワールド座標での仮想マウス位置
    private Camera cam;
    private bool isUpdating = true;

    void Awake()
    {
        cam = Camera.main;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // スプライトを前面に表示
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Foreground";
            spriteRenderer.sortingOrder = 100;
        }

        // マウスカーソルを非表示
        Cursor.visible = false;
    }

    void Start()
    {
        // 初期位置をマウスの現在位置に合わせる
        virtualPos = GetMouseWorldPosition();
        transform.position = virtualPos;
    }

    void Update()
    {
        if (!isUpdating) return;
        // Input.GetAxis は画面端でも値が続く
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        Vector2 mouseDelta = new Vector2(mouseX, mouseY);

        // 左クリック中はゆっくり移動
        if (Input.GetMouseButton(0))
        {
            virtualPos += mouseDelta * leftClickFollowSpeed;
        }
        else
        {
            // 通常はマウス入力をそのまま適用
            virtualPos += mouseDelta * normalFollowSpeed;
        }

        // ワールド座標をスクリーン座標の範囲内に制限（画面外に出ないようにする）
        ClampPositionToScreen();

        transform.position = virtualPos;
    }

    public void SetMoving(bool moving)
    {
        isUpdating = moving;
    }
    /// <summary>
    /// 仮想マウスの位置を画面内に制限
    /// </summary>
    private void ClampPositionToScreen()
    {
        if (cam == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(virtualPos);

        // 画面の端でクランプ
        screenPos.x = Mathf.Clamp(screenPos.x, 0, cam.pixelWidth);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, cam.pixelHeight);
        screenPos.z = Mathf.Abs(cam.transform.position.z);

        virtualPos = cam.ScreenToWorldPoint(screenPos);
    }

    /// <summary>
    /// 実際のマウスのワールド座標を取得（初期化用）
    /// </summary>
    private Vector2 GetMouseWorldPosition()
    {
        if (cam == null) return Vector2.zero;

        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(cam.transform.position.z);
        Vector2 worldPos = cam.ScreenToWorldPoint(mp);

        return worldPos;
    }

    /// <summary>
    /// 仮想マウスの現在位置を取得
    /// </summary>
    public Vector2 GetVirtualMousePosition()
    {
        return virtualPos;
    }
}