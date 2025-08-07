using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlock : MonoBehaviour
{
    [SerializeField] ParticleSystem _particle;
    private SpriteRenderer _rend;
    private void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(BoxDest());
    }
    private IEnumerator BoxDest()
    {
        _rend.enabled = false;
        _particle.Play();
        yield return new WaitForSeconds(_particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}
