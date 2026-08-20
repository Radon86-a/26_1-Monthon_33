using UnityEngine;

// ターン切り替えの制御をする
public class TurnManager : MonoBehaviour
{
    public GameData gameData;
    public GamePlayerData gamePlayerData;
    public int rnd;
    void Start()
    {
        DecideTurn();
    }

    public void DecideTurn()
    {
        rnd = Random.Range(1, 2);
        gameData.turn_player_id = rnd;
        CheckIsPlayerTurn();
    }

    public void ChangeTurn()
    {
        if(gameData.turn_player_id == 1)
        {
            gameData.turn_player_id = 2;
            CheckIsPlayerTurn();
            return;
        }
        else if (gameData.turn_player_id == 2)
        {
            gameData.turn_player_id  = 1;
            CheckIsPlayerTurn();
            return;
        }else
        {
            Debug.Log("不明なターンです");
        }
    }

    public void CheckIsPlayerTurn()
    {
        if(gameData.turn_player_id == gamePlayerData.player_id)
        {
            gameData.is_player_turn = true;
        }else
        {
            gameData.is_player_turn = false;
        }
    }

    public void OnEndButtonClick()
    {
        ChangeTurn();
    }
}
