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

    public GameObject cannonTowerPrefab;

    public GameObject machineTowerPrefab;

    private GameObject towerToBuild;

    public GameObject GetTowerToBuild ()
    {
        return towerToBuild;
    }

    public void SetTowerToBuild (GameObject tower)
    {
        towerToBuild = tower;
    }
}
