using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages game resources such as gold, wood, stone, and population.
/// Currently focused on gold management with interfaces for future expansion.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    [Header("Resource Amounts")]
    [SerializeField] private int gold = 100; // Starting gold amount
    [SerializeField] private int wood = 0;
    [SerializeField] private int stone = 0;
    
    [Header("Population Stats")]
    [SerializeField] private int totalPopulation = 10;
    [SerializeField] private int availablePopulation = 10;
    [SerializeField] private int militiaCount = 0;
    
    // Events to notify other systems when resources change (using UnityEvent for Inspector visibility)
    public UnityEvent<int> onGoldChanged;
    public UnityEvent<int> onWoodChanged;
    public UnityEvent<int> onStoneChanged;
    public UnityEvent<int, int> onPopulationChanged; // total, available
    
    private static ResourceManager _instance;
    public static ResourceManager Instance 
    { 
        get 
        { 
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ResourceManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("ResourceManager");
                    _instance = singletonObject.AddComponent<ResourceManager>();
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
        
        // Initialize UnityEvents if they haven't been created in the inspector
        if (onGoldChanged == null) onGoldChanged = new UnityEvent<int>();
        if (onWoodChanged == null) onWoodChanged = new UnityEvent<int>();
        if (onStoneChanged == null) onStoneChanged = new UnityEvent<int>();
        if (onPopulationChanged == null) onPopulationChanged = new UnityEvent<int, int>();
    }
    
    #region Gold Management
    
    /// <summary>
    /// Gets the current gold amount
    /// </summary>
    /// <returns>Current gold amount</returns>
    public int GetGold()
    {
        return gold;
    }
    
    /// <summary>
    /// Adds gold to the resource pool
    /// </summary>
    /// <param name="amount">Amount of gold to add</param>
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        
        gold += amount;
        onGoldChanged?.Invoke(gold);
        Debug.Log($"Added {amount} gold. Total: {gold}");
    }
    
    /// <summary>
    /// Attempts to spend gold. Returns true if successful.
    /// </summary>
    /// <param name="amount">Amount of gold to spend</param>
    /// <returns>True if there's enough gold, false otherwise</returns>
    public bool SpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold >= amount)
        {
            gold -= amount;
            onGoldChanged?.Invoke(gold);
            Debug.Log($"Removed {amount} gold. Remaining: {gold}");
            return true;
        }
        return false;
    }
    
    #endregion
    
    #region Other Resources (Placeholder for future implementation)
    
    /// <summary>
    /// Gets the current wood amount
    /// </summary>
    /// <returns>Current wood amount</returns>
    public int GetWood()
    {
        return wood;
    }
    
    /// <summary>
    /// Adds wood to the resource pool
    /// </summary>
    /// <param name="amount">Amount of wood to add</param>
    public void AddWood(int amount)
    {
        if (amount <= 0) return;
        
        wood += amount;
        onWoodChanged?.Invoke(wood);
        Debug.Log($"Added {amount} wood. Total: {wood}");
    }
    
    /// <summary>
    /// Attempts to spend wood. Returns true if successful.
    /// </summary>
    /// <param name="amount">Amount of wood to spend</param>
    /// <returns>True if there's enough wood, false otherwise</returns>
    public bool SpendWood(int amount)
    {
        if (amount <= 0) return true;
        if (wood >= amount)
        {
            wood -= amount;
            onWoodChanged?.Invoke(wood);
            Debug.Log($"Removed {amount} wood. Remaining: {wood}");
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Gets the current stone amount
    /// </summary>
    /// <returns>Current stone amount</returns>
    public int GetStone()
    {
        return stone;
    }
    
    /// <summary>
    /// Adds stone to the resource pool
    /// </summary>
    /// <param name="amount">Amount of stone to add</param>
    public void AddStone(int amount)
    {
        if (amount <= 0) return;
        
        stone += amount;
        onStoneChanged?.Invoke(stone);
        Debug.Log($"Added {amount} stone. Total: {stone}");
    }
    
    /// <summary>
    /// Attempts to spend stone. Returns true if successful.
    /// </summary>
    /// <param name="amount">Amount of stone to spend</param>
    /// <returns>True if there's enough stone, false otherwise</returns>
    public bool SpendStone(int amount)
    {
        if (amount <= 0) return true;
        if (stone >= amount)
        {
            stone -= amount;
            onStoneChanged?.Invoke(stone);
            Debug.Log($"Removed {amount} stone. Remaining: {stone}");
            return true;
        }
        return false;
    }
    
    #endregion
    
    #region Population Management
    
    /// <summary>
    /// Gets the total population count
    /// </summary>
    /// <returns>Total population</returns>
    public int GetTotalPopulation()
    {
        return totalPopulation;
    }
    
    /// <summary>
    /// Gets the available population count (not assigned to militia)
    /// </summary>
    /// <returns>Available population</returns>
    public int GetAvailablePopulation()
    {
        return availablePopulation;
    }
    
    /// <summary>
    /// Gets the current militia count
    /// </summary>
    /// <returns>Militia count</returns>
    public int GetMilitiaCount()
    {
        return militiaCount;
    }
    
    /// <summary>
    /// Increases the total population
    /// </summary>
    /// <param name="amount">Amount to increase by</param>
    public void IncreasePopulation(int amount)
    {
        if (amount <= 0) return;
        
        totalPopulation += amount;
        availablePopulation += amount;
        onPopulationChanged?.Invoke(totalPopulation, availablePopulation);
        Debug.Log($"Population changed - Total: {totalPopulation}, Available: {availablePopulation}");
    }
    
    /// <summary>
    /// Recruits militia from available population
    /// </summary>
    /// <param name="amount">Number of militia to recruit</param>
    /// <returns>True if successful, false if not enough available population</returns>
    public bool RecruitMilitia(int amount)
    {
        if (amount <= 0) return true;
        if (availablePopulation >= amount)
        {
            availablePopulation -= amount;
            militiaCount += amount;
            onPopulationChanged?.Invoke(totalPopulation, availablePopulation);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Disbands militia back to available population
    /// </summary>
    /// <param name="amount">Number of militia to disband</param>
    public void DisbandMilitia(int amount)
    {
        if (amount <= 0) return;
        
        int actualDisband = Mathf.Min(amount, militiaCount);
        militiaCount -= actualDisband;
        availablePopulation += actualDisband;
        onPopulationChanged?.Invoke(totalPopulation, availablePopulation);
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Displays current resource status in console (for debugging)
    /// </summary>
    public void DebugResources()
    {
        Debug.Log($"Resources - Gold: {gold}, Wood: {wood}, Stone: {stone}");
        Debug.Log($"Population - Total: {totalPopulation}, Available: {availablePopulation}, Militia: {militiaCount}");
    }
    
    #endregion
}