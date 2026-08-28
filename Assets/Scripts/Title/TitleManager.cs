using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button startButton;

    void Start()
    {
        // イベントに関数を登録（紐付け）
        NetworkManager.Instance.OnConnected += HandleConnected;

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
    }
    // 1. ボタンが押されたら接続を開始する
    public void StartGame()
    {
        statusText.text = "connecting...";
        if (startButton != null) startButton.interactable = false;

        NetworkManager.Instance.ConnectToServer();
    }

    // 2. サーバーから確実に接続完了の通知が届いたら呼ばれる
    private void HandleConnected()
    {
        statusText.text = "connected!";
        
        // 安全にシーン遷移を実行
        SceneManager.MoveScene(1);
    }
}
