using UnityEngine;

// 自身のターンの管理をする
public class TurnManager : MonoBehaviour
{
    public Card card;
    public void StartTurn()
    {
        card.DrawToHund(1);
    } 
}
