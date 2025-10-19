using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMTest : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;
    // Start is called before the first frame update
    void Start()
    {
        BGMManager.Instance.Play(bgmClip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
