using System;

[Serializable]
public static class InitializeGame
{
    public static void Initialize
    (GamePlayerData gamePlayerData, PlayerData playerData, GameData gameData, CharacterData characterData)
    {
        gamePlayerData.game_deck = playerData.deck;
        gamePlayerData.hunds.Clear();
        gamePlayerData.trush.Clear();
        gameData.my_data.max_hp = characterData.characters[playerData.player_attacker.character_id].character_hp;
        gameData.my_data.current_hp = characterData.characters[playerData.player_attacker.character_id].character_hp;
        gameData.my_data.atk = characterData.characters[playerData.player_attacker.character_id].character_atk;
    }
}
