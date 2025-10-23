using UnityEngine;

public class SetActiveSlowZone : MonoBehaviour
{
    private Renderer[] renderers;

    void Awake()
    {
        // �������g�ƑS�Ă̎q�I�u�W�F�N�g��Renderer���擾
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SetRenderersEnabled(true);
        }
        else
        {
            SetRenderersEnabled(false);
        }
    }

    // Renderer���ꊇ�ŗL��/�����ɂ���֐�
    private void SetRenderersEnabled(bool enabled)
    {
        foreach (var rend in renderers)
        {
            rend.enabled = enabled;
        }
    }
}
