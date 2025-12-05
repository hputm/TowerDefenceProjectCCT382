using UnityEngine;

public class Node : MonoBehaviour
{
    public Color hoverColor;
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


    void OnMouseDown ()
    {
        if (!buildManager.CanBuild)
        {
            return;
        }
        if (tower != null)
        {
            Debug.Log("Can't build there! - display on screen.");
            return;
        }
        buildManager.BuildTowerOn(this);
    }

    void OnMouseEnter ()
    {
        if (!buildManager.CanBuild)
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
