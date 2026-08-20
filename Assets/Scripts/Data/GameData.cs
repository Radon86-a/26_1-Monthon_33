using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    public int turn_player_id;
    public bool is_player_turn;
}