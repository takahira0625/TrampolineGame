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
    private Camera mainCamera; // ������ �J�������L���b�V������ϐ�

    // ������ �n�_�̕ێ����@��ύX ������
    private Vector3 startViewportPos; // �n�_���r���[�|�[�g���W�ŋL��
    private Vector2 startPos;         // ���t���[���X�V�����n�_�̃��[���h���W

    private bool canCreate;

    // ������ Start���\�b�h��ǉ� ������
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
                // ������ �n�_�̃��[���h���W�𖈃t���[���X�V ������
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
                    // ������ �ŏI�I�Ȏn�_�ƏI�_��ݒ� ������
                    startPos = mainCamera.ViewportToWorldPoint(startViewportPos);
                    Vector2 endPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                    List<Vector2> points = new List<Vector2> { startPos, endPos };
                    currentEdgeCollider.points = points.ToArray();
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

        // ������ �n�_�����[���h���W�ƃr���[�|�[�g���W�̗����ŋL�^ ������
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