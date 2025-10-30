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
        string currentSceneName = SceneManager.GetActiveScene().name;
        // �V�[������ "Stage" �Ŏn�܂�A���̌�ɐ����������`����z��
        if (currentSceneName.StartsWith("Stage") && currentSceneName.Length == 7) // "StageXX" �̌`�����`�F�b�N
        {
            // "Stage" �̌�̐��������𒊏o
            if (int.TryParse(currentSceneName.Substring(5, 2), out int stageNumber)) // "Stage" ��5�����ڂ���2���̐������p�[�X
            {
                if (stageNumber >= 1 && stageNumber <= 6)
                {
                    SceneManager.LoadScene("StageSelectScene1_6");
                    Debug.Log($"Current scene {currentSceneName} is a Stage 1-6. Loading StageSelectScene1_6.");
                    return; // ���������������̂ł����Ŋ֐����I��
                }
                else if (stageNumber >= 7 && stageNumber <= 12)
                {
                    SceneManager.LoadScene("StageSelectScene7_12");
                    Debug.Log($"Current scene {currentSceneName} is a Stage 7-12. Loading StageSelectScene7_12.");
                    return; // ���������������̂ł����Ŋ֐����I��
                }
            }
        }
    }
}
