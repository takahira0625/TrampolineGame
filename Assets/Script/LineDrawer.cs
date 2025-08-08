using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�Q��")]
    public GameObject linePrefab;

    [Header("�`��ݒ�")]
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

                    // Platform Effector�̊p�x���v�Z���Đݒ�
                    PlatformEffector2D effector = currentLineRenderer.GetComponent<PlatformEffector2D>();
                    if (effector != null)
                    {
                        // ���̕����x�N�g������@���i�����ȃx�N�g���j���v�Z
                        Vector2 direction = endPos - startPos;
                        Vector2 normal = Vector2.Perpendicular(direction).normalized;

                        // �@�������������Ă���ꍇ�͔��]������
                        if (normal.y < 0)
                        {
                            normal = -normal;
                        }

                        // Trampoline �R���|�[�l���g�ɖ@����n��
                        Trampoline trampoline = currentLineRenderer.GetComponent<Trampoline>();
                        if (trampoline != null)
                        {
                            trampoline.SetNormal(normal);
                        }

                        // ���[���h�̐^��i0, 1�j�Ɛ��̖@���Ƃ̊Ԃ̊p�x���v�Z���A�I�t�Z�b�g�Ƃ��Đݒ�
                        effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, normal);
                    }
                    // ������ �����܂� ������
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