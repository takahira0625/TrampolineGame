using UnityEngine;

public class ArcParticleShooter : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public float interval = 0.3f;
    public float minAngle = 45f;
    public float maxAngle = 135f;
    public float speed = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            EmitInArc();
            timer = 0f;
        }
    }

    void EmitInArc()
    {
        if (particleSystem == null) return;

        var emitParams = new ParticleSystem.EmitParams();

        // ランダムな角度（度→ラジアン）
        float angle = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;

        // 前方向（右方向を基準にして円弧に）
        Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

        emitParams.velocity = dir * speed;
        emitParams.startColor = Color.magenta;
        emitParams.startSize = 0.1f;

        particleSystem.Emit(emitParams, 1);
    }
}
