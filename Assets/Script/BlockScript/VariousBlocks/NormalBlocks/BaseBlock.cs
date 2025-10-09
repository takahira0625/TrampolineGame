using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    [SerializeField] protected ParameterConfig parameter;
    [SerializeField] protected BreakBlock breakBlock; // 演出担当のスクリプト

    protected int health;

    protected virtual void Awake()
    {
        Physics.bounceThreshold = 0.0f;
        //①コンポーネントを追加（BreakBlock.cs,ParameterConfig.cs）

        // -------------------------------
        // BreakBlock を自動取得・追加
        // -------------------------------
        if (breakBlock == null)
        {
            breakBlock = GetComponent<BreakBlock>();
            if (breakBlock == null)
            {
                breakBlock = gameObject.AddComponent<BreakBlock>();
            }
        }
        // -------------------------------
        // ParameterConfig を自動取得
        // -------------------------------
        if (parameter == null)
        {
            // Resources フォルダに ParameterConfig を置く想定
            parameter = Resources.Load<ParameterConfig>("ParameterConfig");
        }
        // health 設定
        if (parameter != null)
            health = parameter.Health;

        SetSprite(parameter.baseSprite);
    }

    // スプライトを変更
    protected void SetSprite(Sprite sprite)
    {
        if (sprite == null || parameter == null) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // スプライトを設定
        sr.sprite = sprite;

        // Slicedモードに設定
        sr.drawMode = SpriteDrawMode.Sliced;

        // サイズを適用
        sr.size = new Vector2(parameter.Width, parameter.Height);
        Debug.Log(sr.size);

        // BoxCollider2Dのサイズも合わせる
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(parameter.Width, parameter.Height);
        }
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
