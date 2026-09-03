using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsData", menuName = "Scriptable Objects/CardsData")]
public class CardsData : ScriptableObject
{
    [SerializeField]public List<CardData> cardsData;
}

[System.Serializable]
public struct CardData
{
    [Header("カードの情報")]
    public string card_name;
    public Sprite card_image;

    [Header("カードの性質")]
    public bool is_attackable;
    public bool is_healable;
    public bool is_damageable;
    public bool is_drawable;
    public bool is_selective_drawable;
    public bool is_hand_des;
    public bool is_equipment;

    [Header("カードの効果")]
    public int pump_amount;
    public int heal_amount;
    public int draw_num;
    public int select_num;
    public int my_des_num;
    public int opponent_des_num;
    public int my_damage_amount;
    public int opponent_damage_amount;
}