using UnityEngine;
using UnityEngine.UI;

public class SpeedHUD : MonoBehaviour
{
    private Text speedText;
    private Rigidbody2D playerRb;

    void Start()
    {
        speedText = GetComponent<Text>();
        if (speedText == null)
        {
            Debug.LogError("[SpeedHUD] Textコンポーネントが見つかりません。");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[SpeedHUD] タグ 'Player' のオブジェクトが見つかりません。");
            return;
        }

        playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb == null)
        {
            Debug.LogError("[SpeedHUD] PlayerにRigidbody2Dがアタッチされていません。");
            return;
        }

        Debug.Log("[SpeedHUD] 初期化成功：Player検出完了");
    }

    void Update()
    {
        if (playerRb == null || speedText == null) return;

        float speed = playerRb.velocity.magnitude;
        speedText.text = $"Speed: {speed:F2}";
    }
}
