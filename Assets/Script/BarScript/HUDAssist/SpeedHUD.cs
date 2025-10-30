using UnityEngine;
using UnityEngine.UI;

public class SpeedHUD : MonoBehaviour
{
    private Text speedText;
    private GameObject[] players;
    private float updateInterval = 0.5f; // プレイヤーリストの更新間隔
    private float timer = 0f;

    void Start()
    {
        speedText = GetComponent<Text>();
        if (speedText == null)
        {
            Debug.LogError("[SpeedHUD] Textコンポーネントが見つかりません。");
            enabled = false;
            return;
        }

        UpdatePlayerList();
    }

    void Update()
    {
        // 一定間隔でPlayerリストを更新
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

        // 全プレイヤーから最速を探索
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
    /// 現在存在する全てのPlayerタグのオブジェクトを取得
    /// </summary>
    private void UpdatePlayerList()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        // Debug.Log($"[SpeedHUD] Playerリスト更新: {players.Length}個");
    }
}
