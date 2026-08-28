using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private Button matchButton;
    [SerializeField] private Text statusText;

    async void Start()
    {
        // 1. サーバーへ接続
        statusText.text = "サーバーに接続中...";
        await NetworkManager.Instance.ConnectToServer();
        statusText.text = "サーバー接続完了";

        // 2. イベント購読
        NetworkManager.Instance.OnWaiting += (msg) =>
        {
            statusText.text = "対戦相手を探しています...";
        };

        NetworkManager.Instance.OnMatchFound += (data) =>
        {
            statusText.text = "対戦相手が見つかりました！戦闘画面へ移行します...";
            // 3. マッチング成立で戦闘シーンへ移動
            SceneManager.MoveScene(2);
        };

        // マッチングボタン押下
        matchButton.onClick.AddListener(() =>
        {
            matchButton.interactable = false;
            NetworkManager.Instance.StartMatching();
        });
    }

    private void OnDestroy()
    {
        // シーン破棄時にイベント購読解除（メモリリーク防止）
        if (NetworkManager.Instance != null)
        {
            // 必要に応じてイベント解除を記述
        }
    }
}
