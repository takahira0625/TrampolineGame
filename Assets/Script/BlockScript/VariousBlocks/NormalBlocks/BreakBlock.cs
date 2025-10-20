using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class BreakBlock : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _rend;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private Collider2D _col;
    [SerializeField] private AudioClip BreakBlockSE; // �u���b�N�j�󎞂̌��ʉ�
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
        // �J�X�^��SE���ݒ肳��Ă��Ȃ��ꍇ�̂ݎ������[�h
        if (BreakBlockSE == null)
        {
            BreakBlockSE = Resources.Load<AudioClip>("Audio/SE/Block/BreakBlock");

            if (BreakBlockSE == null)
            {
                Debug.LogWarning("����SE��������܂���: Audio/SE/Block/BreakBlock");
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

        // �p�[�e�B�N�����Đ�
        if (_particle != null)
            _particle.Play();

        // �p�[�e�B�N���̒������҂�
        yield return new WaitForSeconds(_particle.main.startLifetime.constantMax);

        Destroy(gameObject);
    }

}
