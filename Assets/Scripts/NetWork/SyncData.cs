using System;

public class SyncData
{
    public static SendData sendData;

    // 自分の状態をバックエンドへ送信
    public static void SyncMyState(string actionType)
    {
        GameData data = new GameData
        {
            type = "game_state", 
            current_turn_player_id = sendData.current_turn_player_id,
            my_data = new playerData
            {
                current_hp = sendData.my_current_hp,
                atk = sendData.my_atk,
                max_hp = sendData.my_max_hp,
                hand_count = sendData.hand_count
            },
            opponent_data = new playerData
            {
                current_hp = sendData.opponent_current_hp,
                atk = sendData.opponent_atk,
                max_hp = sendData.opponent_max_hp
            },
            action = actionType,
            des_num = sendData.des_num,
            heal_amount = sendData.heal_amount
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