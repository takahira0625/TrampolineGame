using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 追従するターゲット（プレイヤー）
    public Transform target;

    // カメラの追従の滑らかさ（値が小さいほど滑らか）
    public float smoothSpeed = 0.125f;

    // カメラとターゲットの初期距離
    private Vector3 offset;

    void Start()
    {
        // ゲーム開始時のカメラとターゲットの位置関係を記憶
        offset = transform.position - target.position;
    }

    // 全てのUpdate処理が終わった後に呼ばれる
    void LateUpdate()
    {
        // ターゲットが設定されていなければ何もしない
        if (target == null)
        {
            return;
        }

        // ターゲットの位置に初期の距離を足して、カメラの目標位置を計算
        Vector3 desiredPosition = target.position + offset;

        // 現在位置から目標位置へ滑らかに移動させる
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // カメラの位置を更新
        transform.position = smoothedPosition;
    }
}