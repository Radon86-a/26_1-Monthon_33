using System.Collections.Generic;
using JetBrains.Annotations;

public class Card
{
    public DrawCard drawCard;
    public GamePlayerData gamePlayerData;
    public CardData cardData;
    public GameData gameData;
    public int card_id;

    public void UseCard(int card_id)
    {

        if(cardData.is_attackable)
        {
            Attack.DoAttack(gameData.current_hp, gameData.atk);

            SyncData.SyncMyState("attack");
        }
        if(cardData.is_drawable)
        {}
        if(cardData.is_selective_drawable)
        {}
    }

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
