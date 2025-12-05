using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    void Awake ()
    {
        if (instance != null)
        {
            Debug.LogError("More than one buildmanager in scene!");
        }
        instance = this;
    }

    public GameObject arrowTowerPrefab;

    public GameObject sniperTowerPrefab;

    public GameObject machineTowerPrefab;

    private TowerBlueprint towerToBuild;

    public bool CanBuild { get { return towerToBuild != null; } }
    public bool HasMoney { get { return PlayerManager.Money >= towerToBuild.cost; } }

    public void BuildTowerOn (Node node)
    {
        Vector3 currentOffset;
        if (towerToBuild.prefab == arrowTowerPrefab)
        {
            currentOffset = new Vector3(0f, 5.65f, 0f);
        }
        else
        {
            currentOffset = node.positionOffset;

        }

        if (PlayerManager.Money < towerToBuild.cost)
        {
            Debug.Log("Not enough money to build that!");
            return;
        }

        PlayerManager.Money -= towerToBuild.cost;

        GameObject tower = (GameObject)Instantiate(towerToBuild.prefab, node.transform.position + currentOffset, Quaternion.identity);

        node.tower = tower;

        Debug.Log("Tower build! Money left: " + PlayerManager.Money);
    }
    
    public void SelectTowerToBuild (TowerBlueprint tower)
    {
        towerToBuild = tower;
    }
}
