using UnityEngine;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;
    public static BGMManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("BGMManager");
                instance = obj.AddComponent<BGMManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private AudioSource audioSource;
    private float volume = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    /// <summary>
    /// BGM���Đ�����
    /// </summary>
    public void Play(AudioClip clip, bool fadeIn = false, float fadeDuration = 1f)
    {
        if (clip == null) return;

        // ����BGM���Đ����Ȃ牽�����Ȃ�
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        if (fadeIn && audioSource.isPlaying)
        {
            StartCoroutine(FadeOutAndIn(clip, fadeDuration));
        }
        else
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    /// <summary>
    /// BGM���~����
    /// </summary>
    public void Stop(bool fadeOut = false, float fadeDuration = 1f)
    {
        if (fadeOut)
        {
            StartCoroutine(FadeOut(fadeDuration));
        }
        else
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// BGM���ꎞ��~����
    /// </summary>
    public void Pause()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// �ꎞ��~����BGM���ĊJ����
    /// </summary>
    public void Resume()
    {
        audioSource.UnPause();
    }

    /// <summary>
    /// BGM�̉��ʂ�ݒ肷�� (0.0 ~ 1.0)
    /// </summary>
    public void SetVolume(float vol)
    {
        volume = Mathf.Clamp01(vol);
        audioSource.volume = volume;
    }

    /// <summary>
    /// ���݂̉��ʂ��擾����
    /// </summary>
    public float GetVolume()
    {
        return volume;
    }

    /// <summary>
    /// BGM���Đ������ǂ���
    /// </summary>
    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    private System.Collections.IEnumerator FadeOut(float duration)
    {
        float startVol = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = volume;
    }

    private System.Collections.IEnumerator FadeOutAndIn(AudioClip newClip, float duration)
    {
        yield return StartCoroutine(FadeOut(duration));
        audioSource.clip = newClip;
        audioSource.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, elapsed / duration);
            yield return null;
        }
    }
}