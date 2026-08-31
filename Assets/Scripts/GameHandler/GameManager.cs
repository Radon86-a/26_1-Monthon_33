using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public PlayerData playerData;
    public GameData gameData;

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
        NetworkManager.Instance.OnBattleStateReceived += HandleBattleState;

        // ボタンのリスナー設定
        for(int i = 0; i < cardButton.Count; i++)
        {
            int index = i;
    cardButton[index].onClick.AddListener(() => OnPlayCardClicked(index));
        }
        endTurnButton.onClick.AddListener(OnEndTurnClicked);
        attackButton.onClick.AddListener(OnAttackClicked);

        // 先行プレイヤーが初期ターンプレイヤーになる
        if (NetworkManager.Instance.IsFirstPlayer)
        {
            SyncData.sendData.current_turn_player_id = playerData.player_id;
            // 初期状態をサーバーへ同期
            SyncData.SyncMyState("init");
        }
        
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
        Attack.DoAttack(gameData.current_hp, gameData.atk);

        SyncData.SyncMyState("attack");
    }

    // サーバーからデータを受信したときの処理
    private void HandleBattleState(GameData data)
    {
        // ターン更新
        SyncData.sendData.current_turn_player_id = data.current_turn_player_id;

        if (data.player_id == playerData.player_id)
        {
            // 自分のデータ反映
            SyncData.sendData.current_hp = data.current_hp;
            SyncData.sendData.max_hp = data.max_hp;
            SyncData.sendData.atk = data.atk;
            SyncData.sendData.hand_count = data.hand_count;
        }
        else
        {
            // 相手のデータ反映
            opponentHpText.text = $"HP: {data.current_hp} / {data.max_hp}";
            opponentAtkText.text = $"ATK: {data.atk}";
            Debug.Log($"手札: {data.hand_count}枚");
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        // 自分のUI更新
        myHpText.text = $"HP: {SyncData.sendData.current_hp} / {SyncData.sendData.max_hp}";
        myAtkText.text = $"ATK: {SyncData.sendData.atk}";
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

