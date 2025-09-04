using UnityEngine;
using TMPro; // TextMeshProを扱うために必要
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // このクラスの唯一のインスタンスを保持する（シングルトン）
    public static GameManager instance;

    // インスペクターから設定する項目
    public int requiredCoins = 5; // ゴールに必要なコインの数
    public GameObject goalTextObject; // ゴールテキストのUIオブジェクト
    /*public TextMeshProUGUI coinCounterText;*/ // コインカウンターのUIテキスト
    public PlayerController playerController; // プレイヤーのスクリプト

    private int currentCoins = 0; // 現在のコイン取得数

    void Awake()
    {
        // シングルトンの設定
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ゲーム開始時にUIを初期化
        /*UpdateCoinCounter();*/
        goalTextObject.SetActive(false); // ゴールテキストを非表示に
    }

    // コインが取得された時に呼ばれる関数
    public void AddCoin()
    {
        currentCoins++;
        /*UpdateCoinCounter();*/

        // ゴール条件を満たしたかチェック
        if (currentCoins >= requiredCoins)
        {
            Goal();
        }
    }

    // コインカウンターUIを更新する関数
    /*void UpdateCoinCounter()
    {
        if (coinCounterText != null)
        {
            coinCounterText.text = "Coin: " + currentCoins + " / " + requiredCoins;
        }
    }*/

    // ゴール処理を行う関数
    void Goal()
    {
        SceneManager.LoadScene("ResultScene");

        // プレイヤーの動きを止める
        if (playerController != null)
        {
            playerController.canMove = false;
        }
    }
}