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
        Debug.Log("Cannon Tower Selected");
        buildManager.SetTowerToBuild(buildManager.cannonTowerPrefab);
    }

    public void PurchaseMachineTower ()
    {
        Debug.Log("Machine Tower Selected");
        buildManager.SetTowerToBuild(buildManager.machineTowerPrefab);
    }
}
