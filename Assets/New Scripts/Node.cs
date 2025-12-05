using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Color notEnoughMoney;
    public Vector3 positionOffset;

    public GameObject tower;

    private Renderer rend;
    private Color startColor;

    BuildManager buildManager;

    void Start ()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.instance;
    }

    public Vector3 GetBuildPosition ()
	{
		return transform.position + positionOffset;
	}


    void OnMouseDown ()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (tower != null)
        {
            buildManager.SelectNode(this);
            return;
        }
        if (!buildManager.CanBuild)
        {
            return;
        }
        BuildTower(buildManager.GetTowerToBuild());
    }

    void BuildTower(TowerBlueprint blueprint)
    {
        Vector3 currentOffset;
        if (blueprint.prefab == buildManager.arrowTowerPrefab)
        {
            currentOffset = new Vector3(0f, 5.65f, 0f);
        }
        else
        {
            currentOffset = positionOffset;

        }

        if (PlayerManager.Money < blueprint.cost)
        {
            Debug.Log("Not enough money to build that!");
            return;
        }

        PlayerManager.Money -= blueprint.cost;

        GameObject _tower = (GameObject)Instantiate(blueprint.prefab, transform.position + currentOffset, Quaternion.identity);

        tower = _tower;

        Debug.Log("Tower build!");

    }

    void OnMouseEnter ()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!buildManager.CanBuild)
        {
            return;
        }
        if (buildManager.HasMoney)
        {
            rend.material.color = hoverColor;
        } else
        {
            rend.material.color = notEnoughMoney;
        }
    }

    void OnMouseExit ()
    {
        rend.material.color = startColor;
    }
    
}
