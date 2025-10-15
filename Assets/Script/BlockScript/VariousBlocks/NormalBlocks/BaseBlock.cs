using UnityEngine;
using System.Collections.Generic;

public class BaseBlock : MonoBehaviour
{
    [SerializeField] protected ParameterConfig parameter;
    [SerializeField] protected BreakBlock breakBlock; // 演出担当のスクリプト

    [Header("効果音設定")]
    [Tooltip("Playerとの接触時に効果音を再生する")]
    [SerializeField] protected bool playSoundOnHit = true;
    [Tooltip("このブロック専用の効果音（設定した場合はタグに関係なくこの音を再生）")]
    [SerializeField] protected AudioClip customHitSound;

    protected int health;

    // ★ 追加: 効果音キャッシュ（クラス全体で共有）
    private static Dictionary<string, AudioClip> soundCache = new Dictionary<string, AudioClip>();
    private static AudioClip defaultSound;
    private static bool isSoundCacheInitialized = false;

    // ★ 追加: このインスタンスで使用する効果音（Awakeで事前ロード）
    private AudioClip cachedHitSound;

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

        SetSprite(parameter.baseSprite);

        // ★ 追加: 効果音の事前ロード
        PreloadHitSound();
    }

    // ★ 新規追加: 効果音を事前にロードしてキャッシュ
    private void PreloadHitSound()
    {
        // カスタム効果音が設定されている場合
        if (customHitSound != null)
        {
            cachedHitSound = customHitSound;
            return;
        }

        // 初回のみデフォルト効果音をロード
        if (!isSoundCacheInitialized)
        {
            defaultSound = Resources.Load<AudioClip>("Audio/SE/NormalBlock");
            isSoundCacheInitialized = true;
        }

        // タグに応じた効果音を取得
        string blockTag = gameObject.tag;

        // キャッシュに存在するか確認
        if (soundCache.ContainsKey(blockTag))
        {
            cachedHitSound = soundCache[blockTag];
        }
        else
        {
            // 新規ロードしてキャッシュに保存
            string soundPath = "Audio/SE/" + blockTag;
            AudioClip clip = Resources.Load<AudioClip>(soundPath);

            if (clip != null)
            {
                soundCache[blockTag] = clip;
                cachedHitSound = clip;
            }
            else
            {
                // デフォルト効果音を使用
                cachedHitSound = defaultSound;
                soundCache[blockTag] = defaultSound; // キャッシュに保存
            }
        }
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
            // ★ 修正: 効果音を再生
            PlayHitSound();

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

    // ★ 修正: キャッシュされた効果音を即座に再生
    protected virtual void PlayHitSound()
    {
        if (!playSoundOnHit) return;

        // キャッシュされた効果音を再生（即座に再生される）
        if (cachedHitSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(cachedHitSound);
        }
    }
}
