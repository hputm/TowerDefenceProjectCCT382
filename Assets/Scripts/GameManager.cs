using UnityEngine;
using System.Collections;

/// <summary>
/// Manages overall game state, wave progression, and win/lose conditions.
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        WaveInProgress,
        Victory,
        Defeat
    }
    
    [Header("Game Settings")]
    [SerializeField] private int initialLives = 20;
    [SerializeField] private int currentLives;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private float timeBetweenWaves = 5f;
    
    [Header("Game Objects")]
    [SerializeField] private GameObject keep; // The player's main castle
    
    [Header("System References")]
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private EnemySpawner enemySpawner;
    
    private GameState currentState = GameState.Menu;
    // private bool gameStarted = false;  // Not currently used
    
    // Events for game state changes
    public System.Action<GameState> onGameStateChanged;
    public System.Action<int> onWaveChanged;
    public System.Action<int> onLivesChanged;
    
    private static GameManager _instance;
    public static GameManager Instance 
    { 
        get 
        { 
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return _instance; 
        } 
    }
    
    private void Awake()
    {
        // Singleton pattern implementation
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize references if not set
        if (resourceManager == null)
            resourceManager = ResourceManager.Instance;
            
        if (enemySpawner == null)
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }
    
    private void Start()
    {
        currentLives = initialLives;
        ChangeState(GameState.Menu);
    }
    
    #region Game State Management
    
    /// <summary>
    /// Changes the current game state
    /// </summary>
    /// <param name="newState">The new game state</param>
    public void ChangeState(GameState newState)
    {
        GameState previousState = currentState;
        currentState = newState;
        
        // Handle state entry logic
        switch (newState)
        {
            case GameState.Menu:
                HandleMenuState();
                break;
            case GameState.Playing:
                HandlePlayingState();
                break;
            case GameState.Paused:
                HandlePausedState();
                break;
            case GameState.WaveInProgress:
                HandleWaveInProgressState();
                break;
            case GameState.Victory:
                HandleVictoryState();
                break;
            case GameState.Defeat:
                HandleDefeatState();
                break;
        }
        
        onGameStateChanged?.Invoke(newState);
        Debug.Log($"Game state changed from {previousState} to {newState}");
    }
    
    private void HandleMenuState()
    {
        // Pause game systems
        Time.timeScale = 0f;
        // Future UI implementation would show main menu
    }
    
    private void HandlePlayingState()
    {
        // Resume game systems
        Time.timeScale = 1f;
        // Future UI implementation would show game HUD
    }
    
    private void HandlePausedState()
    {
        // Pause game systems
        Time.timeScale = 0f;
        // Future UI implementation would show pause menu
    }
    
    private void HandleWaveInProgressState()
    {
        // Ensure game is running
        Time.timeScale = 1f;
        // Future implementation would prevent starting new waves
    }
    
    private void HandleVictoryState()
    {
        // Stop all game systems
        Time.timeScale = 0f;
        // Future UI implementation would show victory screen
        Debug.Log("Victory! You have successfully defended the keep!");
    }
    
    private void HandleDefeatState()
    {
        // Stop all game systems
        Time.timeScale = 0f;
        // Future UI implementation would show defeat screen
        Debug.Log("Defeat! The keep has fallen.");
    }
    
    /// <summary>
    /// Get current game state
    /// 获取当前游戏状态
    /// </summary>
    public GameState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// Get current wave number
    /// 获取当前波次
    /// </summary>
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    /// <summary>
    /// Get current lives
    /// 获取当前生命值
    /// </summary>
    public int GetLives()
    {
        return currentLives;
    }
    
    #endregion
    
    #region Game Flow Methods
    
    /// <summary>
    /// Starts the game from the menu
    /// </summary>
    public void StartGame()
    {
        if (currentState == GameState.Menu || currentState == GameState.Paused)
        {
            currentLives = initialLives;
            currentWave = 0;
            onLivesChanged?.Invoke(currentLives);
            ChangeState(GameState.Playing);
            StartNextWave();
        }
    }
    
    /// <summary>
    /// Start next wave of enemies
    /// 开始下一波敌人
    /// </summary>
    public void StartNextWave()
    {
        if (currentState == GameState.Playing)
        {
            var spawner = FindFirstObjectByType<EnemySpawner>();
            if (spawner != null)
            {
                // Increment wave counter
                currentWave++;
                onWaveChanged?.Invoke(currentWave);
                
                // Start the new wave
                spawner.SetCurrentWave(currentWave);
                spawner.StartWave();
            }
        }
    }
    
    /// <summary>
    /// Pause the game
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            onGameStateChanged?.Invoke(currentState);
            Time.timeScale = 0f;
        }
    }
    
    /// <summary>
    /// Resume the game
    /// 继续游戏
    /// </summary>
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            onGameStateChanged?.Invoke(currentState);
            Time.timeScale = 1f;
        }
    }
    
    /// <summary>
    /// Return to main menu
    /// 返回主菜单
    /// </summary>
    public void ReturnToMenu()
    {
        currentState = GameState.Menu;
        onGameStateChanged?.Invoke(currentState);
        Time.timeScale = 1f;
        
        // Reset game state
        currentWave = 0;
        currentLives = initialLives;
        
        // Clean up existing enemies and towers
        CleanupGameObjects();
    }
    
    /// <summary>
    /// Clean up game objects between games
    /// 清理游戏对象
    /// </summary>
    void CleanupGameObjects()
    {
        // Clean up enemies
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        
        // Clean up buildings (except static ones like castle)
        var buildings = FindObjectsByType<BuildingBase>(FindObjectsSortMode.None);
        foreach (var building in buildings)
        {
            if (!building.isStaticBuilding)
            {
                Destroy(building.gameObject);
            }
        }
    }
    
    /// <summary>
    /// Called when a wave is completed
    /// </summary>
    public void OnWaveCompleted()
    {
        if (currentState == GameState.WaveInProgress)
        {
            ChangeState(GameState.Playing);
            Debug.Log($"Wave {currentWave} completed!");
            
            // Reward player for completing wave
            if (resourceManager != null)
            {
                int waveReward = currentWave * 50; // Example reward formula
                resourceManager.AddGold(waveReward);
                Debug.Log($"Received {waveReward} gold for completing wave {currentWave}");
            }
            
            // Schedule next wave
            Invoke(nameof(StartNextWave), timeBetweenWaves);
        }
    }
    
    #endregion
    
    #region Game Events
    
    /// <summary>
    /// Called when an enemy reaches the keep
    /// </summary>
    public void OnEnemyReachedKeep()
    {
        currentLives--;
        onLivesChanged?.Invoke(currentLives);
        
        Debug.Log($"Enemy reached the keep! Lives remaining: {currentLives}");
        
        if (currentLives <= 0)
        {
            ChangeState(GameState.Defeat);
        }
    }
    
    /// <summary>
    /// Called when the keep is destroyed
    /// </summary>
    public void OnKeepDestroyed()
    {
        ChangeState(GameState.Defeat);
    }
    
    /// <summary>
    /// Called when all enemies are defeated and objectives are met
    /// </summary>
    public void OnVictoryConditionMet()
    {
        ChangeState(GameState.Victory);
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Gets the keep object
    /// </summary>
    /// <returns>Keep GameObject</returns>
    public GameObject GetKeep()
    {
        return keep;
    }
    
    #endregion
}