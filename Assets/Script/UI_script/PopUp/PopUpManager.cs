using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] private GameObject popUpObject;
    [SerializeField] private GameObject toggleObject;
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
    /// ポップアップの表示/非表示を切り替える
    /// </summary>
    public void TogglePopUp()
    {
        if (popUpObject == null) return;

        // SEを再生
        if (clickSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(clickSE);
        }

        bool isActive = popUpObject.activeSelf;

        if (isActive)
        {
            // 現在表示中 → 非表示にする
            popUpObject.SetActive(false);
            Time.timeScale = 1f;

            if (toggleObject != null)
            {
                toggleObject.SetActive(false);
            }
        }
        else
        {
            // 現在非表示 → 表示する
            popUpObject.SetActive(true);
            Time.timeScale = 0f;

            if (toggleObject != null)
            {
                toggleObject.SetActive(true);
            }
        }
    }

   
}
