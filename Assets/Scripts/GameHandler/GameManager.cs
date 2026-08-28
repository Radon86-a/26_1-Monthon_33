using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public PlayerData playerData;
    [Header("自分 (Player)")]
    [SerializeField] private TextMeshProUGUI myHpText;
    [SerializeField] private TextMeshProUGUI myAtkText;
    [SerializeField] private TextMeshProUGUI myHandText;

    [Header("相手 (Opponent)")]
    [SerializeField] private TextMeshProUGUI opponentHpText;
    [SerializeField] private TextMeshProUGUI opponentAtkText;
    [SerializeField] private TextMeshProUGUI opponentHandText;

    [Header("バトル状態")]
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Button playCardButton;
    [SerializeField] private Button endTurnButton;

    // 自分のキャラステータス
    private int maxHp = 100;
    private int currentHp = 100;
    private int atk = 10;
    private int handCount = 5;

    private string currentTurnPlayerId;
    private bool IsMyTurn => currentTurnPlayerId == playerData.player_id;

    void Start()
    {
        NetworkManager.Instance.OnBattleStateReceived += HandleBattleState;

        // ボタンのリスナー設定
        playCardButton.onClick.AddListener(OnPlayCardClicked);
        endTurnButton.onClick.AddListener(OnEndTurnClicked);

        // 先行プレイヤーが初期ターンプレイヤーになる
        if (NetworkManager.Instance.IsFirstPlayer)
        {
            currentTurnPlayerId = playerData.player_id;
            // 初期状態をサーバーへ同期
            SyncMyState("init");
        }
        
        UpdateUI();
    }

    // カード使用ボタン（クライアント側で処理して相手に同期）
    private void OnPlayCardClicked()
    {
        if (!IsMyTurn || handCount <= 0) return;

        // クライアント側でカード処理（例：手札を1枚消費、攻撃力+5）
        handCount--;
        atk += 5;

        // 状態をサーバーへ送信して相手画面にも反映
        SyncMyState("play_card");
    }

    // ターン終了ボタン
    private void OnEndTurnClicked()
    {
        if (!IsMyTurn) return;

        // ターン終了をサーバーへ通知
        SyncMyState("end_turn");
    }

    // 自分の状態を送信
    private void SyncMyState(string actionType)
    {
        GameData data = new GameData
        {
            current_turn_player_id = currentTurnPlayerId,
            current_hp = currentHp,
            max_hp = maxHp,
            atk = atk,
            hand_count = handCount,
            action = actionType
        };

        NetworkManager.Instance.SendBattleState(data);
    }

    // サーバーからデータを受信したときの処理
    private void HandleBattleState(GameData data)
    {
        // ターン更新
        currentTurnPlayerId = data.current_turn_player_id;

        if (data.player_id == playerData.player_id)
        {
            // 自分のデータ反映
            currentHp = data.current_hp;
            maxHp = data.max_hp;
            atk = data.atk;
            handCount = data.hand_count;
        }
        else
        {
            // 相手のデータ反映
            opponentHpText.text = $"HP: {data.current_hp} / {data.max_hp}";
            opponentAtkText.text = $"ATK: {data.atk}";
            opponentHandText.text = $"手札: {data.hand_count}枚";
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        // 自分のUI更新
        myHpText.text = $"HP: {currentHp} / {maxHp}";
        myAtkText.text = $"ATK: {atk}";
        myHandText.text = $"手札: {handCount}枚";

        // ターン表示とボタンの活性/非活性
        if (IsMyTurn)
        {
            turnText.text = "<color=green>あなたのターン</color>";
            playCardButton.interactable = handCount > 0;
            endTurnButton.interactable = true;
        }
        else
        {
            turnText.text = "<color=red>相手のターン</color>";
            playCardButton.interactable = false;
            endTurnButton.interactable = false;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnBattleStateReceived -= HandleBattleState;
        }
    }
}