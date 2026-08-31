using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private Button matchButton;
    [SerializeField] private Button deckButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI matchButtonText;
    private bool is_matching;

    void Start()
    {
        is_matching = false;
        // イベント登録
        NetworkManager.Instance.OnWaiting += HandleWaiting;
        NetworkManager.Instance.OnMatchFound += HandleMatchFound;
        NetworkManager.Instance.OnMatchCancelled += HandleMatchCancelled;

        if (matchButton != null)
        {
            matchButton.onClick.AddListener(OnMatchButtonClicked);
        }
        if (deckButton != null)
        {
            deckButton.onClick.AddListener(OnDeckButtonClicked);
        }

        // ★未接続ならサーバーに接続する
        if (!NetworkManager.Instance.IsConnected)
        {
            statusText.text = "connecting...";
            if (matchButton != null) matchButton.interactable = false;

            NetworkManager.Instance.ConnectToServer();

            statusText.text = "connected! press match";
            if (matchButton != null) matchButton.interactable = true;
        }
    }

    private void OnMatchButtonClicked()
    {
        if(is_matching)
        {
            is_matching = false;
            deckButton.interactable = true;
            statusText.text = "cancelling...";
            NetworkManager.Instance.CancelMatching();
            return;
        }
        else
        {
            is_matching = true;
            deckButton.interactable = false;
            statusText.text = "data sending...";
            NetworkManager.Instance.StartMatching();
            return;
        }
        
    }

    private void OnDeckButtonClicked()
    {
        SceneManager.MoveScene(3);
    }

    // 3. サーバーから待機中通知を受信
    private void HandleWaiting(string msg)
    {
        statusText.text = "matching...";
        if (matchButtonText != null) matchButtonText.text = "Cancel";
    }

    // サーバーからキャンセル完了通知を受信
    private void HandleMatchCancelled()
    {
        statusText.text = "canceled";
        is_matching = false;
        if (deckButton != null) deckButton.interactable = true;
        if (matchButtonText != null) matchButtonText.text = "Buttle";
    }

    // 4. マッチング成立時戦闘シーンへ遷移する
    private void HandleMatchFound(NetworkPayload data)
    {
        statusText.text = "matched!";

        SceneManager.MoveScene(2);
    }

    private void OnDestroy()
    {
        // シーン破棄時にイベント解除
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnWaiting -= HandleWaiting;
            NetworkManager.Instance.OnMatchFound -= HandleMatchFound;
            NetworkManager.Instance.OnMatchCancelled -= HandleMatchCancelled;
        }

        if (matchButton != null)
        {
            matchButton.onClick.RemoveListener(OnMatchButtonClicked);
        }
        if (deckButton != null)
        {
            deckButton.onClick.RemoveListener(OnDeckButtonClicked);
        }
    }
}
