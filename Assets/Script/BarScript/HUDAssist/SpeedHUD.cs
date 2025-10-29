using UnityEngine;
using UnityEngine.UI;

public class SpeedHUD : MonoBehaviour
{
    private Text speedText;
    private GameObject[] players;
    private float updateInterval = 0.5f; // �v���C���[���X�g�̍X�V�Ԋu
    private float timer = 0f;

    void Start()
    {
        speedText = GetComponent<Text>();
        if (speedText == null)
        {
            Debug.LogError("[SpeedHUD] Text�R���|�[�l���g��������܂���B");
            enabled = false;
            return;
        }

        UpdatePlayerList();
    }

    void Update()
    {
        // ���Ԋu��Player���X�g���X�V
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            UpdatePlayerList();
            timer = 0f;
        }

        if (players == null || players.Length == 0)
        {
            speedText.text = "Max Speed: -";
            return;
        }

        float maxSpeed = 0f;
        GameObject fastestPlayer = null;

        // �S�v���C���[����ő���T��
        foreach (var player in players)
        {
            if (player == null) continue;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            float speed = rb.velocity.magnitude;
            if (speed > maxSpeed)
            {
                maxSpeed = speed;
                fastestPlayer = player;
            }
        }

        if (fastestPlayer != null)
        {
            speedText.text = $"Max Speed: {maxSpeed:F2}";
        }
        else
        {
            speedText.text = "Max Speed: -";
        }
    }

    /// <summary>
    /// ���ݑ��݂���S�Ă�Player�^�O�̃I�u�W�F�N�g���擾
    /// </summary>
    private void UpdatePlayerList()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        // Debug.Log($"[SpeedHUD] Player���X�g�X�V: {players.Length}��");
    }
}
