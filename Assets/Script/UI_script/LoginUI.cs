using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    // �{�^���� OnClick() ����Ăяo���ăV�[���J�ڂ��邾��
    public void OnClickLogin()
    {
        // ���O���͂Ȃ� �� �P���ɑJ��
        SceneManager.LoadScene("StageSelectScene");
    }
}
