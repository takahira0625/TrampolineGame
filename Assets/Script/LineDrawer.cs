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
    // �ǉ�����: �Z���������e��
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

                    // �u������: �}�E�X�A�b�v���� Effector / �@���ݒ�܂����������S��
                    // �iOnMouseUp ���̊Y���u���b�N�����������j
                    PlatformEffector2D effector = currentLineRenderer.GetComponent<PlatformEffector2D>();
                    if (effector != null)
                    {
                        Vector2 direction = endPos - startPos;
                        if (direction.sqrMagnitude > 1e-6f)
                        {
                            // ���ɐ����Ȗ@�����擾
                            Vector2 normal = Vector2.Perpendicular(direction).normalized;

                            // ��������������Ȃ甽�]�i������D��̈��������j
                            if (normal.y < 0f)
                            {
                                normal = -normal;
                            }

                            // Trampoline �ɖ@����ʒm
                            Trampoline trampoline = currentLineRenderer.GetComponent<Trampoline>();
                            if (trampoline != null)
                            {
                                trampoline.SetNormal(normal);
                            }

                            // �قڐ����`�΂�: Effector �L���A�قڐ���: Effector ������
                            bool nearlyVertical = Mathf.Abs(normal.y) < 0.01f;
                            effector.enabled = !nearlyVertical;

                            if (effector.enabled)
                            {
                                effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, normal);
                            }
                        }
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

    // �u������: CheckLineValidity ���p�x�����Ȃ��ł�
    private void CheckLineValidity(Vector2 endPos)
    {
        Vector2 direction = endPos - startPos;

        // 1) �ŏ����`�F�b�N�̂�
        if (direction.sqrMagnitude < minLength * minLength)
        {
            canCreate = false;
        }
        else
        {
            // 2) �d�Ȃ�i����/���g�����|�����j�`�F�b�N�͏]���ǂ���
            currentEdgeCollider.enabled = false;
            RaycastHit2D hit = Physics2D.Linecast(startPos, endPos, trampolineLayer);
            currentEdgeCollider.enabled = true;
            canCreate = (hit.collider == null);
        }

        Color colorToSet = canCreate ? validColor : invalidColor;
        currentLineRenderer.startColor = colorToSet;
        currentLineRenderer.endColor = colorToSet;
    }

}