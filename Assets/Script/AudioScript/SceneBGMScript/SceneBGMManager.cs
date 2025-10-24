using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBGMManager : MonoBehaviour
{
    public static SceneBGMManager instance;
    [SerializeField] private AudioClip titleBGMClip;
    [SerializeField] private AudioClip stageBGMClip;
    [SerializeField] private AudioClip resultBGMClip;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayTitleBGM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayTitleBGM()
    {
        BGMManager.Instance.SetVolume(1f);
        BGMManager.Instance.Play(titleBGMClip, fadeIn: true, fadeDuration: 2f);
    }
    public void PlayStageBGM()
    {
        BGMManager.Instance.SetVolume(1f);
        BGMManager.Instance.Play(stageBGMClip, fadeIn: true, fadeDuration: 1f);
    }
    public void PlayResultBGM()
    {
        BGMManager.Instance.SetVolume(1f);
        BGMManager.Instance.Play(resultBGMClip, fadeIn: true, fadeDuration: 2f);
    }

}
