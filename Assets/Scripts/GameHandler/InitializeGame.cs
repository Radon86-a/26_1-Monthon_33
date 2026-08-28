using System;

[Serializable]
public class InitializeGame
{
    public void Initialize(GamePlayerData gamePlayerData, PlayerData playerData)
    {
        gamePlayerData.game_deck = playerData.deck;
        gamePlayerData.hunds.Clear();
        gamePlayerData.trush.Clear();
    }
}
