using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Card : MonoBehaviour
{
    public DrawCard drawCard;
    public GamePlayerData gamePlayerData;
    public void DrawToHund(int num)
    {
        for(int i = 0; i < num; i++)
        {
            drawCard.Draw(gamePlayerData.hunds);
        }
    }
    public void DrawToTemp(List<GameCardData> temp, int num)
    {
        for(int i = 0; i < num; i++)
        {
            drawCard.Draw(temp);
        }
    }
    public int Attack(int attack, int hp)
    {
        int remain_hp = hp - attack;
        if(remain_hp > 0)
        {
            return remain_hp;
        }else
        {
            return 0;
        }
    }
    public int Heal(int heal, int hp, int max_hp)
    {
        int healed_hp = hp + heal;
        if(healed_hp < max_hp)
        {
            return healed_hp;
        }else
        {
            return max_hp;
        }
    }

    public int PumpAttack(int pump, int attack)
    {
        int pumped_attack = attack + pump;
        if(pumped_attack > 0)
        {
            return pumped_attack;
        }else
        {
            return 0;
        }
    }
}
