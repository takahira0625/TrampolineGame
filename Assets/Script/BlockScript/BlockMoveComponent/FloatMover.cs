using UnityEngine;

public class FloatMover : MonoBehaviour
{
    [Header("左右の動き")]
    public float xAmplitude = 0f; // 0 にすれば動かない
    public float xSpeed = 1f;

    [Header("上下の動き")]
    public float yAmplitude = 0f; // 0 にすれば動かない
    public float ySpeed = 1f;

    [Header("動きのタイプ")]
    public bool useSinWave = true; // サイン波で滑らかに動く

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float xOffset = 0f;
        float yOffset = 0f;

        if (useSinWave)
        {
            xOffset = Mathf.Sin(Time.time * xSpeed) * xAmplitude;
            yOffset = Mathf.Sin(Time.time * ySpeed) * yAmplitude;
        }
        else
        {
            xOffset = Mathf.PingPong(Time.time * xSpeed, xAmplitude * 2) - xAmplitude;
            yOffset = Mathf.PingPong(Time.time * ySpeed, yAmplitude * 2) - yAmplitude;
        }

        transform.position = startPos + new Vector3(xOffset, yOffset, 0f);
    }
}
