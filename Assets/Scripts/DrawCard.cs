using UnityEngine;

public class DrawCard : MonoBehaviour
{
    public GamePlayerData gamePlayerData;
    public int rnd;
    public void Draw()
    {
        if(gamePlayerData.deck_num > 0)
        {
            gamePlayerData.hunds.Add(new GameCardData{card_id = 1, is_used = false});
        }
        else
        {
            ShuffleDeck();
            Draw();
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
