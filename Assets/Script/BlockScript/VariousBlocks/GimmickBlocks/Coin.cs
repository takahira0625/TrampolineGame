using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ぶつかった相手が "Player" タグを持っているか確認
        if (other.CompareTag("Player"))
        {
            // GameManagerにコインを取得したことを通知する
            GameManager.instance.AddCoin();

            // コイン自身を消滅させる
            Destroy(gameObject);
        }
    }
}
