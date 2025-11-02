using UnityEngine;

public class InfinityMove : MonoBehaviour
{
    [Header("“®‚«İ’è")]
    public float amplitude = 2.0f; // U‚ê•i‘å‚«‚³j
    public float speed = 1.0f;     // “®‚­‘¬‚³
    public bool rotateDirection = false; // true‚Å‹tŒü‚«‚Ì‡‚É

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float t = Time.time * speed;

        // rotateDirection‚Å‹t‰ñ“]‚à‰Â”\‚É
        if (rotateDirection)
            t = -t;

        // 8‚Ìš‚ğì‚é”Šw“I‚È‘g‚İ‡‚í‚¹
        float x = Mathf.Sin(t) * amplitude;
        float y = Mathf.Sin(t * 2) * 0.5f * amplitude; // Y‚Í”{‘¬‚Å¬‚³‚ß‚É

        transform.position = startPos + new Vector3(x, y, 0f);
    }
}
