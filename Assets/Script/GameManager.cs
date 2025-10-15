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

    // ==== �^�C�}�[�֘A ====
    [SerializeField] private bool autoStartTimer = false; // �Q�[���J�n�Ŏ����v�����邩
    private bool isTiming = false;
    private bool hasStarted = false;        // �� �ǋL�F��x�����J�n�Ǘ�
    private float elapsedTime = 0f;
    public float FinalTime { get; private set; } = -1f; // �S�[�����̊m��^�C��
    // BGM�֘A
    public AudioClip gameBGM;
    void Awake()
    {
        // �V���O���g���̐ݒ�
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����ׂ��ŕێ�
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // �Q�[���J�n����UI��������
        /*UpdateCoinCounter();*/
        goalTextObject.SetActive(false); // �S�[���e�L�X�g���\����
        if (goalTextObject != null) goalTextObject.SetActive(false);
        if (autoStartTimer) StartTimer();
        BGMManager.Instance.Play(gameBGM);
    }

    private void Update()
    {
        if (isTiming)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    // ==== �^�C�}�[���� ====
    public void StartTimer()
    {
        elapsedTime = 0f;
        FinalTime = -1f;
        isTiming = true;
        hasStarted = true;
    }

    // �� �ǋL�F��d�N���h�~�p�̃��b�p�[
    public void StartTimerOnce()
    {
        if (!hasStarted) StartTimer();
    }

    public void StopTimer()
    {
        isTiming = false;
        FinalTime = elapsedTime;
    }
    public float ElapsedTime => elapsedTime; // ���݂̌o�ߕb���O������Q��
    // �������g���C�ōČv���������ꍇ�ɌĂԗp�i�C�Ӂj
    public void ResetTimerForNewRun()
    {
        isTiming = false;
        hasStarted = false;
        elapsedTime = 0f;
        FinalTime = -1f;
    }
    public static string FormatTime(float t)
    {
        if (t < 0f) return "--:--.--";
        int minutes = (int)(t / 60f);
        float seconds = t - minutes * 60f;
        return $"{minutes:00}:{seconds:00.00}";
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
        StopTimer(); // �� �^�C�}�[�m��
        SceneManager.LoadScene("ResultScene");

        // �v���C���[�̓������~�߂�
        if (playerController != null)
        {
            playerController.canMove = false;
        }
    }
    public void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene("GameOverScene");
    }
}