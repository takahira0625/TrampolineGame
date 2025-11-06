using UnityEngine;

public class HorizontalMove : MonoBehaviour
{
    [Header("移動設定")]
    public float amplitude = 2.0f; // 左右の振れ幅
    public float speed = 1.0f;     // 移動スピード
    public bool useSinWave = true; // サイン波でスムーズに動くかどうか

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float xOffset;

        if (useSinWave)
        {
            // サイン波でスムーズに左右移動
            xOffset = Mathf.Sin(Time.time * speed) * amplitude;
        }
        else
        {
            // PingPongで直線的な往復
            xOffset = Mathf.PingPong(Time.time * speed, amplitude * 2) - amplitude;
        }

        transform.position = startPos + new Vector3(xOffset, 0, 0);
    }
}
