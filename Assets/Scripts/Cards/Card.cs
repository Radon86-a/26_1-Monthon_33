using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Sprite button_image;
    [SerializeField] private TextMeshPro idText;
    public DrawCard drawCard;
    public GamePlayerData gamePlayerData;
    public CardsData cardData;
    public GameData gameData;
    public int card_id;
    public RectTransform RectTransform { get; private set; }

    public int CardId { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        if (button == null) button = GetComponent<Button>();
    }
    /// <summary>
    /// カードの初期設定
    /// </summary>
    /// <param name="id">カードID</param>
    /// <param name="onClickCallback">クリックされたときに呼ばれる処理</param>
    public void Setup(int card_id, Action<int> onClickCallback = null)
    {
        name = $"Card_{CardId}";

        // UI表示の更新
        if (idText != null)
        {
            idText.text = CardId.ToString();
        }

        if(cardData.cardsData[card_id].card_image != null)
        {
            button_image = cardData.cardsData[card_id].card_image;
        }

        // ボタンのクリックイベントを登録
        button.onClick.RemoveAllListeners();
        if (onClickCallback != null)
        {
            button.onClick.AddListener(() => onClickCallback(CardId));
        }
    }

    public void UseCard(int card_id)
    {

        if(cardData.cardsData[card_id].is_attackable)
        {
            Attack.DoAttack(gameData.opponent_data.current_hp, gameData.my_data.atk);

            SyncData.SyncMyState("attack");
        }
        if(cardData.cardsData[card_id].is_healable)
        {
            Heal(cardData.cardsData[card_id].heal_amount, gameData.my_data.current_hp, gameData.my_data.max_hp);
            
            SyncData.SyncMyState("heal");
        }
        if(cardData.cardsData[card_id].is_damageable)
        {}
        if(cardData.cardsData[card_id].is_drawable)
        {}
        if(cardData.cardsData[card_id].is_selective_drawable)
        {}
        if(cardData.cardsData[card_id].is_hand_des)
        {}
        if(cardData.cardsData[card_id].is_equipment)
        {}
    }

    public void DrawToHund(int num)
    {
        for(int i = 0; i < num; i++)
        {
            drawCard.Draw(gamePlayerData.hunds);
        }
    }
    public void DrawToTemp(List<GameCardData> temp, int num)
    {
        for(int i = 0; i < num; i++)
        {
            drawCard.Draw(temp);
        }
    }
    
    public int Heal(int heal, int hp, int max_hp)
    {
        int healed_hp = hp + heal;
        if(healed_hp < max_hp)
        {
            return healed_hp;
        }else
        {
            return max_hp;
        }
    }

    public int PumpAttack(int pump, int attack)
    {
        int pumped_attack = attack + pump;
        if(pumped_attack > 0)
        {
            return pumped_attack;
        }else
        {
            return 0;
        }
    }
}
