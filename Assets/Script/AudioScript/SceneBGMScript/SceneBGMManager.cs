using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBGMManager : MonoBehaviour
{
    public static SceneBGMManager instance;
    [SerializeField] private AudioClip titleBGMClip;
    [SerializeField] private AudioClip stageBGMClip;
    [SerializeField] private AudioClip resultBGMClip;
    
    private const string VOLUME_KEY = "BGMVolume";
    
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
        // ï€ë∂Ç≥ÇÍÇΩâπó ÇìKóp
        LoadAndApplyVolume();
        PlayTitleBGM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void LoadAndApplyVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.5f);
        BGMManager.Instance.SetVolume(savedVolume);
    }
    
    public void PlayTitleBGM()
    {
        // SetVolume(1f)ÇçÌèú
        BGMManager.Instance.Play(titleBGMClip, fadeIn: true, fadeDuration: 2f);
    }
    
    public void PlayStageBGM()
    {
        // SetVolume(1f)ÇçÌèú
        BGMManager.Instance.Play(stageBGMClip, fadeIn: true, fadeDuration: 1f);
    }
    
    public void PlayResultBGM()
    {
        // SetVolume(1f)ÇçÌèú
        BGMManager.Instance.Play(resultBGMClip, fadeIn: true, fadeDuration: 2f);
    }
}
