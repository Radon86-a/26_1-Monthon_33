public class SyncData
{
    public static SendData sendData;

    public static void SyncMyState(string actionType)
    {
        GameData data = new GameData
        {
            type = "game_state",
            player_id = NetworkManager.Instance.playerData.player_id,
            current_turn_player_id = sendData.current_turn_player_id,
            action = actionType,
            des_num = sendData.des_num,
            heal_amount = sendData.heal_amount,
            my_data = new playerData
            {
                player_id = NetworkManager.Instance.playerData.player_id,
                current_hp = sendData.my_current_hp,
                max_hp = sendData.my_max_hp,
                atk = sendData.my_atk,
                hand_count = sendData.hand_count
            }
            // opponent_data は相手自身が送ってくるのでここでは空でOK
        };

        NetworkManager.Instance.SendBattleState(data);
    }
}

[System.Serializable]
public struct SendData
{
    public string current_turn_player_id;
    public int my_current_hp;
    public int my_max_hp;
    public int my_atk;
    public int opponent_current_hp;
    public int opponent_max_hp;
    public int opponent_atk;
    public int hand_count;
    public int des_num;
    public int heal_amount;
}