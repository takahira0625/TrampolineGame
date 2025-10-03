using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [Header("オブジェクト参照")]
    public GameObject linePrefab;

    [Header("描画設定")]
    [SerializeField] private LayerMask trampolineLayer;
    [SerializeField] private Color validColor = Color.white;
    [SerializeField] private Color invalidColor = Color.red;
    [SerializeField][Range(0, 90)] private float maxAngle = 45f;
    // 追加推奨: 短すぎる線を弾く
    [SerializeField][Min(0f)] private float minLength = 0.1f;


    private LineRenderer currentLineRenderer;
    private EdgeCollider2D currentEdgeCollider;
    private Camera mainCamera;

    private Vector3 startViewportPos;
    private Vector2 startPos;

    private bool canCreate;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }

        if (Input.GetMouseButton(0))
        {
            if (currentLineRenderer != null)
            {
                startPos = mainCamera.ViewportToWorldPoint(startViewportPos);
                currentLineRenderer.SetPosition(0, startPos);

                Vector2 endPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                currentLineRenderer.SetPosition(1, endPos);

                CheckLineValidity(endPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentLineRenderer != null)
            {
                if (canCreate)
                {
                    startPos = mainCamera.ViewportToWorldPoint(startViewportPos);
                    Vector2 endPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                    List<Vector2> points = new List<Vector2> { startPos, endPos };
                    currentEdgeCollider.points = points.ToArray();

                    // 置き換え: マウスアップ時の Effector / 法線設定まわりを少し堅牢化
                    // （OnMouseUp 内の該当ブロックを書き換え）
                    PlatformEffector2D effector = currentLineRenderer.GetComponent<PlatformEffector2D>();
                    if (effector != null)
                    {
                        Vector2 direction = endPos - startPos;
                        if (direction.sqrMagnitude > 1e-6f)
                        {
                            // 線に垂直な法線を取得
                            Vector2 normal = Vector2.Perpendicular(direction).normalized;

                            // 上向き成分が負なら反転（上向き優先の一方向判定）
                            if (normal.y < 0f)
                            {
                                normal = -normal;
                            }

                            // Trampoline に法線を通知
                            Trampoline trampoline = currentLineRenderer.GetComponent<Trampoline>();
                            if (trampoline != null)
                            {
                                trampoline.SetNormal(normal);
                            }

                            // ほぼ水平～斜め: Effector 有効、ほぼ垂直: Effector 無効化
                            bool nearlyVertical = Mathf.Abs(normal.y) < 0.01f;
                            effector.enabled = !nearlyVertical;

                            if (effector.enabled)
                            {
                                effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, normal);
                            }
                        }
                    }

                    // ▲▲▲ ここまで ▲▲▲
                }
                else
                {
                    Destroy(currentLineRenderer.gameObject);
                }

                currentLineRenderer = null;
                currentEdgeCollider = null;
            }
        }
    }

    private void CreateLine()
    {
        GameObject lineGO = Instantiate(linePrefab);
        lineGO.layer = LayerMask.NameToLayer("Trampoline");

        currentLineRenderer = lineGO.GetComponent<LineRenderer>();
        currentEdgeCollider = lineGO.GetComponent<EdgeCollider2D>();

        startPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        startViewportPos = mainCamera.WorldToViewportPoint(startPos);

        currentLineRenderer.positionCount = 2;
        currentLineRenderer.SetPosition(0, startPos);
        currentLineRenderer.SetPosition(1, startPos);

        canCreate = true;
    }

    // Block タグとの重なりも弾くように拡張
    private void CheckLineValidity(Vector2 endPos)
    {
        Vector2 direction = endPos - startPos;

        // 1) 長さチェック
        if (direction.sqrMagnitude < minLength * minLength)
        {
            canCreate = false;
        }
        else
        {
            // 自身の EdgeCollider を一時無効化
            currentEdgeCollider.enabled = false;

            // 2) 既存トランポリンとの重なり
            RaycastHit2D hitTrampoline = Physics2D.Linecast(startPos, endPos, trampolineLayer);

            // 3) Block タグとの重なり検出
            bool hitBlock = false;
            RaycastHit2D[] hits = Physics2D.LinecastAll(startPos, endPos);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider != null && hits[i].collider.CompareTag("Block"))
                {
                    hitBlock = true;
                    break;
                }
            }

            currentEdgeCollider.enabled = true;

            canCreate = (hitTrampoline.collider == null) && !hitBlock;
        }

        Color colorToSet = canCreate ? validColor : invalidColor;
        currentLineRenderer.startColor = colorToSet;
        currentLineRenderer.endColor = colorToSet;
    }
}