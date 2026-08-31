using System;

public class SyncData
{
    public static SendData sendData;

    // 自分の状態を送信
    public static void SyncMyState(string actionType)
    {
        GameData data = new GameData
        {
            type = "game_state", // ★サーバーがルーティングするために必須
            current_turn_player_id = sendData.current_turn_player_id,
            current_hp = sendData.current_hp,
            max_hp = sendData.max_hp,
            atk = sendData.atk,
            hand_count = sendData.hand_count,
            action = actionType,
            des_num = sendData.des_num,         // ★追加変数を反映
            heal_amount = sendData.heal_amount  // ★追加変数を反映
        };

        NetworkManager.Instance.SendBattleState(data);
    }
}

[System.Serializable]
public struct SendData
{
    public string current_turn_player_id;
    public int current_hp;
    public int max_hp;
    public int atk;
    public int hand_count;
    public int des_num;     // ★追加
    public int heal_amount; // ★追加
}