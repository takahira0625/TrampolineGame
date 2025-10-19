using UnityEngine;
using System; // ← イベントを使うために必要

public class PlayerInventory : MonoBehaviour
{
    //鍵の数が変わったときに通知するイベント
    public static event Action<int> OnKeyCountChanged;

    // イベント発火用メソッド
    public static void RaiseKeyCountChanged(int keyCount)
    {
        OnKeyCountChanged?.Invoke(keyCount);
    }

    public void AddKey()
    {
        // 鍵の総数をGameManagerに登録（全プレイヤーで共有）
        GameManager.instance.AddKeyGlobal();
    }
}