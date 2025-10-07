using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Warp : BaseBlock
{
    private static Warp[] allWarps;
    private bool isDisabled = false;

    [SerializeField] private float disableDuration = 0.5f; // ワープ元・先を一時無効化
    [SerializeField] private float offsetY = 1.0f;         // プレイヤーを少し上に出す

    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.WarpSprite);

        // 全Warpをキャッシュ
        allWarps = FindObjectsOfType<Warp>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDisabled) return; // 無効化中は何もしない

        Warp destination = GetRandomWarpExcludingSelf();
        if (destination == null) return;

        StartCoroutine(WarpPlayer(other.transform, destination));
    }

    private Warp GetRandomWarpExcludingSelf()
    {
        if (allWarps == null || allWarps.Length <= 1) return null;

        List<Warp> list = new List<Warp>();
        foreach (var w in allWarps)
        {
            if (w != this && !w.isDisabled)
                list.Add(w);
        }

        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    private IEnumerator WarpPlayer(Transform player, Warp destination)
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
