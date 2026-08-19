using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public GamePlayerData gamePlayerData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 山と手札と捨て札の枚数の初期化
        gamePlayerData.deck_num = 40;
        gamePlayerData.hunds_num = 0;
        gamePlayerData.trush_num = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        if(gamePlayerData.deck_num >= 1)
        {
            gamePlayerData.hunds_num ++;
            gamePlayerData.deck_num --;
        }else if(gamePlayerData.deck_num == 0)
        {
            gamePlayerData.hunds_num ++;
            gamePlayerData.deck_num = 40 - gamePlayerData.hunds_num;
            gamePlayerData.trush_num = 0;
        }else
        {
            Debug.Log("不明な枚数です");
        }
    }
}
