using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchToLoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �}�E�X�̍��N���b�N�A�܂��͉�ʂ��^�b�`���ꂽ�u�ԂɎ��s
        if (Input.GetMouseButtonDown(0))
        {
            // "SelectScene"�Ƃ������O�̃V�[����ǂݍ���
            SceneManager.LoadScene("SelectScene");
        }
    }
}
