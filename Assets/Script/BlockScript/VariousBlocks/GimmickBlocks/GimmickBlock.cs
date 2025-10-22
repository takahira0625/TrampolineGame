using UnityEngine;

//GimmickBlock:クールタイムを持つギミックブロックの基底クラス
//1.クールタイム中は無敵 2.クールタイム中は色を暗くする 3.クールタイム中はバウンスを１にする
public abstract class GimmickBlock : BaseBlock
{
    //コンポーネントの取得
    [SerializeField] protected CoolTimeScript cooldown;
    protected Collider2D col;
    protected Renderer rend;

    protected override void Awake()
    {
        base.Awake();
        //①レンダラーの取得
        rend = GetComponent<Renderer>();
        //②cooltimeComponentの取得
        if (cooldown == null)
        {
            cooldown = GetComponent<CoolTimeScript>();
            if (cooldown == null)
            {
                cooldown = gameObject.AddComponent<CoolTimeScript>();
            }
        }
        cooldown.OnCooldownChanged += HandleCooldownChanged;
        //③コライダーの取得
        col = GetComponent<Collider2D>();
    }

    private void HandleCooldownChanged(bool isOnCooldown)
    {
        if (isOnCooldown)
        {
            SetSprite(parameter.blackSprite);//このコードはOK
            OnCooldownStart();
        }
        else
        {
            SetActiveState();
        }
    }

    protected virtual void OnPlayerTouch(GameObject player)
    {
        // 共通の「プレイヤー接触時」処理（クールタイムなど）
        if (cooldown != null && !cooldown.IsOnCooldown)
        {
            cooldown.StartCooldown(parameter.cooldownTime);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerTouch(collision.gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerTouch(other.gameObject);
        }
    }
    protected override void TakeDamage(int damage)
    {
        if (cooldown != null && cooldown.IsOnCooldown) return; // クールタイム中は無敵
        base.TakeDamage(damage);  // 通常処理
    }

    protected virtual void OnCooldownStart() { }
    protected virtual void SetActiveState() { }

}
