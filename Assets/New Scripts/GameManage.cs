using UnityEngine;
using System.Collections;

public class GameManage : MonoBehaviour
{
    public static bool gameEnded = false;

    public GameObject gameOverUI;
   
    void Update()
    {
        if (gameEnded)
        {
            return;
        }

        if (PlayerManager.Lives <= 0)
        {
            EndGame();
        }
        
    }
    void EndGame ()
    {
        gameEnded = true;
        Debug.Log("Game Over!");

        gameOverUI.SetActive(true);
    }
}
