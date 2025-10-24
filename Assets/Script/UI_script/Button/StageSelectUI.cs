using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    private void LoadStage(int n)
    {
        Debug.Log("SE��炵�܂���");
        SEManager.Instance.PlayOneShot(clickSE);
        SceneBGMManager.instance.PlayStageBGM();
        SceneManager.LoadScene("Stage" + n.ToString("D2"));
    }

    public void OnClickStage1()
    {
        LoadStage(1);
    }

    public void OnClickStage2()
    {
        LoadStage(2);
    }

    public void OnClickStage3()
    {
        LoadStage(3);
    }

    public void OnClickStage4()
    {
        LoadStage(4);
    }

    public void OnClickStage5()
    {
        LoadStage(5);
    }

    public void OnClickStage7()
    {
        LoadStage(7);
    }

    public void OnClickStage8()
    {
        LoadStage(8);
    }

    public void OnClickStage9()
    {
        LoadStage(9);
    }

    public void OnClickStage10()
    {
        LoadStage(10);
    }

    public void OnClickStage11()
    {
        LoadStage(11);
    }

    public void OnClickStage12()
    {
        LoadStage(12);
    }
}
