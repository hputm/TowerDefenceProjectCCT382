using UnityEngine;

/// <summary>
/// Handles tower selection in the game
/// </summary>
public class TowerSelector : MonoBehaviour
{
    [Tooltip("Reference to the UI manager")]
    public UIManager uiManager;
    
    [Tooltip("Reference to the tower upgrade UI")]
    public TowerUpgradeUI towerUpgradeUI;
    
    private Camera mainCamera;
    private BuildingBase selectedBuilding;
    
    void Start()
    {
        mainCamera = Camera.main;
        
        // If no UI managers assigned, try to find them
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        
        if (towerUpgradeUI == null)
        {
            towerUpgradeUI = FindObjectOfType<TowerUpgradeUI>();
        }
    }
    
    void Update()
    {
        // Only handle selection when game is playing
        if (GameManager.Instance != null && 
            GameManager.Instance.GetCurrentState() != GameManager.GameState.Playing)
        {
            return;
        }
        
        // Handle mouse click for tower selection
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }
    
    /// <summary>
    /// Handle mouse click for tower selection
    /// </summary>
    void HandleMouseClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Check if we hit a building
        if (Physics.Raycast(ray, out hit))
        {
            BuildingBase building = hit.collider.GetComponent<BuildingBase>();
            if (building != null)
            {
                SelectBuilding(building);
                return;
            }
        }
        
        // If we didn't hit a building, deselect
        DeselectBuilding();
    }
    
    /// <summary>
    /// Select a building
    /// </summary>
    void SelectBuilding(BuildingBase building)
    {
        // Deselect previous building
        if (selectedBuilding != null && selectedBuilding != building)
        {
            DeselectBuilding();
        }
        
        selectedBuilding = building;
        
        // Notify building it's been selected
        if (selectedBuilding.onBuildingSelected != null)
        {
            selectedBuilding.onBuildingSelected();
        }
        
        // Show upgrade UI for this building if it has an upgrade system
        TowerUpgradeSystem upgradeSystem = building.GetComponent<TowerUpgradeSystem>();
        if (upgradeSystem != null)
        {
            if (uiManager != null)
            {
                uiManager.UpdateTowerUpgradeUI(upgradeSystem);
            }
            else if (towerUpgradeUI != null)
            {
                towerUpgradeUI.UpdateTowerUpgradeUI(upgradeSystem);
            }
        }
    }
    
    /// <summary>
    /// Deselect the currently selected building
    /// </summary>
    void DeselectBuilding()
    {
        selectedBuilding = null;
        
        // Hide upgrade UI
        if (uiManager != null)
        {
            uiManager.HideTowerUpgradePanel();
        }
        else if (towerUpgradeUI != null)
        {
            towerUpgradeUI.ShowBuildSelectionPanel();
        }
    }
}