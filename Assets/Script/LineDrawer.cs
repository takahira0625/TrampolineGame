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

                    // Platform Effectorの角度を計算して設定
                    PlatformEffector2D effector = currentLineRenderer.GetComponent<PlatformEffector2D>();
                    if (effector != null)
                    {
                        // 線の方向ベクトルから法線（垂直なベクトル）を計算
                        Vector2 direction = endPos - startPos;
                        Vector2 normal = Vector2.Perpendicular(direction).normalized;

                        // 法線が下を向いている場合は反転させる
                        if (normal.y < 0)
                        {
                            normal = -normal;
                        }

                        // Trampoline コンポーネントに法線を渡す
                        Trampoline trampoline = currentLineRenderer.GetComponent<Trampoline>();
                        if (trampoline != null)
                        {
                            trampoline.SetNormal(normal);
                        }

                        // ワールドの真上（0, 1）と線の法線との間の角度を計算し、オフセットとして設定
                        effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, normal);
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

    private void CheckLineValidity(Vector2 endPos)
    {
        Vector2 direction = endPos - startPos;
        if (direction.x == 0)
        {
            canCreate = false;
        }
        else
        {
            float angle = Mathf.Abs(Mathf.Atan(direction.y / direction.x) * Mathf.Rad2Deg);
            canCreate = angle <= maxAngle;
        }

        if (canCreate)
        {
            currentEdgeCollider.enabled = false;
            RaycastHit2D hit = Physics2D.Linecast(startPos, endPos, trampolineLayer);
            currentEdgeCollider.enabled = true;

            if (hit.collider != null)
            {
                canCreate = false;
            }
        }

        Color colorToSet = canCreate ? validColor : invalidColor;
        currentLineRenderer.startColor = colorToSet;
        currentLineRenderer.endColor = colorToSet;
    }
}