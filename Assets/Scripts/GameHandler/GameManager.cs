using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public PlayerData playerData;
    public GameData gameData;
    public TurnManager turnManager;
    public GamePlayerData gamePlayerData;
    public CharacterData characterData;

    [Header("自分 (Player)")]
    [SerializeField] private TextMeshProUGUI myHpText;
    [SerializeField] private TextMeshProUGUI myAtkText;

    [Header("相手 (Opponent)")]
    [SerializeField] private TextMeshProUGUI opponentHpText;
    [SerializeField] private TextMeshProUGUI opponentAtkText;

    [Header("バトル状態")]
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private List<Button> cardButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button attackButton;

    // 自分のキャラステータス
    
    private bool IsMyTurn => 
    !string.IsNullOrEmpty(SyncData.sendData.current_turn_player_id) && 
    !string.IsNullOrEmpty(playerData.player_id) &&
    SyncData.sendData.current_turn_player_id == playerData.player_id;

    void Start()
{
    InitializeGame.Initialize(gamePlayerData, playerData, gameData, characterData);
    NetworkManager.Instance.OnBattleStateReceived += HandleBattleState;

    // ボタンのリスナー設定
    for (int i = 0; i < cardButton.Count; i++)
    {
        int index = i;
        cardButton[index].onClick.AddListener(() => OnPlayCardClicked(index));
    }
    endTurnButton.onClick.AddListener(OnEndTurnClicked);
    attackButton.onClick.AddListener(OnAttackClicked);

    // 自分の初期ステータスを格納
    SyncData.sendData.my_current_hp = gameData.my_data.current_hp;
    SyncData.sendData.my_max_hp = gameData.my_data.max_hp;
    SyncData.sendData.my_atk = gameData.my_data.atk;
    SyncData.sendData.hand_count = 4;

    // ★ターン管理の初期化
    if (NetworkManager.Instance.IsFirstPlayer)
    {
        // 先行：自分が最初のターン
        SyncData.sendData.current_turn_player_id = playerData.player_id;
        turnManager.GameStrat(); // 先行側のみゲーム開始処理を実行
    }
    else
    {
        // 後攻：相手（先行側）のIDを初期ターン保持者にする
        SyncData.sendData.current_turn_player_id = NetworkManager.Instance.OpponentId;
    }

    // 初期ステータスを同期送信
    SyncData.SyncMyState("init");

    UpdateUI();
}

    // カード使用ボタン（クライアント側で処理して相手に同期）
    private void OnPlayCardClicked(int hund_card_id)
    {
        if (!IsMyTurn) return;

        // クライアント側でカード処理

        // 状態をサーバーへ送信して相手画面にも反映
        SyncData.SyncMyState("play_card");
    }

    // ターン終了ボタン
    private void OnEndTurnClicked()
    {
        if (!IsMyTurn) return;

        // ターン終了をサーバーへ通知
        SyncData.SyncMyState("end_turn");
    }

    private void OnAttackClicked()
    {
        if (!IsMyTurn) return;
        Attack.DoAttack(gameData.opponent_data.current_hp, gameData.my_data.atk);

        SyncData.SyncMyState("attack");
    }

    // サーバーからデータを受信したときの処理
    private void HandleBattleState(GameData data)
    {
        // 1. ターンの更新と開始トリガーの検知
        if (!string.IsNullOrEmpty(data.current_turn_player_id))
        {
            string lastTurnId = SyncData.sendData.current_turn_player_id;
            SyncData.sendData.current_turn_player_id = data.current_turn_player_id;

            // 相手からターンが渡ってきた瞬間のみ開始処理
            if (lastTurnId != data.current_turn_player_id && IsMyTurn)
            {
                turnManager.StartTurn();
            }
        }

        // 2. サーバー側で反転済みのため、常に my_data が自分、opponent_data が相手
        SyncData.sendData.my_current_hp = data.my_data.current_hp;
        SyncData.sendData.my_max_hp = data.my_data.max_hp;
        SyncData.sendData.my_atk = data.my_data.atk;
        SyncData.sendData.hand_count = data.my_data.hand_count;

        SyncData.sendData.opponent_current_hp = data.opponent_data.current_hp;
        SyncData.sendData.opponent_max_hp = data.opponent_data.max_hp;
        SyncData.sendData.opponent_atk = data.opponent_data.atk;

        // UIの全体更新
        UpdateUI();
    }

    private void UpdateUI()
    {
        // 自分のUI更新
        myHpText.text = $"HP: {SyncData.sendData.my_current_hp} / {SyncData.sendData.my_max_hp}";
        myAtkText.text = $"ATK: {SyncData.sendData.my_atk}";

        // 相手のUI更新（★ここで確実に画面へ反映）
        opponentHpText.text = $"HP: {SyncData.sendData.opponent_current_hp} / {SyncData.sendData.opponent_max_hp}";
        opponentAtkText.text = $"ATK: {SyncData.sendData.opponent_atk}";

        // ターン表示とボタンの活性/非活性
        if (IsMyTurn)
        {
            turnText.text = "<color=green>your turn</color>";
            for (int i = 0; i < cardButton.Count; i++)
            {
                cardButton[i].interactable = true;
            }
            endTurnButton.interactable = true;
            attackButton.interactable = true;
        }
        else
        {
            turnText.text = "<color=red>rival turn</color>";
            for (int i = 0; i < cardButton.Count; i++)
            {
                cardButton[i].interactable = false;
            }
            endTurnButton.interactable = false;
            attackButton.interactable = false;
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

