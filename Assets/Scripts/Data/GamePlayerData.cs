using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GamePlayerData", menuName = "Scriptable Objects/GamePlayerData")]
public class GamePlayerData : ScriptableObject
{
    public List<GameCardData> hunds = new List<GameCardData>();
    public List<GameCardData> game_deck;
    public List<GameCardData> trush;
}

[System.Serializable]
public struct GameCardData
{
    public int card_id;
    public bool is_used;
    public bool is_selected;
}