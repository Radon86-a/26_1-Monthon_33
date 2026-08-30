public class Attack
{
    public static int DoAttack(int attack, int hp)
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
}
