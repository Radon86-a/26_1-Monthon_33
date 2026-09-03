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
    public List<CharacterCard> attacker_card;
    public List<CharacterCard> supporter_card;
}

[System.Serializable]
public struct CharacterCard
{
    public int card_id;
    public int card_num;
}