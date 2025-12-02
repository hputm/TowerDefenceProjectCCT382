using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Main UI manager for the tower defense game
/// Handles all UI elements including resource display, tower selection, and game state
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Resource Display")]
    [Tooltip("Gold amount text display")]
    public Text goldText;
    
    [Tooltip("Wood amount text display")]
    public Text woodText;
    
    [Tooltip("Current wave text display")]
    public Text waveText;
    
    [Tooltip("Lives/Health text display")]
    public Text livesText;
    
    [Header("Tower Selection Panel")]
    [Tooltip("Container for tower selection buttons")]
    public Transform towerSelectionContainer;
    
    [Tooltip("Arrow Tower button")]
    public Button arrowTowerButton;
    
    [Tooltip("Road Block button")]
    public Button roadBlockButton;
    
    [Tooltip("Defense Building button")]
    public Button defenseBuildingButton;
    
    [Header("Tower Upgrade Panel")]
    [Tooltip("Tower upgrade panel container")]
    public GameObject towerUpgradePanel;
    
    [Tooltip("Upgrade button")]
    public Button upgradeButton;
    
    [Tooltip("Sell button")]
    public Button sellButton;
    
    [Tooltip("Tower info text")]
    public Text towerInfoText;
    
    [Tooltip("Upgrade cost text")]
    public Text upgradeCostText;
    
    [Header("Game State Panels")]
    [Tooltip("Main menu panel")]
    public GameObject mainMenuPanel;
    
    [Tooltip("Game over panel")]
    public GameObject gameOverPanel;
    
    [Tooltip("Win panel")]
    public GameObject winPanel;
    
    [Tooltip("Pause panel")]
    public GameObject pausePanel;
    
    [Header("References")]
    [Tooltip("Resource manager reference")]
    public ResourceManager resourceManager;
    
    [Tooltip("Game manager reference")]
    public GameManager gameManager;
    
    [Tooltip("Building placement system reference")]
    public BuildingPlacementSystem buildingPlacementSystem;
    
    // Current selected tower for upgrades
    private TowerUpgradeSystem selectedTower;
    
    void Start()
    {
        InitializeUI();
        SetupEventListeners();
    }
    
    /// <summary>
    /// Initialize all UI elements
    /// </summary>
    void InitializeUI()
    {
        // Hide all panels except main menu at start
        if (towerUpgradePanel != null) towerUpgradePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        
        // Show main menu
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
    
    /// <summary>
    /// Setup event listeners for resource and game state changes
    /// </summary>
    void SetupEventListeners()
    {
        // Get references if not assigned
        if (resourceManager == null) resourceManager = ResourceManager.Instance;
        if (gameManager == null) gameManager = GameManager.Instance;
        
        // Subscribe to resource change events
        if (resourceManager != null)
        {
            resourceManager.onGoldChanged.AddListener(UpdateGoldDisplay);
            resourceManager.onWoodChanged.AddListener(UpdateWoodDisplay);
        }
        
        // Subscribe to game state change events
        if (gameManager != null)
        {
            gameManager.onGameStateChanged.AddListener(OnGameStateChanged);
            gameManager.onWaveChanged.AddListener(UpdateWaveDisplay);
            gameManager.onLivesChanged.AddListener(UpdateLivesDisplay);
        }
        
        // Initial UI updates
        UpdateResourceDisplays();
        UpdateGameInfoDisplays();
    }
    
    /// <summary>
    /// Update all resource displays
    /// </summary>
    void UpdateResourceDisplays()
    {
        UpdateGoldDisplay();
        UpdateWoodDisplay();
    }
    
    /// <summary>
    /// Update game info displays (wave, lives)
    /// </summary>
    void UpdateGameInfoDisplays()
    {
        UpdateWaveDisplay();
        UpdateLivesDisplay();
    }
    
    /// <summary>
    /// Update gold display
    /// </summary>
    public void UpdateGoldDisplay()
    {
        if (goldText != null && resourceManager != null)
        {
            goldText.text = $"Gold: {resourceManager.GetGold()}";
        }
    }
    
    /// <summary>
    /// Update gold display with specific value
    /// </summary>
    public void UpdateGoldDisplay(int amount)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {amount}";
        }
    }
    
    /// <summary>
    /// Update wood display
    /// </summary>
    public void UpdateWoodDisplay()
    {
        if (woodText != null && resourceManager != null)
        {
            woodText.text = $"Wood: {resourceManager.GetWood()}";
        }
    }
    
    /// <summary>
    /// Update wood display with specific value
    /// </summary>
    public void UpdateWoodDisplay(int amount)
    {
        if (woodText != null)
        {
            woodText.text = $"Wood: {amount}";
        }
    }
    
    /// <summary>
    /// Update wave display
    /// </summary>
    public void UpdateWaveDisplay()
    {
        if (waveText != null && gameManager != null)
        {
            waveText.text = $"Wave: {gameManager.GetCurrentWave()}";
        }
    }
    
    /// <summary>
    /// Update wave display with specific value
    /// </summary>
    public void UpdateWaveDisplay(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {wave}";
        }
    }
    
    /// <summary>
    /// Update lives display
    /// </summary>
    public void UpdateLivesDisplay()
    {
        if (livesText != null && gameManager != null)
        {
            livesText.text = $"Lives: {gameManager.GetLives()}";
        }
    }
    
    /// <summary>
    /// Update lives display with specific value
    /// </summary>
    public void UpdateLivesDisplay(int lives)
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {lives}";
        }
    }
    
    /// <summary>
    /// Handle game state changes
    /// </summary>
    void OnGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Menu:
                ShowMainMenu();
                break;
            case GameManager.GameState.Playing:
                ShowGameUI();
                break;
            case GameManager.GameState.Paused:
                ShowPauseMenu();
                break;
            case GameManager.GameState.GameOver:
                ShowGameOver();
                break;
            case GameManager.GameState.Victory:
                ShowVictory();
                break;
        }
    }
    
    /// <summary>
    /// Show main menu
    /// </summary>
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (towerUpgradePanel != null) towerUpgradePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    
    /// <summary>
    /// Show main game UI
    /// </summary>
    public void ShowGameUI()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (towerUpgradePanel != null) towerUpgradePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    
    /// <summary>
    /// Show pause menu
    /// </summary>
    public void ShowPauseMenu()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
    }
    
    /// <summary>
    /// Show game over screen
    /// </summary>
    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
    
    /// <summary>
    /// Show victory screen
    /// </summary>
    public void ShowVictory()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }
    
    /// <summary>
    /// Start a new game
    /// </summary>
    public void StartNewGame()
    {
        if (gameManager != null)
        {
            gameManager.StartNewGame();
        }
    }
    
    /// <summary>
    /// Resume the paused game
    /// </summary>
    public void ResumeGame()
    {
        if (gameManager != null)
        {
            gameManager.ResumeGame();
        }
    }
    
    /// <summary>
    /// Return to main menu
    /// </summary>
    public void ReturnToMenu()
    {
        if (gameManager != null)
        {
            gameManager.ReturnToMenu();
        }
    }
    
    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Toggle pause state
    /// </summary>
    public void TogglePause()
    {
        if (gameManager != null)
        {
            if (gameManager.GetCurrentState() == GameManager.GameState.Playing)
            {
                gameManager.PauseGame();
            }
            else if (gameManager.GetCurrentState() == GameManager.GameState.Paused)
            {
                gameManager.ResumeGame();
            }
        }
    }
    
    /// <summary>
    /// Start next wave
    /// </summary>
    public void StartNextWave()
    {
        if (gameManager != null)
        {
            gameManager.StartNextWave();
        }
    }
    
    /// <summary>
    /// Select arrow tower for placement
    /// </summary>
    public void SelectArrowTower()
    {
        if (buildingPlacementSystem != null)
        {
            buildingPlacementSystem.SelectBuildingToPlace(BuildingPlacementSystem.BuildingType.ArrowTower);
        }
    }
    
    /// <summary>
    /// Select road block for placement
    /// </summary>
    public void SelectRoadBlock()
    {
        if (buildingPlacementSystem != null)
        {
            buildingPlacementSystem.SelectBuildingToPlace(BuildingPlacementSystem.BuildingType.RoadBlock);
        }
    }
    
    /// <summary>
    /// Select defense building for placement
    /// </summary>
    public void SelectDefenseBuilding()
    {
        if (buildingPlacementSystem != null)
        {
            buildingPlacementSystem.SelectBuildingToPlace(BuildingPlacementSystem.BuildingType.DefenseBuilding);
        }
    }
    
    /// <summary>
    /// Update tower upgrade UI when a tower is selected
    /// </summary>
    public void UpdateTowerUpgradeUI(TowerUpgradeSystem tower)
    {
        selectedTower = tower;
        
        if (towerUpgradePanel != null)
        {
            towerUpgradePanel.SetActive(true);
        }
        
        // Update tower info display
        if (towerInfoText != null && tower != null)
        {
            var building = tower.GetComponent<BuildingBase>();
            if (building != null)
            {
                towerInfoText.text = $"Tower: {building.buildingType}\n" +
                                    $"Level: {tower.GetCurrentTier()}\n" +
                                    $"Health: {building.currentHealth}/{building.maxHealth}\n" +
                                    $"Damage: {building.attackDamage}\n" +
                                    $"Range: {building.attackRange}";
            }
        }
        
        // Update upgrade cost display
        if (upgradeCostText != null && tower != null)
        {
            var nextTier = tower.GetNextTier();
            var cost = tower.GetUpgradeCost(nextTier);
            if (cost != null)
            {
                upgradeCostText.text = $"Upgrade Cost:\nGold: {cost.goldRequired}\nWood: {cost.woodRequired}";
            }
            else
            {
                upgradeCostText.text = "Max Level";
            }
        }
        
        // Enable/disable upgrade button based on whether upgrade is possible
        if (upgradeButton != null && tower != null)
        {
            upgradeButton.interactable = tower.CanUpgrade();
        }
    }
    
    /// <summary>
    /// Hide tower upgrade panel
    /// </summary>
    public void HideTowerUpgradePanel()
    {
        if (towerUpgradePanel != null)
        {
            towerUpgradePanel.SetActive(false);
        }
        selectedTower = null;
    }
    
    /// <summary>
    /// Upgrade selected tower
    /// </summary>
    public void UpgradeSelectedTower()
    {
        if (selectedTower != null)
        {
            selectedTower.UpgradeTower();
            UpdateTowerUpgradeUI(selectedTower); // Refresh UI
        }
    }
    
    /// <summary>
    /// Sell selected tower
    /// </summary>
    public void SellSelectedTower()
    {
        if (selectedTower != null)
        {
            selectedTower.SellTower();
            HideTowerUpgradePanel();
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (resourceManager != null)
        {
            resourceManager.onGoldChanged.RemoveListener(UpdateGoldDisplay);
            resourceManager.onWoodChanged.RemoveListener(UpdateWoodDisplay);
        }
        
        if (gameManager != null)
        {
            gameManager.onGameStateChanged.RemoveListener(OnGameStateChanged);
            gameManager.onWaveChanged.RemoveListener(UpdateWaveDisplay);
            gameManager.onLivesChanged.RemoveListener(UpdateLivesDisplay);
        }
    }
}