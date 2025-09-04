using UnityEngine;
using TMPro; // TextMeshPro���������߂ɕK�v
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ���̃N���X�̗B��̃C���X�^���X��ێ�����i�V���O���g���j
    public static GameManager instance;

    // �C���X�y�N�^�[����ݒ肷�鍀��
    public int requiredCoins = 5; // �S�[���ɕK�v�ȃR�C���̐�
    public GameObject goalTextObject; // �S�[���e�L�X�g��UI�I�u�W�F�N�g
    /*public TextMeshProUGUI coinCounterText;*/ // �R�C���J�E���^�[��UI�e�L�X�g
    public PlayerController playerController; // �v���C���[�̃X�N���v�g

    private int currentCoins = 0; // ���݂̃R�C���擾��

    void Awake()
    {
        // �V���O���g���̐ݒ�
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // �Q�[���J�n����UI��������
        /*UpdateCoinCounter();*/
        goalTextObject.SetActive(false); // �S�[���e�L�X�g���\����
    }

    // �R�C�����擾���ꂽ���ɌĂ΂��֐�
    public void AddCoin()
    {
        currentCoins++;
        /*UpdateCoinCounter();*/

        // �S�[�������𖞂��������`�F�b�N
        if (currentCoins >= requiredCoins)
        {
            Goal();
        }
    }

    // �R�C���J�E���^�[UI���X�V����֐�
    /*void UpdateCoinCounter()
    {
        if (coinCounterText != null)
        {
            coinCounterText.text = "Coin: " + currentCoins + " / " + requiredCoins;
        }
    }*/

    // �S�[���������s���֐�
    void Goal()
    {
        SceneManager.LoadScene("ResultScene");

        // �v���C���[�̓������~�߂�
        if (playerController != null)
        {
            playerController.canMove = false;
        }
    }
}