using System;
using System.Collections;
using UnityEngine;

public class CoolTimeScript : MonoBehaviour
{
    private bool isOnCooldown = false;
    public event Action<bool> OnCooldownChanged; // bool‚Åó‘Ô‚ð’Ê’m

    public bool IsOnCooldown => isOnCooldown;

    public void StartCooldown(float duration)
    {
        if (!isOnCooldown)
            StartCoroutine(CooldownTimer(duration));
    }

    private IEnumerator CooldownTimer(float duration)
    {
        isOnCooldown = true;
        OnCooldownChanged?.Invoke(true);

        yield return new WaitForSeconds(duration);

        isOnCooldown = false;
        OnCooldownChanged?.Invoke(false);
    }
}
