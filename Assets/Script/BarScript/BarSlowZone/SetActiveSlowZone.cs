using UnityEngine;

public class SetActiveSlowZone : MonoBehaviour
{
    private Renderer[] renderers;
    private bool isPlaying = false;

    [SerializeField, Header("SlowZone有効時の効果音")]
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

        // 左クリック中 → 有効化＋ループ再生
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
        // 離したら → 無効化＋停止
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
