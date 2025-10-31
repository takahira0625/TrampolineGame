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
    private const int MAX_AUDIO_SOURCES = 3; // 同時再生可能な効果音の数

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
    /// SEを再生する
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
    /// SEをワンショット再生する（複数の同じSEを重ねて再生可能）
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
    /// 特定のSEを停止する
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
    /// すべてのSEを停止する
    /// </summary>
    public void StopAll()
    {
        foreach (AudioSource source in audioSources)
        {
            source.Stop();
        }
    }

    /// <summary>
    /// SEの音量を設定する (0.0 ~ 1.0)
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
    /// 現在の音量を取得する
    /// </summary>
    public float GetVolume()
    {
        return volume;
    }

    /// <summary>
    /// 使用可能なAudioSourceを取得する
    /// </summary>
    private AudioSource GetAvailableAudioSource()
    {
        // 再生中でないAudioSourceを探す
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // すべて使用中の場合、最も古いものを使用
        return audioSources[0];
    }

    /// <summary>
    /// ループするSEを再生する
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
    /// ループSEを停止する
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