using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject[] card;
    public void MakeCard(int card_id)
    {}
    public List<GameCardData> SortCard(List<GameCardData> list)
    {
        list.Sort((a, b)=> a.card_id.CompareTo(b.card_id));
        return list;
    }
}
