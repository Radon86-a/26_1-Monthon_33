using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public GameData gameData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 山と手札と捨て札の枚数の初期化
        gameData.deck_num = 40;
        gameData.hunds_num = 0;
        gameData.trush_num = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        if(gameData.deck_num >= 1)
        {
            gameData.hunds_num ++;
            gameData.deck_num --;
        }else if(gameData.deck_num == 0)
        {
            gameData.hunds_num ++;
            gameData.deck_num = 40 - gameData.hunds_num;
            gameData.trush_num = 0;
        }else
        {
            Debug.Log("不明な枚数です");
        }
    }
}
