using UnityEngine;

// 鍵が飛んでいく最終目的地となる空のGameObjectにアタッチする
public class KeyDestination : MonoBehaviour
{
    // シーン内で簡単にアクセスできるようにするための static プロパティ
    public static KeyDestination Instance { get; private set; }

    private void Awake()
    {
        // シーン内に一つだけ存在することを保証
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("KeyDestinationが二重に存在します。新しいインスタンスを破棄します。");
            Destroy(gameObject);
        }
    }
}