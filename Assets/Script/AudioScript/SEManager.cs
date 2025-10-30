using UnityEngine;
using System.Collections.Generic;

public class SEManager : MonoBehaviour
{
    private static SEManager instance;
    public static SEManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("SEManager");
                instance = obj.AddComponent<SEManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private List<AudioSource> audioSources = new List<AudioSource>();
    private float volume = 1f;
    private const int MAX_AUDIO_SOURCES = 3; // �����Đ��\�Ȍ��ʉ��̐�

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        for (int i = 0; i < MAX_AUDIO_SOURCES; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.volume = volume;
            audioSources.Add(source);
        }
    }

    /// <summary>
    /// SE���Đ�����
    /// </summary>
    public void Play(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;

        AudioSource availableSource = GetAvailableAudioSource();
        if (availableSource != null)
        {
            availableSource.clip = clip;
            availableSource.volume = volume * volumeScale;
            availableSource.Play();
        }
    }

    /// <summary>
    /// SE�������V���b�g�Đ�����i�����̓���SE���d�˂čĐ��\�j
    /// </summary>
    public void PlayOneShot(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;

        AudioSource availableSource = GetAvailableAudioSource();
        if (availableSource != null)
        {
            availableSource.PlayOneShot(clip, volume * volumeScale);
        }
    }

    /// <summary>
    /// �����SE���~����
    /// </summary>
    public void Stop(AudioClip clip)
    {
        foreach (AudioSource source in audioSources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                source.Stop();
            }
        }
    }

    /// <summary>
    /// ���ׂĂ�SE���~����
    /// </summary>
    public void StopAll()
    {
        foreach (AudioSource source in audioSources)
        {
            source.Stop();
        }
    }

    /// <summary>
    /// SE�̉��ʂ�ݒ肷�� (0.0 ~ 1.0)
    /// </summary>
    public void SetVolume(float vol)
    {
        volume = Mathf.Clamp01(vol);
        foreach (AudioSource source in audioSources)
        {
            source.volume = volume;
        }
    }

    /// <summary>
    /// ���݂̉��ʂ��擾����
    /// </summary>
    public float GetVolume()
    {
        return volume;
    }

    /// <summary>
    /// �g�p�\��AudioSource���擾����
    /// </summary>
    private AudioSource GetAvailableAudioSource()
    {
        // �Đ����łȂ�AudioSource��T��
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // ���ׂĎg�p���̏ꍇ�A�ł��Â����̂��g�p
        return audioSources[0];
    }

    /// <summary>
    /// ���[�v����SE���Đ�����
    /// </summary>
    public AudioSource PlayLoop(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return null;

        AudioSource availableSource = GetAvailableAudioSource();
        if (availableSource != null)
        {
            availableSource.clip = clip;
            availableSource.volume = volume * volumeScale;
            availableSource.loop = true;
            availableSource.Play();
            return availableSource;
        }
        return null;
    }

    /// <summary>
    /// ���[�vSE���~����
    /// </summary>
    public void StopLoop(AudioSource source)
    {
        if (source != null)
        {
            source.loop = false;
            source.Stop();
        }
    }
}