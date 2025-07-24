using UnityEngine;

public class SkyBeansManager : MonoBehaviour
{
    //�X�e�[�W��̂��ׂĂ�SkyBeans
    private SkyBeans[] allSkyBeans;
    //���ݗL����SkyBeans���
    private SkyBeans currentHovered = null;

    private void Start()
    {
        allSkyBeans = FindObjectsOfType<SkyBeans>();
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SkyBeans hovered = GetNearestSkyBeans(mousePos);

        // �n�C���C�g�؂�ւ�
        if (hovered != currentHovered)
        {
            if (currentHovered != null)
                currentHovered.SetHighlight(false);

            currentHovered = hovered;

            if (currentHovered != null)
                currentHovered.SetHighlight(true);
        }

        // ���N���b�N�Œ����`����
        if (Input.GetMouseButtonDown(0) && currentHovered != null)
        {
            currentHovered.GenerateNewRectangle(mousePos);
        }
    }

    /// <summary>
    /// �͈͓���SkyBeans�̒�����ł��߂����̂�Ԃ�
    /// </summary>
    private SkyBeans GetNearestSkyBeans(Vector2 clickPosition)
    {
        SkyBeans closest = null;
        float minDistance = float.MaxValue;

        foreach (SkyBeans sb in allSkyBeans)
        {
            if (!sb.IsWithinRange(clickPosition)) continue;

            float dist = Vector2.Distance(sb.transform.position, clickPosition);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = sb;
            }
        }

        return closest;
    }
}