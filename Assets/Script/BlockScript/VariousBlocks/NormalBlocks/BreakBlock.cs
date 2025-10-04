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
        Debug.Log(gameObject.name + " BreakBlock.OnBreak called!"); // �������ŌĂ΂ꂽ���Ƃ��m�F
        StartCoroutine(BreakSequence());
    }

    private IEnumerator BreakSequence()
    {
        _rend.enabled = false;
        _col.enabled = false;

        // �p�[�e�B�N�����Đ�
        if (_particle != null)
            _particle.Play();

        // �p�[�e�B�N���̒������҂�
        yield return new WaitForSeconds(_particle.main.startLifetime.constantMax);

        Destroy(gameObject);
    }

}
