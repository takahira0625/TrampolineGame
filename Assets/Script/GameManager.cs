using UnityEngine;
using TMPro; // TextMeshPro���������߂ɕK�v
using System.Collections.Generic;//PlayerList�Ǘ��p
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
    
    // ==== Double�u���b�N�ɂ��v���C���[�Ǘ��p ====
    private List<PlayerController> activePlayers = new List<PlayerController>();

    // ==== �^�C�}�[�֘A ====
    [SerializeField] private bool autoStartTimer = false; // �Q�[���J�n�Ŏ����v�����邩
    private bool isTiming = false;
    private bool hasStarted = false;        // �� �ǋL�F��x�����J�n�Ǘ�
    private float elapsedTime = 0f;
    public float FinalTime { get; private set; } = -1f; // �S�[�����̊m��^�C��
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

        //DoubleBlock�p�F�V�[���J�n���Ƀv���C���[�o�^
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        if (playerController != null)
        {
            RegisterPlayer(playerController);
        }
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

    // ==== �ȉ��ADouble�u���b�N�ɂ��v���C���[�Ǘ��p ====
    // �v���C���[��o�^�i�V�[���J�n���ɌĂԁj
    public void RegisterPlayer(PlayerController player)
    {
        if (!activePlayers.Contains(player))
        {
            activePlayers.Add(player);
        }
    }   
    // �v���C���[�����񂾂Ƃ��ɌĂ�
    public void UnregisterPlayer(PlayerController player)
    {
        if (activePlayers.Contains(player))
        {
            activePlayers.Remove(player);
        }

        // �S�������񂾂�Q�[���I�[�o�[
        if (activePlayers.Count == 0)
        {
            Debug.Log("�S�v���C���[�����S���܂����BGameOver�B");
            GameOver();
        }
    }
    public void SpawnAdditionalPlayer(Transform originalPlayer)
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController �����ݒ�̂��ߕ����ł��܂���");
            return;
        }

        // ���v���C���[�̈ʒu����ɐV�����v���C���[�𐶐�
        GameObject clone = Instantiate(playerController.gameObject, originalPlayer.position, Quaternion.identity);

        // �������炵�ďd�Ȃ�Ȃ��悤��
        Vector3 offset = new Vector3(Random.Range(-1.0f, 1.0f), 0.5f, 0f);
        clone.transform.position += offset;

        // clone �� PlayerController �����̂œƗ����ē���
        PlayerController cloneController = clone.GetComponent<PlayerController>();
        if (cloneController != null)
        {
            cloneController.canMove = true;
            RegisterPlayer(cloneController);
        }

        Debug.Log($"�v���C���[�𕪗􂳂��܂����I ���݂̃v���C���[��: {activePlayers.Count}");
    }

    // ==== �S�[���E�Q�[���I�[�o�[���� ====
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