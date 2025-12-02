using UnityEngine;

/// <summary>
/// Base class for all buildings in the game.
/// Contains the fundamental properties required for all building types:
/// - Health system
/// - Attack properties (range, speed, damage)
/// - Placement logic
/// - Upgrade system
/// </summary>
public class BuildingBase : MonoBehaviour, IDamageable
{
    [Header("Building Identification")]
    [Tooltip("Unique identifier for this building type")]
    public string buildingType = "Generic Building";
    
    [Tooltip("Whether this building can attack enemies")]
    public bool canAttack = true;
    
    [Header("Health System")]
    [Tooltip("Maximum health of the building")]
    public float maxHealth = 100f;
    
    [Tooltip("Current health of the building")]
    [HideInInspector] public float currentHealth;
    
    [Header("Attack Properties")]
    [Tooltip("Attack range of the building")]
    public float attackRange = 5f;
    
    [Tooltip("Attack speed (attacks per second)")]
    public float attackSpeed = 1f;
    
    [Tooltip("Damage per attack")]
    public float attackDamage = 10f;
    
    [Header("Placement Constraints")]
    [Tooltip("Where this building can be placed")]
    public PlacementType placementType = PlacementType.Anywhere;
    
    [Tooltip("Whether this building can be manually placed by the player")]
    public bool isPlayerPlaceable = true;
    
    [Tooltip("Whether this building is a static part of the level (like the castle)")]
    public bool isStaticBuilding = false;
    
    // Reference to the grid cell this building is placed on
    [HideInInspector] public GridCell gridCell;
    
    // Internal variables
    protected bool isInitialized = false;
    protected float attackCooldown = 0f;
    protected Transform targetEnemy;
    
    // Events
    public System.Action<BuildingBase> onBuildingDestroyed;
    public System.Action<float> onHealthChanged;
    
    public enum PlacementType
    {
        Anywhere,           // Can be placed anywhere
        RoadOnly,           // Can only be placed on roads
        RoadEndsOnly,       // Can only be placed at road ends
        WaterOnly,          // Can only be placed on water (for future use)
        ImpassableOnly      // Can only be placed on impassable terrain (for future use)
    }
    
    protected virtual void Start()
    {
        InitializeBuilding();
    }
    
    /// <summary>
    /// Initialize the building with default values
    /// </summary>
    protected virtual void InitializeBuilding()
    {
        currentHealth = maxHealth;
        isInitialized = true;
    }
    
    protected virtual void Update()
    {
        if (!isInitialized) return;
        
        // Update attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }
    
    #region Health System
    
    /// <summary>
    /// Apply damage to the building
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Clamp to minimum 0
        
        onHealthChanged?.Invoke(currentHealth);
        
        // Check if building is destroyed
        if (currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }
    
    /// <summary>
    /// Heal the building
    /// </summary>
    /// <param name="amount">Amount of health to restore</param>
    public virtual void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Clamp to max health
        
        onHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// Destroy the building
    /// </summary>
    protected virtual void DestroyBuilding()
    {
        Debug.Log($"{buildingType} destroyed at {transform.position}!", this);
        
        // Remove building from grid cell
        if (gridCell != null)
        {
            gridCell.RemoveBuilding();
        }
        
        // Notify listeners
        onBuildingDestroyed?.Invoke(this);
        
        // Handle game-specific destruction logic
        OnDestroyed();
        
        // Remove the game object
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Override this method to add specific destruction behavior
    /// </summary>
    protected virtual void OnDestroyed()
    {
        // This method can be overridden by subclasses
        // For example, to spawn particles, play sounds, etc.
    }
    
    /// <summary>
    /// Get the current health percentage (0-1)
    /// </summary>
    /// <returns>Health percentage</returns>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    #endregion
    
    #region Attack System
    
    /// <summary>
    /// Check if the building can attack at this moment
    /// </summary>
    /// <returns>True if the building can attack</returns>
    public virtual bool CanAttack()
    {
        return canAttack && attackCooldown <= 0 && currentHealth > 0;
    }
    
    /// <summary>
    /// Perform an attack on a target
    /// </summary>
    /// <param name="target">Target to attack</param>
    public virtual void Attack(Transform target)
    {
        if (!CanAttack() || target == null) return;
        
        // Apply damage to target
        var enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(attackDamage);
        }
        
        // Reset attack cooldown
        attackCooldown = 1f / attackSpeed;
    }
    
    /// <summary>
    /// Get the attack range of this building
    /// </summary>
    /// <returns>Attack range</returns>
    public float GetAttackRange()
    {
        return attackRange;
    }
    
    /// <summary>
    /// Get the attack damage of this building
    /// </summary>
    /// <returns>Attack damage</returns>
    public float GetAttackDamage()
    {
        return attackDamage;
    }
    
    /// <summary>
    /// Get the attack speed of this building
    /// </summary>
    /// <returns>Attack speed</returns>
    public float GetAttackSpeed()
    {
        return attackSpeed;
    }
    
    #endregion
    
    #region Placement System
    
    /// <summary>
    /// Check if this building can be placed at a specific location
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="normal">Surface normal at the position</param>
    /// <returns>True if the building can be placed at the location</returns>
    public virtual bool CanBePlacedAt(Vector3 position, Vector3 normal)
    {
        // Static buildings cannot be placed
        if (isStaticBuilding) return false;
        
        // Use grid system to check placement
        GridCell cell = GridManager.Instance.GetCell(position);
        if (cell == null) return false;
        
        return cell.CanPlaceBuilding(this);
    }
    
    /// <summary>
    /// Set the grid cell this building is placed on
    /// </summary>
    /// <param name="cell">Grid cell</param>
    public void SetGridCell(GridCell cell)
    {
        gridCell = cell;
    }
    
    #endregion
    
    #region IDamageable Implementation
    
    /// <summary>
    /// Get the current health of this building
    /// </summary>
    /// <returns>Current health</returns>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    /// <summary>
    /// Get the maximum health of this building
    /// </summary>
    /// <returns>Maximum health</returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    /// <summary>
    /// Check if this building is dead/destroyed
    /// </summary>
    /// <returns>True if health is zero or below</returns>
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Get the building type
    /// </summary>
    /// <returns>Building type string</returns>
    public string GetBuildingType()
    {
        return buildingType;
    }
    
    /// <summary>
    /// Check if this building is destroyed
    /// </summary>
    /// <returns>True if building is destroyed</returns>
    public bool IsDestroyed()
    {
        return currentHealth <= 0;
    }
    
    #endregion
}