using UnityEngine;
using System.Collections;

public class SetActiveSlowZone : MonoBehaviour
{
    private Renderer[] renderers;
    private bool isPlaying = false;

    //追記: 点滅制御用の変数
    private Coroutine blinkCoroutine;
    [SerializeField] private Color blinkColor = Color.red; // 点滅時の目標色
    [SerializeField] private float blinkSpeed = 20f; // 点滅速度 (速いほど速く点滅)
    private Color originalColor;
    private Material[] materials; // 色変更にはMaterialが必要

    [SerializeField, Header("右クリック動作スクリプト")]
    private RightClickTriggerOn rightClickTriggerOn;

    private float clickHoldTime = 0f;
    [SerializeField] private float requiredHoldTime = 5f;
    private bool actionTriggered = false;

    [SerializeField, Header("SlowZone有効時の効果音")]
    private AudioClip activateLoopSE;
    [SerializeField, Header("アクション直前の警告音 (2,3秒)")]
    private AudioClip alarmSE;

    private float alarmStartTime = 3.8f;

    private AudioSource audioSource;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        //追記: マテリアルと初期色の設定
        // 複数の子レンダラーがある場合を考慮し、materials配列を用意
        if (renderers.Length > 0)
        {
            materials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                materials[i] = renderers[i].material;
            }
            // 最初のレンダラーの初期色を保存（複数のレンダラーが同じ色を共有している前提）
            originalColor = materials[0].color;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    //追記: クリックを離した/アクション実行時などに呼ばれるクリーンアップメソッド
    private void CleanupState()
    {
        SetRenderersEnabled(false);
        if (isPlaying)
        {
            audioSource.Stop();
            isPlaying = false;
        }

        //追記: 点滅停止と色リセット
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            SetColor(originalColor); // 元の色に戻す
        }

        clickHoldTime = 0f;
        actionTriggered = false;
    }

    void Update()
    {
        // 右クリック動作中はリセットして終了するロジックは変更なし
        if (rightClickTriggerOn != null && rightClickTriggerOn.IsMoving)
        {
            CleanupState(); // クリーンアップメソッドを呼び出し
            return;
        }


        bool isActive = InputManager.IsLeftClickPressed();

        if (isActive)
        {
            SetRenderersEnabled(true);

            if (!actionTriggered)
            {
                clickHoldTime += Time.deltaTime;

                if (clickHoldTime < alarmStartTime)
                {
                    // 0から1.8秒
                    if (!isPlaying || audioSource.clip != activateLoopSE)
                    {
                        PlayNewLoopSE(activateLoopSE);
                    }

                    //追記: 1.8秒未満では点滅を停止し、元の色を維持
                    if (blinkCoroutine != null)
                    {
                        StopCoroutine(blinkCoroutine);
                        blinkCoroutine = null;
                        SetColor(originalColor);
                    }
                }
                else
                {
                    // 1.8秒から3.0秒
                    if (clickHoldTime < requiredHoldTime)
                    {
                        if (!isPlaying || audioSource.clip != alarmSE)
                        {
                            PlayNewLoopSE(alarmSE);
                        }

                        //追記: 1.8秒を超えたら点滅を開始
                        if (blinkCoroutine == null)
                        {
                            blinkCoroutine = StartCoroutine(BlinkRenderer());
                        }
                    }
                }

                if (clickHoldTime >= requiredHoldTime)
                {
                    if (rightClickTriggerOn != null)
                    {

                        // SEを確実に停止＆点滅を停止し、色をリセット
                        CleanupState();

                        // コルーチンを呼び出し
                        StartCoroutine(rightClickTriggerOn.MoveForwardAndBack());

                        actionTriggered = true;// 複数回実行を防ぐ
                    }
                }
            }
        }
        else
        {
            // 左クリックを離したら状態をクリーンアップ
            CleanupState();
        }
    }

    //追記: 点滅を処理するコルーチン
    private IEnumerator BlinkRenderer()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;
            Color currentColor = Color.Lerp(originalColor, blinkColor, t);
            SetColor(currentColor);
            yield return null; // 毎フレーム実行
        }
    }

    private void SetColor(Color color)
    {
        foreach (var mat in materials)
        {
            mat.color = color;
        }
    }

    private void PlayNewLoopSE(AudioClip clip)
    {
        if (clip == null) return;

        // 既に鳴っている音があれば停止
        if (isPlaying) audioSource.Stop();

        // クリップをセットし、再生
        audioSource.clip = clip;
        audioSource.Play();
        isPlaying = true;
        Debug.Log($"SEを切り替えました: {clip.name}");
    }

    private void SetRenderersEnabled(bool enabled)
    {
        foreach (var rend in renderers)
        {
            rend.enabled = enabled;
        }
    }
}