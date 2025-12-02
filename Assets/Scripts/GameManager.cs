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
    [SerializeField] private int startingLives = 20;
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
                _instance = FindObjectOfType<GameManager>();
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
            enemySpawner = FindObjectOfType<EnemySpawner>();
    }
    
    private void Start()
    {
        currentLives = startingLives;
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
    /// Gets the current game state
    /// </summary>
    /// <returns>Current game state</returns>
    public GameState GetGameState()
    {
        return currentState;
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
            currentLives = startingLives;
            currentWave = 0;
            onLivesChanged?.Invoke(currentLives);
            ChangeState(GameState.Playing);
            StartNextWave();
        }
    }
    
    /// <summary>
    /// Pauses the game
    /// </summary>
    public void PauseGame()
    {
        if (currentState == GameState.Playing || currentState == GameState.WaveInProgress)
        {
            ChangeState(GameState.Paused);
        }
    }
    
    /// <summary>
    /// Resumes the game from pause
    /// </summary>
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
        }
    }
    
    /// <summary>
    /// Starts the next wave of enemies
    /// </summary>
    public void StartNextWave()
    {
        if (currentState != GameState.Playing && currentState != GameState.WaveInProgress)
            return;
            
        currentWave++;
        onWaveChanged?.Invoke(currentWave);
        ChangeState(GameState.WaveInProgress);
        
        // Notify spawner to start wave
        if (enemySpawner != null)
        {
            enemySpawner.StartWave(currentWave);
        }
        
        Debug.Log($"Starting wave {currentWave}");
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
    /// Gets the current wave number
    /// </summary>
    /// <returns>Current wave number</returns>
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    /// <summary>
    /// Gets the current number of lives
    /// </summary>
    /// <returns>Current lives</returns>
    public int GetCurrentLives()
    {
        return currentLives;
    }
    
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