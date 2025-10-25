using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using URandom = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // ���E�R�C���֘A
    private int totalKeys = 0;
    public int TotalKeys => totalKeys;

    public int requiredCoins = 5;
    public GameObject goalTextObject;
    public PlayerController playerController;

    private int currentCoins = 0;
    private readonly List<PlayerController> activePlayers = new List<PlayerController>();

    // ==== �^�C�}�[ ====
    [Header("Timer")]
    [SerializeField] private bool autoStartTimer = false;
    private bool isTiming = false;
    private bool hasStarted = false;
    private float elapsedTime = 0f;
    public float FinalTime { get; private set; } = -1f;

    // ==== �����L���O���M ====
    [Header("Ranking")]
    [Tooltip("�V�[���� Stage01..12 ���玩�����o�B�蓮�ŌŒ肵�����ꍇ�� 1..12 ���w��")]
    [SerializeField, Range(0, 12)] private int overrideStageNumber = 0; // 0 �Ȃ玩�����o
    [SerializeField] private ScoreSender scoreSenderPrefab; // ������Ύ��������p
    private ScoreSender scoreSender; // ����

    // BGM
    public AudioClip gameBGM;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (goalTextObject != null) goalTextObject.SetActive(false);
        if (autoStartTimer) StartTimer();
        if (BGMManager.Instance != null && gameBGM != null) BGMManager.Instance.Play(gameBGM);

        // �v���C���[�o�^
        if (playerController == null) playerController = FindObjectOfType<PlayerController>();
        if (playerController != null) RegisterPlayer(playerController);

        // �X�R�A���M�p�R���|�[�l���g�̊m��
        EnsureScoreSender();
        ApplyStageNumberToScoreSender();
    }

    private void Update()
    {
        if (isTiming) elapsedTime += Time.deltaTime;
    }

    // ==== �^�C�}�[���� ====
    public void StartTimer()
    {
        elapsedTime = 0f;
        FinalTime = -1f;
        isTiming = true;
        hasStarted = true;
    }

    public void StartTimerOnce()
    {
        if (!hasStarted) StartTimer();
    }

    public void StopTimer()
    {
        isTiming = false;
        FinalTime = elapsedTime;
    }

    public float ElapsedTime => elapsedTime;

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

    // ==== �v���C���[�Ǘ� ====
    public void RegisterPlayer(PlayerController player)
    {
        if (!activePlayers.Contains(player))
        {
            activePlayers.Add(player);
            Debug.Log($"[Register] {player.name} / cnt={activePlayers.Count}");
        }
    }

    public void UnregisterPlayer(PlayerController player)
    {
        if (activePlayers.Contains(player))
        {
            activePlayers.Remove(player);
            Debug.Log($"[Unregister] �v���C���[�폜: {player.name} / �c��v���C���[��: {activePlayers.Count}");
        }
        else
        {
            Debug.LogWarning($"[Unregister] ���X�g�ɑ��݂��Ȃ��v���C���[: {player.name}");
        }

        if (activePlayers.Count == 0)
        {
            Debug.Log("�S�v���C���[�����S���܂����BGameOver�B");
            GameOver();
        }
    }

    public PlayerController SpawnAdditionalPlayer(Transform originalPlayer, Vector2 velocity)
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController �����ݒ�̂��ߕ����ł��܂���B");
            return null;
        }

        GameObject clone = Instantiate(playerController.gameObject, originalPlayer.position, Quaternion.identity);

        Vector3 offset = new Vector3(URandom.Range(-1.0f, 1.0f), 0.5f, 0f);
        clone.transform.position += offset;

        PlayerController cloneController = clone.GetComponent<PlayerController>();
        if (cloneController != null)
        {
            cloneController.canMove = true;
            RegisterPlayer(cloneController);

            Rigidbody2D cloneRb = clone.GetComponent<Rigidbody2D>();
            if (cloneRb != null)
            {
                cloneRb.velocity = velocity;
            }
        }
        Debug.Log($"�v���C���[�𕪗􂳂��܂����I ���݂̃v���C���[��: {activePlayers.Count}");

        return cloneController;
    }

    // ==== �S�[������ ====
    public void Goal()
    {
        StopTimer();
        if (playerController != null) playerController.canMove = false;
        if (goalTextObject != null) goalTextObject.SetActive(true);
        SaveCurrentStageNumber();
        //FinalTimer�̕ێ�
        PlayerPrefs.SetFloat("finaltimer", FinalTime);
        // �X�R�A���M
        int stage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
        int score = Mathf.RoundToInt(-FinalTime * 1000); // ���̃~���b�i�傫���قǑ����j

        if (scoreSender != null)
        {
            // Steam ����擾�i���������Ȃ�t�H�[���o�b�N�j
            long steamId = 0;
            string playerName = "Unknown";
            try
            {
                // SteamLoginManager �͂��Ȃ��̃v���W�F�N�g�̊����N���X��z��
                if (SteamLoginManager.Initialized)
                {
                    steamId = (long)SteamLoginManager.SteamId64;
                    playerName = SteamLoginManager.PersonaName;
                }
            }
            catch { /* �Q�Ƃł��Ȃ����ł������Ȃ��悤�ɂ��� */ }

            scoreSender.SubmitScoreAndGetBoard(steamId, playerName, "all_time", stage, score);
        }
        else
        {
            Debug.LogWarning("[GameManager] scoreSender ��������܂���B�����L���O���M���X�L�b�v���܂��B");
        }

        // �����L���O�֑J�ځi�����҂ꍇ�� WaitForSeconds �����΂��j
        StartCoroutine(GotoRanking(stage));
    }

    private IEnumerator GotoRanking(int stage)
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene($"RankingScene{stage:00}");
    }

    // ==== �L�[���Z�i�O���[�o���j ====
    public void AddKeyGlobal()
    {
        totalKeys++;
        Debug.Log($"�����擾���܂����i���v: {totalKeys}�j");
        PlayerInventory.RaiseKeyCountChanged(totalKeys);
    }

    // ==== �Q�[���I�[�o�[ ====
    public void GameOver()
    {
        SaveCurrentStageNumber();
        if (BGMManager.Instance != null) BGMManager.Instance.SetVolume(0.2f);
        Debug.Log("Game Over!");
        SceneManager.LoadScene("GameOverScene");
    }

    // ==== �X�R�A���M�̉����� ====
    private void EnsureScoreSender()
    {
        if (scoreSender != null) return;

        scoreSender = FindObjectOfType<ScoreSender>();
        if (scoreSender == null)
        {
            if (scoreSenderPrefab != null)
                scoreSender = Instantiate(scoreSenderPrefab);
            else
                scoreSender = new GameObject("ScoreSender(Auto)").AddComponent<ScoreSender>();
        }

        // �V�[���ׂ��ő��M���r�؂�Ȃ��悤�ɕێ�
        DontDestroyOnLoad(scoreSender.gameObject);
    }

    private void ApplyStageNumberToScoreSender()
    {
        if (scoreSender == null) return;
        int stage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
        scoreSender.StageNumber = stage;
    }

    private int TryParseStageNumberFromSceneName()
    {
        // ��: "Stage01", "Stage12" �� 1..12
        string name = SceneManager.GetActiveScene().name;
        var m = Regex.Match(name, @"Stage\s*0?(\d{1,2})");
        if (m.Success && int.TryParse(m.Groups[1].Value, out int n))
        {
            return Mathf.Clamp(n, 1, 12);
        }
        return 1; // �f�t�H���g
    }

    private int GetCurrentStageNumber()
    {
        // ��: �V�[���� "Stage01" �` "Stage12" ���玩�����o
        if (overrideStageNumber > 0) return overrideStageNumber;

        var name = SceneManager.GetActiveScene().name;
        var m = Regex.Match(name, @"Stage\s*0?(\d{1,2})");
        if (m.Success && int.TryParse(m.Groups[1].Value, out int n))
            return Mathf.Clamp(n, 1, 12);
        return 1;
    }
    // ���݂̃V�[���̔ԍ���ۑ�
    public void SaveCurrentStageNumber()
    {
        int stageNumber = GetCurrentStageNumber();
        PlayerPrefs.SetInt("LastStageNumber", stageNumber);
        PlayerPrefs.Save();
        Debug.Log($"�X�e�[�W�ԍ� {stageNumber} ��ۑ����܂���");
    }
    // �Ō�ɕۑ������V�[���̔ԍ����擾
    public int LoadLastStageNumber()
    {
        int stageNumber = PlayerPrefs.GetInt("LastStageNumber", 1);
        Debug.Log($"�Ō�ɕۑ������X�e�[�W�ԍ���ǂݍ��݂܂���: {stageNumber}");
        return stageNumber;
    }
}
