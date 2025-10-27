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
    /// �|�b�v�A�b�v�̕\��/��\����؂�ւ���
    /// </summary>
    public void TogglePopUp()
    {
        if (popUpObject == null) return;

        // SE���Đ�
        if (clickSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(clickSE);
        }

        bool isActive = popUpObject.activeSelf;

        if (isActive)
        {
            // ���ݕ\���� �� ��\���ɂ���
            popUpObject.SetActive(false);
            Time.timeScale = 1f;

            if (toggleObject != null)
            {
                toggleObject.SetActive(false);
            }
        }
        else
        {
            // ���ݔ�\�� �� �\������
            popUpObject.SetActive(true);
            Time.timeScale = 0f;

            if (toggleObject != null)
            {
                toggleObject.SetActive(true);
            }
        }
    }

   
}
