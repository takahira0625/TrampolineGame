using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpRandom: BaseBlock
{
    private static WarpRandom[] allWarps;
    private bool isDisabled = false;
    [SerializeField] private AudioClip warpSE;        // ワープ音
    [SerializeField] private float disableDuration = 0.5f; // ワープ元・先を一時無効化
    [SerializeField] private float offsetY = 1.0f;         // プレイヤーを少し上に出す

    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.WarpRandomSprite);

        // 全Warpをキャッシュ
        allWarps = FindObjectsOfType<WarpRandom>();

        
        if (warpSE == null)
        {
            warpSE = Resources.Load<AudioClip>("Audio/SE/Block/WarpBlock");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isDisabled) return;

        // SE再生
        if (warpSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(warpSE);
        }

        WarpRandom destination = GetRandomWarpExcludingSelf();
        if (destination == null) return;

        StartCoroutine(WarpPlayer(other.transform, destination));
    }

    private WarpRandom GetRandomWarpExcludingSelf()
    {
        if (allWarps == null || allWarps.Length <= 1) return null;

        List<WarpRandom> list = new List<WarpRandom>();

        foreach (var warp in allWarps)
        {
            if (warp != this && !warp.isDisabled)
            {
                list.Add(warp);
            }
        }

        return list.Count > 0 ? list[Random.Range(0, list.Count)] : null;
    }

    private IEnumerator WarpPlayer(Transform player, WarpRandom destination)
    {
        // ワープ元・先を一時的に無効化（戻り防止）
        isDisabled = true;
        destination.isDisabled = true;

        // ワープ処理
        player.position = destination.transform.position + Vector3.up * offsetY;

        // 時間経過後に有効化
        yield return new WaitForSeconds(disableDuration);
        isDisabled = false;
        destination.isDisabled = false;
    }
}
