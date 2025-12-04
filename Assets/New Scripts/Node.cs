using UnityEngine;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Vector3 positionOffset;

    private GameObject tower;

    private Renderer rend;
    private Color startColor;

    BuildManager buildManager;

    void Start ()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.instance;
    }

    void OnMouseDown ()
    {
        if (buildManager.GetTowerToBuild() == null)
        {
            return;
        }
        if (tower != null)
        {
            Debug.Log("Can't build there! - display on screen.");
            return;
        }

        GameObject towerToBuild = buildManager.GetTowerToBuild();
        tower = (GameObject)Instantiate(towerToBuild, transform.position + positionOffset, transform.rotation);
    }

    void OnMouseEnter ()
    {
        if (buildManager.GetTowerToBuild() == null)
        {
            return;
        }
        rend.material.color = hoverColor;
    }

    void OnMouseExit ()
    {
        rend.material.color = startColor;
    }
    
}
