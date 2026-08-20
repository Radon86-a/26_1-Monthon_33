using UnityEngine;

// ターン切り替えの制御をする
public class TurnManager : MonoBehaviour
{
    public GameData gameData;
    public int rnd;
    void Start()
    {
        DecideTurn();
    }

    public void DecideTurn()
    {
        rnd = Random.Range(1, 2);
        gameData.turn_player_id = rnd;
    }

    public void ChangeTurn()
    {
        if(gameData.turn_player_id == 1)
        {
            gameData.turn_player_id = 2;
            return;
        }
        else if (gameData.turn_player_id == 2)
        {
            gameData.turn_player_id  = 1;
            return;
        }else
        {
            Debug.Log("不明なターンです");
        }
    }
}
