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

    public GameObject standardTowerPrefab;

    void Start ()
    {
        towerToBuild = standardTowerPrefab;
    }

    private GameObject towerToBuild;

    public GameObject GetTowerToBuild ()
    {
        return towerToBuild;
    }
}
