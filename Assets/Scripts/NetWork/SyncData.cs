public class SyncData
{
    // 自分の状態を送信
    public static void SyncMyState(SendData sendData)
    {
        GameData data = new GameData
        {
            current_turn_player_id = sendData.current_turn_player_id,
            current_hp = sendData.current_hp,
            max_hp = sendData.max_hp,
            atk = sendData.atk,
            hand_count = sendData.hand_count,
            action = sendData.action
        };

        NetworkManager.Instance.SendBattleState(data);
    }
}
