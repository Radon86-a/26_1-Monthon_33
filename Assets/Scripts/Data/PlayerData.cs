using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string player_name;
    public string player_id;
    public List<GameCardData> deck;
    public Character player_attacker;
    public Character[] player_supporter;
}
