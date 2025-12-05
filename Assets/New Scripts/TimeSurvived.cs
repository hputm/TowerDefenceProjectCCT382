using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TimeSurvived : MonoBehaviour
{
	public TextMeshProUGUI timerText;

	public static float startTime = 300f;
	public static float timeRemaining;

	void Start ()
	{
		timeRemaining = startTime;
	}


	void Update () {
		if (GameManage.gameEnded)
		{
			return;
		}
		timeRemaining -= Time.deltaTime;

		if (timeRemaining <= 0)
		{
			timeRemaining = 0;
			GameManage.gameEnded = true;
		}
		UpdateDisplay();


	}
	void UpdateDisplay ()
	{
		int seconds = Mathf.FloorToInt(timeRemaining);
		timerText.text = seconds.ToString() + "s";
	}
}
