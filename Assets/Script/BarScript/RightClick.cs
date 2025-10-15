using System.Collections;
using UnityEngine;

public class RightClick : MonoBehaviour
{
    [SerializeField] private VirtualMouse virtualMouse;
    [SerializeField] private BarFollowMouse barFollow;

    [Header("前進距離と時間")]
    [SerializeField] private float forwardDistance = 5.0f;
    [SerializeField] private float forwardTime = 0.1f;
    [SerializeField] private float waitTime = 0.05f;
    [SerializeField] private float returnTime = 0.3f;

    private bool isMoving = false;
    private Vector2 originalPosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isMoving)//右クリック
        {
            StartCoroutine(MoveForwardAndBack());
        }
    }

    private IEnumerator MoveForwardAndBack()
    {
        if (barFollow != null) barFollow.stopFollow = true;
        if (virtualMouse != null) virtualMouse.SetMoving(false);

        isMoving = true;
        originalPosition = transform.position;

        Vector2 forward = transform.up;
        Vector2 targetForwardPos = originalPosition + forward.normalized * forwardDistance;

        float elapsed = 0f;
        while (elapsed < forwardTime)
        {
            transform.position = Vector2.Lerp(originalPosition, targetForwardPos, elapsed / forwardTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetForwardPos;

        // --- 戻る ---
        elapsed = 0f;
        while (elapsed < returnTime)
        {
            transform.position = Vector2.Lerp(targetForwardPos, originalPosition, elapsed / returnTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        isMoving = false;
        if (barFollow != null) barFollow.stopFollow = false;
        if (virtualMouse != null) virtualMouse.SetMoving(true); // ←ここで再開
    }

}
