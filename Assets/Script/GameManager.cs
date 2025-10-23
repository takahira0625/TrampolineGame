using System;
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

    // ���E�R�C���֘A�i�����j
    private int totalKeys = 0;
    public int TotalKeys => totalKeys;

    public int requiredCoins = 5;
    public GameObject goalTextObject;
    public PlayerController playerController;

    private int currentCoins = 0;
    private List<PlayerController> activePlayers = new List<PlayerController>();

    // ==== �^�C�}�[�֘A ====
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
    private ScoreSender scoreSender; // ���́i���V�[�� or DontDestroy�j

    // BGM
    public AudioClip gameBGM;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }
    }

    void Start()
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

    void Update()
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

    // �R�C�����擾���ꂽ���ɌĂ΂��֐�
    //public void AddCoin()
    //{
    //    currentCoins++;
    //    /*UpdateCoinCounter();*/

    //    // �S�[�������𖞂��������`�F�b�N
    //    if (currentCoins >= requiredCoins)
    //    {
    //        Goal();
    //    }
    //}

    // ==== �v���C���[�Ǘ� ====
    public void RegisterPlayer(PlayerController player)
    {
        if (!activePlayers.Contains(player))
        {
            activePlayers.Add(player);
            Debug.Log($"�yRegister�z{player.name} / cnt={activePlayers.Count}");
        }
    }
    public void UnregisterPlayer(PlayerController player)
    {
        if (activePlayers.Contains(player))
        {
            activePlayers.Remove(player);

            Debug.Log($"�yUnregister�z�v���C���[�폜: {player.name} / �c��v���C���[��: {activePlayers.Count}");
        }
        else
        {
            Debug.LogWarning($"�yUnregister�z���X�g�ɑ��݂��Ȃ��v���C���[: {player.name}");
        }

        if (activePlayers.Count == 0)
        {
            Debug.Log("�S�v���C���[�����S���܂����BGameOver�B");
            GameOver();
        }
        if (activePlayers.Count == 0) GameOver();
    }

    public PlayerController SpawnAdditionalPlayer(Transform originalPlayer, Vector2 velocity)
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController �����ݒ�̂��ߕ����ł��܂���");
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


    public void Goal()
    {
        StopTimer(); 
        if (playerController != null) playerController.canMove = false;
        if (goalTextObject != null) goalTextObject.SetActive(true);

        StartCoroutine(SubmitAndGotoRanking());
    }

    private System.Collections.IEnumerator SubmitAndGotoRanking()
    {
        if (scoreSender != null && FinalTime >= 0f)
        {
            int stage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
            scoreSender.StageNumber = stage;

            scoreSender.SendClearTimeSeconds(FinalTime);

            yield return new WaitForSeconds(0.5f); 
        }

        int targetStage = Mathf.Clamp(GetCurrentStageNumber(), 1, 12);
        string rankingScene = $"RankingScene{targetStage:00}";
        UnityEngine.SceneManagement.SceneManager.LoadScene(rankingScene);
    }


    public void AddKeyGlobal()
    {
        totalKeys++;
        Debug.Log($"�����擾���܂����i���v: {totalKeys}�j");

        PlayerInventory.RaiseKeyCountChanged(totalKeys);
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        StartCoroutine(SubmitAndGotoRanking());
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

        var name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var m = System.Text.RegularExpressions.Regex.Match(name, @"Stage\s*0?(\d{1,2})");
        if (m.Success && int.TryParse(m.Groups[1].Value, out int n))
            return Mathf.Clamp(n, 1, 12);
        return 1;
    }

}
