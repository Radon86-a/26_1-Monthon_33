using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private Button matchButton;
    [SerializeField] private TextMeshProUGUI statusText;

    async void Start()
    {

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
}
