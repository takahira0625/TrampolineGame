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
    private const int MAX_AUDIO_SOURCES = 20; // ★ 10から20に増やす

    // ★ 新規追加: 優先度管理
    private Queue<int> availableSourceIndices = new Queue<int>();

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
            source.priority = 128; // ★ SEの優先度を設定(0-256、値が小さいほど優先度が高い)
            audioSources.Add(source);
            availableSourceIndices.Enqueue(i);
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
    /// SEをワンショット再生する(複数の同じSEを重ねて再生可能)
    /// </summary>
    public void PlayOneShot(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;

        AudioSource availableSource = GetAvailableAudioSource();
        if (availableSource != null)
        {
            availableSource.PlayOneShot(clip, volume * volumeScale);
        }
        else
        {
            // ★ デバッグログ追加(必要に応じてコメントアウト)
            // Debug.LogWarning($"SE再生失敗: 利用可能なAudioSourceがありません - {clip.name}");
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
    /// 使用可能なAudioSourceを取得する(改善版)
    /// </summary>
    private AudioSource GetAvailableAudioSource()
    {
        // ★ 改善: 再生中でない最初のAudioSourceを探す
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // ★ 改善: すべて使用中の場合、最も再生時間が経過したものを見つける
        AudioSource oldestSource = audioSources[0];
        float oldestTime = audioSources[0].time;

        for (int i = 1; i < audioSources.Count; i++)
        {
            if (audioSources[i].time > oldestTime)
            {
                oldestTime = audioSources[i].time;
                oldestSource = audioSources[i];
            }
        }

        return oldestSource;
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