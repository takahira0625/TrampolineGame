using UnityEngine;

public class SetActiveSlowZone : MonoBehaviour
{
    private Renderer[] renderers;
    private bool isPlaying = false;

    [SerializeField, Header("SlowZone�L�����̌��ʉ�")]
    private AudioClip activateLoopSE;

    private AudioSource audioSource;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        bool isActive = InputManager.IsLeftClickPressed();

        // ���N���b�N�� �� �L�����{���[�v�Đ�
        if (isActive)
        {
            SetRenderersEnabled(true);

            if (!isPlaying && activateLoopSE != null)
            {
                audioSource.clip = activateLoopSE;
                audioSource.Play();
                isPlaying = true;
            }
        }
        // �������� �� �������{��~
        else
        {
            SetRenderersEnabled(false);

            if (isPlaying)
            {
                audioSource.Stop();
                isPlaying = false;
            }
        }
    }

    private void SetRenderersEnabled(bool enabled)
    {
        foreach (var rend in renderers)
        {
            rend.enabled = enabled;
        }
    }
}
