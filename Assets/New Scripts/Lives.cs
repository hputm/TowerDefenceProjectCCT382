using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Lives : MonoBehaviour
{

    public TextMeshProUGUI livesText;

    void Update()
    {
        livesText.text = PlayerManager.Lives.ToString();
        
    }
}
