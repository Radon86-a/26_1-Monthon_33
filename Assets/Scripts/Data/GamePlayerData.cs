using UnityEngine;

[CreateAssetMenu(fileName = "GamePlayerData", menuName = "Scriptable Objects/GamePlayerData")]
public class GamePlayerData : ScriptableObject
{
    public HundData[] hunds;
    public int hunds_num;
    public DeckData[] deck;
    [SerializeField] private int _deck_num;
    public int deck_num
    {
        get => _deck_num;
        set { _deck_num = value; }
    }
    public TrushData[] trush;
    public int trush_num;
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

[System.Serializable]
public struct TrushData
{
    public int card_id;
}