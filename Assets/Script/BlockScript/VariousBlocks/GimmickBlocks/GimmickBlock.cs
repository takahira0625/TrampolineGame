using UnityEngine;

//GimmickBlock:クールタイムを持つギミックブロックの基底クラス
//1.クールタイム中は無敵 2.クールタイム中は色を暗くする 3.クールタイム中はバウンスを１にする
public abstract class GimmickBlock : BaseBlock
{
    // ... （Awake, HandleCooldownChanged までは変更なし） ...
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
            //SetSprite(parameter.blackSprite);//このコードはOK
            OnCooldownStart();
        }
        else
        {
            SetActiveState();
        }
    }

    // OnPlayerTouch は子クラスで実装するので中身は空のまま
    protected virtual void OnPlayerTouch(GameObject player)
    {
    }

    // ▼▼▼ 修正点 1 ▼▼▼
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // クールタイム中でないか確認
            if (cooldown != null && !cooldown.IsOnCooldown)
            {
                // プレイヤーがスロー中でないか確認
                PlayerController pc = collision.gameObject.GetComponent<PlayerController>();

                // pc が存在し、かつスロー中でない (!pc.isActive) 場合のみ OnPlayerTouch を呼ぶ
                if (pc != null && !pc.isActive)
                {
                    OnPlayerTouch(collision.gameObject);
                }
            }
        }

        // base の処理（TakeDamage）は OnPlayerTouch のチェック後に行う
        base.OnCollisionEnter2D(collision);
    }
    // ▲▲▲ 修正点 1 終了 ▲▲▲


    // ▼▼▼ 修正点 2 ▼▼▼
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // クールタイム中でないか確認
            if (cooldown != null && !cooldown.IsOnCooldown)
            {
                // プレイヤーがスロー中でないか確認
                PlayerController pc = other.GetComponent<PlayerController>();

                // pc が存在し、かつスロー中でない (!pc.isActive) 場合のみ OnPlayerTouch を呼ぶ
                if (pc != null && !pc.isActive)
                {
                    OnPlayerTouch(other.gameObject);
                }
            }
        }
    }
    // ▲▲▲ 修正点 2 終了 ▲▲▲

    // ... （TakeDamage, UpdateBlockAppearance などの残りのメソッドは変更なし） ...
    protected override void TakeDamage(int damage)
    {
        if (cooldown != null && cooldown.IsOnCooldown) return; // クールタイム中は無敵
        base.TakeDamage(damage);  // 通常処理

        if (health > 0) // 'health' は BaseBlock から継承
        {
            if (cooldown != null)
            {
                // これが OnCooldownStart() をトリガーする
                cooldown.StartCooldown(parameter.cooldownTime);
            }
        }
    }

    /// <summary>
    /// BaseBlockのHP連動スプライト変更を「無効化」する
    /// </summary>
    protected override void UpdateBlockAppearance()
    {
        // GimmickBlock は SetActiveState / OnCooldownStart で
        // 独自にスプライトを管理するため、BaseBlock の処理は呼ばない。
        // (ここを空にすることで、TakeDamage 時にスプライトが勝手に変わるのを防ぐ)
    }

    protected virtual void OnCooldownStart() { }
    protected virtual void SetActiveState() { }

}