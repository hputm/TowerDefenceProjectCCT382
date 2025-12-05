using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static int Money;
    public int startMoney = 300;

    void Start ()
    {
        Money = startMoney;
    }
}
