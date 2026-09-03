using System;

// バックエンドに送る用のデータ
[Serializable]
public class GameData
{
    public string type;
    public string room_id;
    public string current_turn_player_id;
    public string action;
    public bool is_first;
    public string message;
    public int des_num;
    public int heal_amount;
    public playerData my_data;
    public playerData opponent_data;
}

[Serializable]
public struct playerData
{
    public string player_id;
    public int current_hp;
    public int max_hp;
    public int atk;
    public int hand_count;
}