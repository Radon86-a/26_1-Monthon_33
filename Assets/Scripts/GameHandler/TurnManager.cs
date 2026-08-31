using UnityEngine;

// 自身のターンの管理をする
public class TurnManager : MonoBehaviour
{
    public Card card;
    public DrawCard drawCard;
    public void GameStrat()
    {
        drawCard.ShuffleDeck();
        card.DrawToHund(4);
    }
    public void StartTurn()
    {
        card.DrawToHund(1);
    } 
}
