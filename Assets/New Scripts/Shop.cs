using UnityEngine;

public class Shop : MonoBehaviour
{
    public GameObject arrowTower;
    public GameObject sniperTower;
    public GameObject machineTower;

    BuildManager buildManager;

    void Start ()
    {
        buildManager = BuildManager.instance;
    }
    public void SelectArrowTower ()
    {
        Debug.Log("Arrow Tower Selected");
        buildManager.SelectTowerToBuild(arrowTower);
    }

    public void SelectSniperTower ()
    {
        Debug.Log("Sniper Tower Selected");
        buildManager.SelectTowerToBuild(sniperTower);
    }

    public void SelectMachineTower ()
    {
        Debug.Log("Machine Tower Selected");
        buildManager.SelectTowerToBuild(machineTower);
    }
}
