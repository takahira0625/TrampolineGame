using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    [SerializeField] protected ParameterConfig parameter;
    [SerializeField] protected BreakBlock breakBlock; // 演出担当のスクリプト

    protected int health;
    protected virtual void Awake()
    {
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

        // 見た目を設定
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            sr.drawMode = SpriteDrawMode.Sliced; // Sliced or Tiled
            // 元の Sprite サイズを取得してそのまま設定
            sr.size = sr.sprite.bounds.size;
        }
        SetSprite(parameter.baseSprite);
    }
    //マテリアルを変更
    protected void SetSprite(Sprite sprite)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sprite != null)
        {
            sr.sprite = sprite;
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
