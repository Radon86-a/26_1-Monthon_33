using System;

[Serializable]
public class GameData
{
    public string type = "game_state";
    public string room_id;
    public string player_id;
    public string current_turn_player_id;
    public int current_hp;
    public int max_hp;
    public int atk;
    public int hand_count;
    public string action;
}