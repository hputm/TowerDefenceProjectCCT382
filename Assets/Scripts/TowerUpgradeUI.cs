using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simplified tower upgrade UI manager
/// Handles display of tower models and upgrade options
/// </summary>
public class TowerUpgradeUI : MonoBehaviour
{
    [Header("Resource Display")]
    [Tooltip("Gold amount text display")]
    public Text goldText;
    
    [Tooltip("Resource manager reference")]
    public ResourceManager resourceManager;
    
    [Header("UI Panels")]
    [Tooltip("Tower upgrade panel")]
    public GameObject towerUpgradePanel;
    
    [Tooltip("Build selection panel")]
    public GameObject buildSelectionPanel;
    
    [Header("Tower Info")]
    [Tooltip("Tower info text")]
    public Text towerInfoText;
    
    [Tooltip("Upgrade cost text")]
    public Text upgradeCostText;
    
    [Tooltip("Upgrade button")]
    public Button upgradeButton;
    
    // Current selected tower
    private TowerUpgradeSystem selectedTower;
    
    void Start()
    {
        // 初始化UI / Initialize UI
        InitializeUI();
        
        // 获取资源管理器实例 / Get resource manager instance
        if (resourceManager == null)
        {
            resourceManager = ResourceManager.Instance;
        }
        
        // 订阅资源变更事件 / Subscribe to resource change events
        if (resourceManager != null)
        {
            resourceManager.onGoldChanged.AddListener(UpdateGoldDisplay);
        }
        
        // 初始更新金币显示 / Initial gold display update
        UpdateGoldDisplay();
    }
    
    /// <summary>
    /// 初始化UI元素 / Initialize UI elements
    /// </summary>
    void InitializeUI()
    {
        // 确保面板初始状态正确 / Ensure panels are in correct initial state
        if (towerUpgradePanel != null)
            towerUpgradePanel.SetActive(false);
            
        if (buildSelectionPanel != null)
            buildSelectionPanel.SetActive(true);
    }
    
    /// <summary>
    /// 更新塔升级UI / Update tower upgrade UI
    /// </summary>
    /// <param name="tower">要更新的塔 / Tower to update</param>
    public void UpdateTowerUpgradeUI(TowerUpgradeSystem tower)
    {
        selectedTower = tower;
        
        if (towerUpgradePanel != null)
        {
            towerUpgradePanel.SetActive(true);
        }
        
        if (buildSelectionPanel != null)
        {
            buildSelectionPanel.SetActive(false);
        }
        
        // 在这里可以添加更多UI更新逻辑
        // Here you can add more UI update logic
        Debug.Log("Tower upgrade UI updated for: " + (tower != null ? tower.name : "null"));
    }
    
    public void ShowBuildSelectionPanel()
    {
        if (towerUpgradePanel != null)
        {
            towerUpgradePanel.SetActive(false);
        }
        
        if (buildSelectionPanel != null)
        {
            buildSelectionPanel.SetActive(true);
        }
        
        selectedTower = null;
    }
    
    public void UpdateGoldDisplay()
    {
        if (goldText != null && resourceManager != null)
        {
            goldText.text = $"Gold: {resourceManager.GetGold()}";
        }
    }
    
    public void UpdateGoldDisplay(int newGoldAmount)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {newGoldAmount}";
        }
    }
    
    public void UpgradeSelectedTower()
    {
        if (selectedTower != null)
        {
            selectedTower.UpgradeTower();
            UpdateTowerUpgradeUI(selectedTower); // Refresh UI
        }
    }
    
    public void SellSelectedTower()
    {
        if (selectedTower != null)
        {
            selectedTower.SellTower();
            ShowBuildSelectionPanel();
        }
    }
    
    void OnDestroy()
    {
        // 取消订阅资源变更事件 / Unsubscribe from resource change events
        if (resourceManager != null)
        {
            resourceManager.onGoldChanged.RemoveListener(UpdateGoldDisplay);
        }
    }
}