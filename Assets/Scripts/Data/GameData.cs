using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    public HundData[] hunds;
    public DeckData[] deck;
}

[System.Serializable]
public struct HundData
{
    public int card_id;
    public bool is_used;
}

[System.Serializable]
public struct DeckData
{
    public int card_id;
    public bool is_selected;
}