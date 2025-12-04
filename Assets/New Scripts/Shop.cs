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
        Debug.Log("Arrow Tower Purchased");
        buildManager.SetTowerToBuild(buildManager.arrowTowerPrefab);
    }

    public void PurchaseCannonTower ()
    {
        Debug.Log("Cannon Tower Purchased");
        buildManager.SetTowerToBuild(buildManager.cannonTowerPrefab);
    }

    public void PurchaseMachineTower ()
    {
        Debug.Log("Machine Tower Purchased");
        buildManager.SetTowerToBuild(buildManager.machineTowerPrefab);
    }
}
