using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ManualTextJPManager : MonoBehaviour
{
    [SerializeField] private Text JP1;
    [SerializeField] private Text JP2;
    [SerializeField] private Text JP3;
    // Start is called before the first frame update
    void Start()
    {
        if (!InputManager.LeftClickSlow) {
            JP1.text = "       ���݂��N���b�N �@�Q�[���X�^�[�g";
            JP2.text = "�݂��N���b�N�@�@�@�@�@�@�X���[�]�[���͂�����";
            JP3.text = "�Ђ���N���b�N�@�@�@�@�@�{�[�����͂���";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
