using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private Button matchButton;
    [SerializeField] private Button attackerButton;
    [SerializeField] private Button supporter1Button;
    [SerializeField] private Button supporter2Button;
    [SerializeField] private Button shadowButton;
    [SerializeField] private Button[] characterButtons;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI matchButtonText;
    public PlayerData playerData;
    public CharacterData characterData;
    public CardsData cardsData;
    private bool is_matching;
    private int selected_button_id;

    void Start()
    {
        playerData.deck = new List<GameCardData>();
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
        if (shadowButton != null)
        {
            shadowButton.onClick.AddListener(OnShadowButtonClicked);
        }
        for(int i = 0;i < characterButtons.Length; i++)
        {
            if (characterButtons[i] != null)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => OnCharacterButtonsClicked(index));
        }
        }

        shadowButton.gameObject.SetActive(false);

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
    {
        shadowButton.gameObject.SetActive(true);
        selected_button_id = i;
    }

    public void OnShadowButtonClicked()
    {
        shadowButton.gameObject.SetActive(false);
    }
    public void OnCharacterButtonsClicked(int chara_num)
    {
        playerData.deck = new List<GameCardData>();
        switch (selected_button_id)
        {
            case 0:
            playerData.player_attacker = characterData.characters[chara_num];
            MakeDeckList();
            break;
            case 1:
            playerData.player_supporter1 = characterData.characters[chara_num];
            MakeDeckList();
            break;
            case 2:
            playerData.player_supporter2 = characterData.characters[chara_num];
            MakeDeckList();
            break;
            default:
            Debug.Log("未知のIDです");
            break;
        }
        shadowButton.gameObject.SetActive(false);
    }

    public void MakeDeckList()
    {
        if(playerData.player_attacker.character_id >= 0)
        {
            int attacker = playerData.player_attacker.attacker_card.Count;
        for(int i = 0; i < attacker; i++)
        {
            for(int j = 0; j < playerData.player_attacker.attacker_card[i].card_num; j++)
            {
                playerData.deck.Add(new GameCardData
                {
                    card_id = playerData.player_attacker.attacker_card[i].card_id
                });
            }
        }
        }

        if(playerData.player_supporter1.character_id >= 0)
        {
            int supporter1 = playerData.player_supporter1.supporter_card.Count;
        for(int i = 0; i < supporter1; i++)
        {
            for(int j = 0; j < playerData.player_supporter1.supporter_card[i].card_num; j++)
            {
                playerData.deck.Add(new GameCardData
                {
                    card_id = playerData.player_supporter1.supporter_card[i].card_id
                });
            }
        }
        }
        
        if(playerData.player_supporter2.character_id >= 0)
        {
            int supporter2 = playerData.player_supporter2.supporter_card.Count;
        for(int i = 0; i < supporter2; i++)
        {
            for(int j = 0; j < playerData.player_supporter2.supporter_card[i].card_num; j++)
            {
                playerData.deck.Add(new GameCardData
                {
                    card_id = playerData.player_supporter2.supporter_card[i].card_id
                });
            }
        }
        }
        
    }

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
