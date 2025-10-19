using UnityEngine;
using System.Collections.Generic;

public class BaseBlock : MonoBehaviour
{
    [SerializeField] protected ParameterConfig parameter;
    [SerializeField] protected BreakBlock breakBlock; // ブロック破壊のスクリプト

    [Header("効果音設定")]
    [Tooltip("Playerとの接触時に効果音を再生する")]
    [SerializeField] protected bool playSoundOnHit = true;
    [Tooltip("このブロック専用の効果音(設定した場合はタグに関係なくこの音を再生)")]
    [SerializeField] protected AudioClip customHitSound;

    [Header("エフェクト設定")]
    [Tooltip("Playerとの接触時にエフェクトを再生する")]
    [SerializeField] protected bool playEffectOnHit = true;

    protected int health;

    // ★ 追加: 効果音キャッシュ(クラス全体で共有)
    private static Dictionary<string, AudioClip> soundCache = new Dictionary<string, AudioClip>();
    private static AudioClip defaultSound;
    private static bool isSoundCacheInitialized = false;

    // ★ 追加: 衝突エフェクト(クラス全体で共有)
    private static ParticleSystem hitEffectPrefab;
    private static bool isEffectLoaded = false;

    // ★ 追加: このインスタンスで使用する効果音(Awakeで事前ロード)
    private AudioClip cachedHitSound;

    protected virtual void Awake()
    {
        Physics.bounceThreshold = 0.0f;
        //　コンポーネントを追加(BreakBlock.cs,ParameterConfig.cs)

        // -------------------------------
        // BreakBlock コンポーネント取得・追加
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
        // ParameterConfig コンポーネント取得
        // -------------------------------
        if (parameter == null)
        {
            // Resources フォルダから ParameterConfig を読み込む
            parameter = Resources.Load<ParameterConfig>("ParameterConfig");
        }
        // health 設定
        if (parameter != null)
            health = parameter.Health;

        SetSprite(parameter.baseSprite);

        // ★ 追加: 効果音の事前ロード
        PreloadHitSound();

        // ★ 追加: エフェクトの事前ロード(一度のみ)
        PreloadHitEffect();
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

        // 一度のみデフォルト効果音をロード
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
            string soundPath = "Audio/SE/Block/" + blockTag;
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

    // ★ 新規追加: エフェクトを事前にロード(一度のみ、全ブロック共通)
    private void PreloadHitEffect()
    {
        if (!isEffectLoaded)
        {
            hitEffectPrefab = Resources.Load<ParticleSystem>("Effects/CFXR3 Hit Fire B (Air)");
            isEffectLoaded = true;

            if (hitEffectPrefab == null)
            {
                Debug.LogWarning("HitSparkエフェクトが見つかりません: Resources/Effects/CFXR3 Hit Fire B (Air)");
            }
        }
    }

    // スプライトを変更
    protected virtual void SetSprite(Sprite sprite)
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
            // ★ 追加: 接触位置を取得
            Vector2 contactPoint = collision.contacts[0].point;

            // ★ 改善: 効果音を再生
            PlayHitSound();

            // ★ 追加: エフェクトを再生
            PlayHitEffect(contactPoint);

            // 耐力を減らす
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

    // ★ 改善: キャッシュされた効果音を即座に再生
    protected virtual void PlayHitSound()
    {
        if (!playSoundOnHit) return;

        // キャッシュされた効果音を再生(瞬時に再生される)
        if (cachedHitSound != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(cachedHitSound);
        }
    }

    // ★ 新規追加: 衝突位置にエフェクトを生成(シンプル版)
    protected virtual void PlayHitEffect(Vector2 position)
    {
        if (!playEffectOnHit || hitEffectPrefab == null) return;

        // エフェクトを生成して再生
        ParticleSystem effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);

        // 自動削除
        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
    }
}