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
    
    private bool IsMyTurn => SyncData.sendData.current_turn_player_id == playerData.player_id;

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

    // 1. 自分の基本初期ステータスを sendData に格納
    SyncData.sendData.my_current_hp = gameData.my_data.current_hp;
    SyncData.sendData.my_max_hp = gameData.my_data.max_hp;
    SyncData.sendData.my_atk = gameData.my_data.atk;
    SyncData.sendData.hand_count = 4; // 初期手札枚数など
    turnManager.GameStrat();

    // 2. 先行プレイヤーのみ初期ターン保持者を自分に設定する（後攻は相手のIDが入るのを待つ）
    if (NetworkManager.Instance.IsFirstPlayer)
    {
        SyncData.sendData.current_turn_player_id = playerData.player_id;
    }

    // 3. ★先行・後攻問わず、自分の初期ステータスを相手に送信する
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
    // ターン更新（先行が送信した init、またはターン終了時のデータから反映）
    if (!string.IsNullOrEmpty(data.current_turn_player_id))
    {
        string lastTurnId = SyncData.sendData.current_turn_player_id;
        SyncData.sendData.current_turn_player_id = data.current_turn_player_id;

        if (lastTurnId != data.current_turn_player_id && IsMyTurn)
        {
            turnManager.StartTurn();
        }
    }

    // データ反映
    if (data.my_data.player_id == playerData.player_id)
    {
        // 自分の確定値
        SyncData.sendData.my_current_hp = data.my_data.current_hp;
        SyncData.sendData.my_max_hp = data.my_data.max_hp;
        SyncData.sendData.my_atk = data.my_data.atk;
        SyncData.sendData.hand_count = data.my_data.hand_count;
    }
    else
    {
        // ★相手の初期データ（init）や最新データが届いたら相手UIを更新
        opponentHpText.text = $"HP: {data.opponent_data.current_hp} / {data.opponent_data.max_hp}";
        opponentAtkText.text = $"ATK: {data.opponent_data.atk}";
        // 相手の手札枚数UIなどがあればここで更新
    }

    UpdateUI();
}

    private void UpdateUI()
    {
        // 自分のUI更新
        myHpText.text = $"HP: {SyncData.sendData.my_current_hp} / {SyncData.sendData.my_max_hp}";
        myAtkText.text = $"ATK: {SyncData.sendData.my_atk}";
        Debug.Log($"手札: {SyncData.sendData.hand_count}枚");

        // ターン表示とボタンの活性/非活性
        if (IsMyTurn)
        {
            turnText.text = "<color=green>your turn</color>";
            for(int i = 0; i < cardButton.Count; i++)
            {
            cardButton[i].interactable = true;
            }
            endTurnButton.interactable = true;
            attackButton.interactable = true;
        }
        else
        {
            turnText.text = "<color=red>rival turn</color>";
            for(int i = 0; i < cardButton.Count; i++)
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

