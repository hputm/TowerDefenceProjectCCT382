using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;

    public Button upgradeButton;

    private Node target;

    public void SetTarget (Node _target)
    {
        target = _target;

        transform.position = target.GetBuildPosition();

        if (target.isUpgraded) 
        {
            upgradeButton.interactable = false;

        }
        else {
            upgradeButton.interactable = true;
        }
        ui.SetActive(true);
    }

    public void Hide ()
    {
        ui.SetActive(false);
    }

    public void Upgrade ()
    {
        target.UpgradeTower();
        BuildManager.instance.DeselectNode();
    }

}
