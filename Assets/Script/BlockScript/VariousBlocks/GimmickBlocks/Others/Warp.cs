using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : BaseBlock
{
    private static Warp[] allWarps;
    private bool isDisabled = false;
    [SerializeField] private AudioClip warpSE;        // ワープ音
    [SerializeField] private float disableDuration = 0.5f; // ワープ元・先を一時無効化
    [SerializeField] private float offsetY = 1.0f;         // プレイヤーを少し上に出す

    protected override void Awake()
    {
        base.Awake();
        SetSprite(parameter.WarpSprite);
        
        if (warpSE == null)
        {
            warpSE = Resources.Load<AudioClip>("Audio/SE/Block/WarpBlock");
        }
        
        allWarps = FindObjectsOfType<Warp>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isDisabled) return;

        // SE再生
        if (warpSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlayOneShot(warpSE);
        }

        Warp destination = GetRandomWarpExcludingSelf();
        if (destination != null)
        {
            StartCoroutine(WarpPlayer(other.transform, destination));
        }
    }

    private Warp GetRandomWarpExcludingSelf()
    {
        if (allWarps == null || allWarps.Length <= 1) return null;

        List<Warp> availableWarps = new List<Warp>();
        foreach (var warp in allWarps)
        {
            if (warp != this && !warp.isDisabled)
            {
                availableWarps.Add(warp);
            }
        }

        return availableWarps.Count > 0 ? availableWarps[Random.Range(0, availableWarps.Count)] : null;
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
