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
            Debug.LogError("[SpeedHUD] Text�R���|�[�l���g��������܂���B");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[SpeedHUD] �^�O 'Player' �̃I�u�W�F�N�g��������܂���B");
            return;
        }

        playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb == null)
        {
            Debug.LogError("[SpeedHUD] Player��Rigidbody2D���A�^�b�`����Ă��܂���B");
            return;
        }

        Debug.Log("[SpeedHUD] �����������FPlayer���o����");
    }

    void Update()
    {
        if (playerRb == null || speedText == null) return;

        float speed = playerRb.velocity.magnitude;
        speedText.text = $"Speed: {speed:F2}";
    }
}
