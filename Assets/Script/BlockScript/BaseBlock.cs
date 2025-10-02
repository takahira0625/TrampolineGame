using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    [SerializeField] protected int health = 3;              // ブロックの体力
    [SerializeField] protected BreakBlock breakBlock;       // 演出担当のスクリプト

    protected virtual void Awake()
    {
        // 自動で BreakBlock を取得
        if (breakBlock == null)
            breakBlock = GetComponent<BreakBlock>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 体力を減らす
            TakeDamage(1);
        }
    }
    protected virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            // 破壊処理をBreakBlockに通知
            if (breakBlock != null)
                breakBlock.OnBreak();
        }
    }
}
