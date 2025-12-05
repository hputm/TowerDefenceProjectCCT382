using UnityEngine;
using System.Collections;

public class GameManage : MonoBehaviour
{
    public static bool gameEnded = false;

    public GameObject gameOverUI;
    
    public GameObject gameWonUI;
   
    void Update()
    {
        if (gameEnded)
        {
            return;
        }

        if (PlayerManager.Lives <= 0)
        {
            EndGame();
            return;
        }

        if (Mathf.FloorToInt(TimeSurvived.timeRemaining) <= 0f)
        {
            GameWon();
            return;
        }
        
    }
    void EndGame ()
    {
        gameEnded = true;
        Debug.Log("Game Over!");

        gameOverUI.SetActive(true);
    }

    void GameWon()
    {
        gameEnded = true;
        Debug.Log("You Won!");
        gameWonUI.SetActive(true);
    }
}
