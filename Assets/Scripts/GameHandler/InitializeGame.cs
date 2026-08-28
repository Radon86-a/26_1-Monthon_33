using UnityEngine;

public class InitializeGame : MonoBehaviour
{
    public GameData gameData;
    public GamePlayerData gamePlayerData;
    public PlayerData playerData;
    public TurnManager turnManager;
    void Start()
    {
        gamePlayerData.game_deck = playerData.deck;
        gamePlayerData.hunds.Clear();
        gamePlayerData.trush.Clear();
        turnManager.DecideTurn();
    }
}
