using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnClickToSelect()
    {
        SceneManager.LoadScene("SelectScene");
    }
}
