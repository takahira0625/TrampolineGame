using UnityEngine;

public class VerticalMove : MonoBehaviour
{
    [Header("移動設定")]
    public float amplitude = 1.0f; // 上下の振れ幅
    public float speed = 1.0f;     // 移動の速さ
    public bool useSinWave = true; // スムーズに動かすかどうか

    private Vector3 startPos;      // 初期位置を記録

    void Start()
    {
        // ゲーム開始時の位置を覚える
        startPos = transform.position;
    }

    void Update()
    {
        float offset;

        if (useSinWave)
        {
            // サイン波で上下にスムーズに動く
            offset = Mathf.Sin(Time.time * speed) * amplitude;
        }
        else
        {
            // 三角波のように上下を直線的に往復する
            offset = Mathf.PingPong(Time.time * speed, amplitude * 2) - amplitude;
        }

        transform.position = startPos + new Vector3(0, offset, 0);
    }
}
