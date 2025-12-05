using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Money : MonoBehaviour
{
   public TextMeshProUGUI moneyText;

   void Update () {
	   moneyText.text = "$" + PlayerManager.Money.ToString();
   }
}
