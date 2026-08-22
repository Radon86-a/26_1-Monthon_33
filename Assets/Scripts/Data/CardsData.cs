using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsData", menuName = "Scriptable Objects/CardsData")]
public class CardsData : ScriptableObject
{
    public List<CardData> cardsData;
}

[System.Serializable]
public struct CardData
{
    public int card_id;
    public GameObject card;
}