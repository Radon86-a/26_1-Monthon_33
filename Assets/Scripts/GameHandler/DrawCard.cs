using System.Collections.Generic;
using UnityEngine;

public class DrawCard : MonoBehaviour
{
    public GamePlayerData gamePlayerData;
    public int rnd;
    public void Draw(List<GameCardData> temp)
    {
        if(gamePlayerData.game_deck.Count > 0)
        {
            // game_deckの０番目の要素をtempに加え、game_deckから削除する
            temp.Add
            (new GameCardData{card_id = gamePlayerData.game_deck[0].card_id, is_used = false, is_selected = false});
            gamePlayerData.game_deck.RemoveAt(0);
        }
        else
        {
            ShuffleDeck();
            Draw(temp);
        }
    }

    public void ShuffleDeck()
    {
        gamePlayerData.game_deck.AddRange(gamePlayerData.trush);
        for (int i = gamePlayerData.game_deck.Count - 1; i > 0; i--)
        {
        var j = Random.Range(0, i+1); 
        var temp = gamePlayerData.game_deck[i]; 
        gamePlayerData.game_deck[i] = gamePlayerData.game_deck[j]; 
        gamePlayerData.game_deck[j] = temp;
        }
    }
}
