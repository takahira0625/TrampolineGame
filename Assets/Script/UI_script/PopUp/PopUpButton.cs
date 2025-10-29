using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopUpButton : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ���݂̃V�[�����ēǂݍ��݂���
    /// </summary>
    public void ReloadCurrentScene()
    {
        SEManager.Instance.PlayOneShot(clickSE);
        BGMManager.Instance.Stop();
        SceneBGMManager.instance.PlayStageBGM();
        // Time.timeScale��0�̏ꍇ�͌��ɖ߂�
        Time.timeScale = 1f;

        // ���݂̃V�[�������擾���čēǂݍ���
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    /// <summary>
    /// StageSelectScene1_6�ɑJ�ڂ���
    /// </summary>
    public void LoadStageSelectScene()
    {
        // Time.timeScale��0�̏ꍇ�͌��ɖ߂�
        Time.timeScale = 1f;

        // SE���Đ�
        if (clickSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(clickSE);
        }

        // BGM��ύX
        if (SceneBGMManager.instance != null)
        {
            BGMManager.Instance.Stop();
            SceneBGMManager.instance.PlayTitleBGM();
        }

        // �X�e�[�W�Z���N�g�V�[���ɑJ��
        SceneManager.LoadScene("StageSelectScene1_6");
    }
}
