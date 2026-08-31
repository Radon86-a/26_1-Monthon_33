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
    public int card_id;
    public string card_name;
    public Sprite card_image;

    [Header("カードの性質")]
    public bool is_attackable;
    public bool is_drawable;
    public bool is_selective_drawable;

    [Header("カードの効果")]
    public int pump_amount;
    public int heal_amount;
    public int draw_num;
    public int select_num;
}