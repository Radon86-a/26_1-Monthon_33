using UnityEngine;

[CreateAssetMenu(fileName = "GameCharacterData", menuName = "Scriptable Objects/GameCharacterData")]
public class GameCharacterData : ScriptableObject
{
    public int max_hp;
    public int hp;
    public int atk;
    public int equiped_card_id;
}