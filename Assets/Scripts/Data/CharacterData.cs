using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public List<Character> characters;
}

[System.Serializable]
public struct Character
{
    public int character_id;
    public string character_name;
    public int character_hp;
    public int character_atk;
    public List<int> character_card_num;
}