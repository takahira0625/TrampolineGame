using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    // ボタンの OnClick() から呼び出してシーン遷移するだけ
    public void OnClickLogin()
    {
        // 名前入力なし → 単純に遷移
        SceneManager.LoadScene("StageSelectScene");
    }
}
