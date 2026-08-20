using UnityEngine;

[CreateAssetMenu(fileName = "GameCharacterData", menuName = "Scriptable Objects/GameCharacterData")]
public class GameCharacterData : ScriptableObject
{
    public int player_hp;
    public int player_atk;
    public int enemy_hp;
    public int enemy_atk;
}
