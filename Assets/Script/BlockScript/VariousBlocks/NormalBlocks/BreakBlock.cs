using UnityEngine;
using System.Collections;

public class BreakBlock : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _rend;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private Collider2D _col;
    private void Awake()
    {
        if (_rend == null)
            _rend = GetComponent<SpriteRenderer>();

        if (_col == null)
            _col = GetComponent<Collider2D>();

        if (_particle == null)
            _particle = GetComponentInChildren<ParticleSystem>();
    }

    public void OnBreak()
    {
        Debug.Log(gameObject.name + " BreakBlock.OnBreak called!"); // ←ここで呼ばれたことを確認
        StartCoroutine(BreakSequence());
    }

    private IEnumerator BreakSequence()
    {
        _rend.enabled = false;
        _col.enabled = false;

        // パーティクルを再生
        if (_particle != null)
            _particle.Play();

        // パーティクルの長さ分待つ
        yield return new WaitForSeconds(_particle.main.startLifetime.constantMax);

        Destroy(gameObject);
    }

}
