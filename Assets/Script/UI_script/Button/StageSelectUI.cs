using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour
{
    [SerializeField] private AudioClip clickSE;
    [SerializeField] private Fade fade;
    private void LoadStage(int n)
    {
        Debug.Log("SE");
        SceneBGMManager.instance.PlayStageBGM();
        SceneManager.LoadScene("Stage" + n.ToString("D2"));
    }
    private void FindFadeCanvas()
    {
        GameObject fadeCanvasObject = GameObject.Find("FadeCanvas");
        if (fadeCanvasObject != null)
        {
            fade = fadeCanvasObject.GetComponent<Fade>();
            if (fade == null)
            {
                Debug.LogWarning("FadeCanvas オブジェクトに Fade コンポーネントが見つかりません。");
            }
        }
        else
        {
            Debug.LogWarning("FadeCanvas オブジェクトがシーン内に見つかりません。");
        }
    }
    public void OnClickStage1()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(1);
        });
        
    }

    public void OnClickStage2()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(2);
        });
        
    }

    public void OnClickStage3()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(3);
        });
    }

    public void OnClickStage4()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(4);
        });
    }

    public void OnClickStage5()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(5);
        });
    }
    public void OnClickStage6()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(6);
        });
    }
    public void OnClickStage7()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(7);
        });
    }

    public void OnClickStage8()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(8);
        });
    }

    public void OnClickStage9()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(9);
        });
    }

    public void OnClickStage10()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(10);
        }); 
    }

    public void OnClickStage11()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(11);
        });
    }

    public void OnClickStage12()
    {
        FindFadeCanvas();
        SEManager.Instance.PlayOneShot(clickSE);
        fade.FadeIn(0.5f, () =>
        {
            LoadStage(12);
        });
    }
}
