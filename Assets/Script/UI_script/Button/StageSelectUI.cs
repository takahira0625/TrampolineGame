using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour
{
    // �{�^���� OnClick() ����Ăяo���ăV�[���J�ڂ��邾��
    public void OnClickStage1()
    {
        // ���O���͂Ȃ� �� �P���ɑJ��
        SceneManager.LoadScene("SampleScene 2");
    }
}
