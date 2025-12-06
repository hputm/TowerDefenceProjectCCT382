using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void Retry ()
    {
        TimeSurvived.startTime = 300f;
        GameManage.gameEnded = false;
        SceneManager.LoadScene(1);

    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

}
