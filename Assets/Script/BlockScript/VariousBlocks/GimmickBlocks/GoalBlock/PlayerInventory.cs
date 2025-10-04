using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int KeyCount { get; private set; } = 0;

    public void AddKey()
    {
        KeyCount++;
        Debug.Log("ƒL[æ“¾I Œ»İ‚ÌŠ”: " + KeyCount);
    }
}
