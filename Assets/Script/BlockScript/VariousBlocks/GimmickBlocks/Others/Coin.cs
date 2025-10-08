using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �Ԃ��������肪 "Player" �^�O�������Ă��邩�m�F
        if (other.CompareTag("Player"))
        {
            // GameManager�ɃR�C�����擾�������Ƃ�ʒm����
            GameManager.instance.AddCoin();

            // �R�C�����g�����ł�����
            Destroy(gameObject);
        }
    }
}
