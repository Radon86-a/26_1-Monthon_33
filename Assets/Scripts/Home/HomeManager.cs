using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private Button matchButton;
    [SerializeField] private Button attackerButton;
    [SerializeField] private Button supporter1Button;
    [SerializeField] private Button supporter2Button;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI matchButtonText;
    public PlayerData playerData;
    private bool is_matching;

    void Start()
    {
        playerData.player_attacker = new Character
        {
            character_id = -1
        };
        playerData.player_supporter1 = new Character
        {
            character_id = -1
        };
        playerData.player_supporter2 = new Character
        {
            character_id = -1
        };
        is_matching = false;
        // イベント登録
        NetworkManager.Instance.OnWaiting += HandleWaiting;
        NetworkManager.Instance.OnMatchFound += HandleMatchFound;
        NetworkManager.Instance.OnMatchCancelled += HandleMatchCancelled;

        if (matchButton != null)
        {
            matchButton.onClick.AddListener(OnMatchButtonClicked);
        }
        if (attackerButton != null)
        {
            attackerButton.onClick.AddListener(() => OnCharacterButtonClicked(0));
        }
        if (supporter1Button != null)
        {
            supporter1Button.onClick.AddListener(() => OnCharacterButtonClicked(1));
        }
        if (supporter2Button != null)
        {
            supporter2Button.onClick.AddListener(() => OnCharacterButtonClicked(2));
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

    void Update()
    {
        if(playerData.player_attacker.character_id == -1 ||
        playerData.player_supporter1.character_id == -1 || 
        playerData.player_supporter2.character_id == -1)
        {
            matchButton.interactable = false;
        }
        else
        {
            matchButton.interactable = true;
        }
    }

    private void OnMatchButtonClicked()
    {
        if(is_matching)
        {
            statusText.text = "cancelling...";
            NetworkManager.Instance.CancelMatching();
            return;
        }
        else
        {
            statusText.text = "data sending...";
            NetworkManager.Instance.StartMatching();
            return;
        }
        
    }

    public void OnCharacterButtonClicked(int i)
    {}

    // サーバーから待機中通知を受信
    private void HandleWaiting(string msg)
    {
        statusText.text = "matching...";
        is_matching = true;
        attackerButton.interactable = false;
        supporter1Button.interactable = false;
        supporter2Button.interactable = false;
        if (matchButtonText != null) matchButtonText.text = "Cancel";
    }

    // サーバーからキャンセル完了通知を受信
    private void HandleMatchCancelled()
    {
        statusText.text = "canceled";
        is_matching = false;
        attackerButton.interactable = true;
        supporter1Button.interactable = true;
        supporter2Button.interactable = true;
        if (matchButtonText != null) matchButtonText.text = "Buttle";
    }

    // 4. マッチング成立時戦闘シーンへ遷移する
    private void HandleMatchFound(GameData data)
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
    }
}
