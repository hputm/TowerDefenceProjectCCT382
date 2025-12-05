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
    private Node selectedNode;

    public NodeUI nodeUI;

    public bool CanBuild { get { return towerToBuild != null; } }
    public bool HasMoney { get { return PlayerManager.Money >= towerToBuild.cost; } }
    
    public void SelectNode(Node node)
    {
        if (selectedNode == node)
        {
            DeselectNode();
            return;
        }
        selectedNode = node;
        towerToBuild = null;

        nodeUI.SetTarget(node);
    }
    public void DeselectNode()
    {
        selectedNode = null;
        nodeUI.Hide();
    }

    public void SelectTowerToBuild (TowerBlueprint tower)
    {
        towerToBuild = tower;
        DeselectNode();
    }

    public TowerBlueprint GetTowerToBuild()
    {
        return towerToBuild;

    }
}
