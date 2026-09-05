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

    // リスナー設定
    for (int i = 0; i < cardButton.Count; i++)
    {
        int index = i;
        cardButton[index].onClick.AddListener(() => OnPlayCardClicked(index));
    }
    endTurnButton.onClick.AddListener(OnEndTurnClicked);
    attackButton.onClick.AddListener(OnAttackClicked);

    // 自分の初期ステータスを準備
    SyncData.sendData.my_current_hp = gameData.my_data.current_hp;
    SyncData.sendData.my_max_hp = gameData.my_data.max_hp;
    SyncData.sendData.my_atk = gameData.my_data.atk;
    SyncData.sendData.hand_count = 4;

    // 準備完了待ちUI
    turnText.text = "Waiting for players...";
    SetActionButtonsInteractable(false);

    // ★サーバーへ「準備完了」と「自分の初期ステータス」を送信して待機
    SyncData.SyncMyState("ready");
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
    // ★サーバーから「2人揃って戦闘開始」が届いたときの一括初期化
    if (data.action == "battle_start")
    {
        Debug.Log("<color=green>[Battle] 両者の準備が完了しました。戦闘を開始します。</color>");

        // 相手データ・自分データを確実に取得
        SyncData.sendData.my_current_hp = data.my_data.current_hp;
        SyncData.sendData.my_max_hp = data.my_data.max_hp;
        SyncData.sendData.my_atk = data.my_data.atk;

        SyncData.sendData.opponent_current_hp = data.opponent_data.current_hp;
        SyncData.sendData.opponent_max_hp = data.opponent_data.max_hp;
        SyncData.sendData.opponent_atk = data.opponent_data.atk;

        SyncData.sendData.current_turn_player_id = data.current_turn_player_id;

        // 先行側ならゲーム開始ロジックを実行
        if (IsMyTurn)
        {
            turnManager.GameStrat();
        }

        UpdateUI();
        return;
    }

    // 1. 通常ターン切り替え処理
    if (!string.IsNullOrEmpty(data.current_turn_player_id))
    {
        string lastTurnId = SyncData.sendData.current_turn_player_id;
        SyncData.sendData.current_turn_player_id = data.current_turn_player_id;

        if (lastTurnId != data.current_turn_player_id && IsMyTurn)
        {
            turnManager.StartTurn();
        }
    }

    // 2. 通常アクションのステータス同期
    bool isFromMe = (data.player_id == playerData.player_id) || 
                    (data.my_data.player_id == playerData.player_id);

    if (isFromMe)
    {
        SyncData.sendData.my_current_hp = data.my_data.current_hp;
        SyncData.sendData.my_max_hp = data.my_data.max_hp;
        SyncData.sendData.my_atk = data.my_data.atk;
        SyncData.sendData.hand_count = data.my_data.hand_count;
    }
    else
    {
        SyncData.sendData.opponent_current_hp = data.my_data.current_hp;
        SyncData.sendData.opponent_max_hp = data.my_data.max_hp;
        SyncData.sendData.opponent_atk = data.my_data.atk;
    }

    UpdateUI();
}

    private void UpdateUI()
    {
        // 自分のUI更新
        myHpText.text = $"HP: {SyncData.sendData.my_current_hp} / {SyncData.sendData.my_max_hp}";
        myAtkText.text = $"ATK: {SyncData.sendData.my_atk}";

        // 相手のUI更新
        opponentHpText.text = $"HP: {SyncData.sendData.opponent_current_hp} / {SyncData.sendData.opponent_max_hp}";
        opponentAtkText.text = $"ATK: {SyncData.sendData.opponent_atk}";

        // ターン表示制御
        if (IsMyTurn)
        {
            turnText.text = "<color=green>your turn</color>";
            SetActionButtonsInteractable(true);
        }
        else
        {
            turnText.text = "<color=red>rival turn</color>";
            SetActionButtonsInteractable(false);
        }
    }

    private void SetActionButtonsInteractable(bool interactable)
    {
        if (cardButton != null)
        {
            for (int i = 0; i < cardButton.Count; i++)
            {
                if (cardButton[i] != null)
                {
                    cardButton[i].interactable = interactable;
                }
            }
        }

        if (endTurnButton != null) endTurnButton.interactable = interactable;
        if (attackButton != null) attackButton.interactable = interactable;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnBattleStateReceived -= HandleBattleState;
        }
    }
}

