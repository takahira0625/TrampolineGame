using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class BreakBlock : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _rend;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private Collider2D _col;
    [SerializeField] private AudioClip BreakBlockSE; // ブロック破壊時の効果音
    private void Awake()
    {
        LoadBreakBlockSE();
        if (_rend == null)
            _rend = GetComponent<SpriteRenderer>();

        if (_col == null)
            _col = GetComponent<Collider2D>();

        if (_particle == null)
            _particle = GetComponentInChildren<ParticleSystem>();
    }
    private void LoadBreakBlockSE()
    {
        // カスタムSEが設定されていない場合のみ自動ロード
        if (BreakBlockSE == null)
        {
            BreakBlockSE = Resources.Load<AudioClip>("Audio/SE/Block/BreakBlock");

            if (BreakBlockSE == null)
            {
                Debug.LogWarning("鍵のSEが見つかりません: Audio/SE/Block/BreakBlock");
            }
        }
    }
    public void OnBreak()
    {
        SEManager.Instance.PlayOneShot(BreakBlockSE);
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
