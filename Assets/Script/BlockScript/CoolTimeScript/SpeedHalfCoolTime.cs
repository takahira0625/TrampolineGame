using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedHalfCoolTime : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 5.0f; // クールタイム（秒）
    private bool isOnCooldown = false;
    private Collider2D col;
    private PhysicsMaterial2D physicsMaterial;

    void Awake()
    {
        // Colliderを取得
        col = GetComponent<Collider2D>();
        if (col != null && col.sharedMaterial != null)
        {
            // PhysicsMaterialのBouncinessを2に設定
            physicsMaterial = col.sharedMaterial;
            physicsMaterial.bounciness = 0.5f;
        }
        else
        {
            Debug.LogWarning("Collider2D または PhysicsMaterial2D が見つかりません");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Playerが触れた場合の処理
        if (collision.gameObject.CompareTag("Player") && !isOnCooldown)
        {
            // PhysicsMaterialのBouncinessを1に変更
            if (physicsMaterial != null)
            {
                physicsMaterial.bounciness = 1.0f;
            }

            // クールタイムのカウント開始
            StartCoroutine(CooldownTimer());

            // マテリアル変更関数を呼び出し
            ChangeMaterial();
        }
    }

    // マテリアルを変更する関数
    private void ChangeMaterial()
    {
        // 例：Rendererの色を変更
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.gray; // クールタイム中は灰色に
        }

        Debug.Log("ギミックがクールタイム状態になりました");
    }

    // クールタイム管理用のコルーチン
    private IEnumerator CooldownTimer()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(cooldownTime);

        // クールタイム終了後の処理
        isOnCooldown = false;

        // PhysicsMaterialのBouncinessを元に戻す
        if (physicsMaterial != null)
        {
            physicsMaterial.bounciness = 0.5f;
        }

        // 見た目を元に戻す
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white; // 元の色に戻す
        }

        Debug.Log("ギミックのクールタイムが終了しました");
    }
}