using UnityEngine;

public class Shop : MonoBehaviour
{
    BuildManager buildManager;

    void Start ()
    {
        buildManager = BuildManager.instance;
    }
    public void PurchaseArrowTower ()
    {
        Debug.Log("Arrow Tower Selected");
        buildManager.SetTowerToBuild(buildManager.arrowTowerPrefab);
    }

    public void PurchaseCannonTower ()
    {
        Debug.Log("Sniper Tower Selected");
        buildManager.SetTowerToBuild(buildManager.sniperTowerPrefab);
    }

    public void PurchaseMachineTower ()
    {
        Debug.Log("Machine Tower Selected");
        buildManager.SetTowerToBuild(buildManager.machineTowerPrefab);
    }
}
