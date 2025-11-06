using UnityEngine;
using System.Collections;
public class SpeedDoubleBlock : SpeedChangeBlock
{
    [Header("HPごとの安定スプライト")]
    [Tooltip("HP=3 の時のスプライト (SpeedUp)")]
    [SerializeField] private Sprite activeSpriteHP3; // HP3の見た目
    [Tooltip("HP=2 の時のスプライト (SpeedUpHP2)")]
    [SerializeField] private Sprite activeSpriteHP2; // HP2の見た目
    [Tooltip("HP=1 の時のスプライト (SpeedUpHP1)")]
    [SerializeField] private Sprite activeSpriteHP1; // HP1の見た目

    [Header("クールタイム中のスプライト")]
    [Tooltip("HP3 -> 2 の時のクールスプライト (SpeedUpHP2_cool)")]
    [SerializeField] private Sprite coolTimeSpriteHP2; // HP3->2になった時のクール中
    [Tooltip("HP2 -> 1 の時のクールスプライト (SpeedUpHP1_cool)")]
    [SerializeField] private Sprite coolTimeSpriteHP1; // HP2->1になった時のクール中

    protected override void Awake()
    {
        base.Awake();
        SetActiveState();
    }
    protected override void SetActiveState()
    {
        if (eachMaterial != null)
        {
            // ▼▼▼ 修正点 1 ▼▼▼
            // bounciness を 2.0 (UpBounce) にするのをやめ、
            // 1.0 (Bounce) にして反射角を正常化する
            // eachMaterial.bounciness = parameter.UpBounce; // ← 削除
            eachMaterial.bounciness = parameter.Bounce;   // ← 変更
                                                          // ▲▲▲ 修正点 1 終了 ▲▲▲

            col.sharedMaterial = eachMaterial;
        }

        if (health == 3)
        {
            SetSprite(activeSpriteHP3);
        }
        else if (health == 2)
        {
            SetSprite(activeSpriteHP2);
        }
        else if (health == 1)
        {
            SetSprite(activeSpriteHP1);
        }
    }

    // GimmickBlock または SpeedChangeBlock の OnCooldownStart を上書き
    protected override void OnCooldownStart()
    {
        // (物理特性(bounciness)を変更する)
        base.OnCooldownStart(); // ここで bounciness が 1.0 に戻される

        if (health == 2) // HPが3から2になった
        {
            SetSprite(coolTimeSpriteHP2);
        }
        else if (health == 1) // HPが2から1になった
        {
            SetSprite(coolTimeSpriteHP1);
        }
    }
    /// <summary>
    /// GimmickBlock から呼ばれる、プレイヤー（ボール）接触時の処理
    /// （クールタイム中でなく、スロー中でもない時だけ呼ばれる）
    /// </summary>
    protected override void OnPlayerTouch(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (pc != null)
        {

            pc.RequestSpeedChange(parameter.UpBounce);
        }

        if (rb != null)
        {
            StartCoroutine(ResetDragAfterDelay(rb,1f));
        }
    }

    /// <summary>
    /// ボール（リジッドボディ）のDragを一時的に0にし、指定時間後に元に戻す
    /// </summary>
    private IEnumerator ResetDragAfterDelay(Rigidbody2D rb, float delayTime)
    {
        if (rb == null) yield break;

        float originalDrag = rb.drag;

        rb.drag = 0f; // Dragを0に設定

        yield return new WaitForSeconds(delayTime); // 指定時間待機

        if (rb != null)
        {
            rb.drag = originalDrag;
        }
    }
}