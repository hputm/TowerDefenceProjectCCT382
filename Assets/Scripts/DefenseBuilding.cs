using UnityEngine;

/// <summary>
/// Defense Building implementation.
/// Inherits from BuildingBase and represents defensive structures that can be placed on roads.
/// These buildings can either attack or just block enemies.
/// </summary>
public class DefenseBuilding : BuildingBase
{
    [Header("Defense Building Specific")]
    [Tooltip("Whether this building blocks enemy movement")]
    public new bool blocksMovement = true;
    
    [Tooltip("Whether this building can attack enemies")]
    public bool hasAttackCapability = false;
    
    [Tooltip("Current upgrade tier of the building")]
    public int currentTier = 1;
    
    [Tooltip("Maximum tier this building can reach")]
    public int maxTier = 2;
    
    // Tier-based stats
    [System.Serializable]
    public class TierStats
    {
        public float health = 100f;
        public float attackRange = 2f;
        public float attackSpeed = 0.5f;
        public float attackDamage = 5f;
    }
    
    [Header("Tier Statistics")]
    public TierStats tier1Stats = new TierStats { health = 100f, attackRange = 2f, attackSpeed = 0.5f, attackDamage = 5f };
    public TierStats tier2Stats = new TierStats { health = 200f, attackRange = 2.5f, attackSpeed = 0.7f, attackDamage = 10f };
    
    protected override void Start()
    {
        // Set placement constraint - defense buildings can only be placed on roads
        placementType = PlacementType.RoadOnly;
        buildingType = "Defense Building";
        canAttack = hasAttackCapability;
        
        base.Start();
    }
    
    protected override void InitializeBuilding()
    {
        // Apply tier stats
        ApplyTierStats();
        
        base.InitializeBuilding();
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (!isInitialized) return;
        
        // Only find and attack targets if this building has attack capability
        if (hasAttackCapability)
        {
            FindAndAttackTarget();
        }
    }
    
    /// <summary>
    /// Apply stats based on current tier
    /// </summary>
    protected virtual void ApplyTierStats()
    {
        // Only apply attack stats if this building can attack
        if (hasAttackCapability)
        {
            TierStats stats = GetTierStats(currentTier);
            
            maxHealth = stats.health;
            attackRange = stats.attackRange;
            attackSpeed = stats.attackSpeed;
            attackDamage = stats.attackDamage;
        }
        else
        {
            // For non-attacking defense buildings, only health matters
            TierStats stats = GetTierStats(currentTier);
            maxHealth = stats.health;
        }
        
        // If this is the initial setup, set current health to max
        if (!isInitialized)
        {
            currentHealth = maxHealth;
        }
        // Otherwise, preserve the health percentage
        else
        {
            float healthPercentage = currentHealth / maxHealth;
            currentHealth = maxHealth * healthPercentage;
        }
    }
    
    /// <summary>
    /// Get stats for a specific tier
    /// </summary>
    /// <param name="tier">Tier to get stats for</param>
    /// <returns>TierStats for the specified tier</returns>
    protected TierStats GetTierStats(int tier)
    {
        switch (tier)
        {
            case 1: return tier1Stats;
            case 2: return tier2Stats;
            default: return tier1Stats;
        }
    }
    
    /// <summary>
    /// Find target enemy and attack if in range
    /// </summary>
    protected virtual void FindAndAttackTarget()
    {
        if (!hasAttackCapability) return;
        
        if (targetEnemy == null || !IsEnemyInRange(targetEnemy))
        {
            // Find new target
            targetEnemy = FindClosestEnemy();
        }

        if (targetEnemy != null && CanAttack())
        {
            AttackTarget(targetEnemy);
        }
    }
    
    /// <summary>
    /// Find the closest enemy in range
    /// </summary>
    /// <returns>Transform of the closest enemy, or null if none found</returns>
    protected virtual Transform FindClosestEnemy()
    {
        if (!hasAttackCapability) return null;
        
        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;

        // Get all enemies
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in allEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= attackRange && distance < closestDistance)
            {
                closestEnemy = enemy.transform;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }
    
    /// <summary>
    /// Check if enemy is in attack range
    /// </summary>
    /// <param name="enemy">Enemy to check</param>
    /// <returns>True if enemy is in range</returns>
    protected virtual bool IsEnemyInRange(Transform enemy)
    {
        if (!hasAttackCapability) return false;
        return Vector3.Distance(transform.position, enemy.position) <= attackRange;
    }
    
    /// <summary>
    /// Attack the target enemy
    /// </summary>
    /// <param name="target">Target to attack</param>
    protected virtual void AttackTarget(Transform target)
    {
        if (!hasAttackCapability) return;
        
        if (!CanAttack() || target == null) return;
        
        // Direct damage attack (not projectile-based)
        var enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(attackDamage);
        }
        
        // Reset attack cooldown
        attackCooldown = 1f / attackSpeed;
    }
    
    #region Upgrade System
    
    /// <summary>
    /// Upgrade the building to the next tier
    /// </summary>
    /// <returns>True if upgrade was successful</returns>
    public virtual bool UpgradeBuilding()
    {
        // Check if already at max tier
        if (currentTier >= maxTier)
        {
            Debug.Log("Building is already at maximum tier", this);
            return false;
        }
        
        // Check if player can afford upgrade (would integrate with ResourceManager)
        if (!CanAffordUpgrade())
        {
            Debug.Log("Cannot afford building upgrade", this);
            return false;
        }
        
        // Spend resources
        SpendUpgradeResources();
        
        // Perform upgrade
        currentTier++;
        ApplyTierStats();
        
        Debug.Log($"Building upgraded to tier {currentTier}", this);
        return true;
    }
    
    /// <summary>
    /// Check if the player can afford to upgrade the building
    /// </summary>
    /// <returns>True if player has enough resources</returns>
    protected virtual bool CanAffordUpgrade()
    {
        // Integrate with ResourceManager when available
        // For now, allow all upgrades
        return true;
    }
    
    /// <summary>
    /// Spend resources required for upgrading
    /// </summary>
    protected virtual void SpendUpgradeResources()
    {
        // Integrate with ResourceManager when available
        // This is where you would deduct gold/other resources
    }
    
    /// <summary>
    /// Get the cost to upgrade to the next tier
    /// </summary>
    /// <returns>Gold cost for upgrade</returns>
    public virtual int GetUpgradeCost()
    {
        // Return different costs based on current tier
        switch (currentTier)
        {
            case 1: return 75;   // Cost to upgrade from tier 1 to 2
            default: return 0;   // Already at max tier
        }
    }
    
    /// <summary>
    /// Get current tier of the building
    /// </summary>
    /// <returns>Current tier</returns>
    public int GetCurrentTier()
    {
        return currentTier;
    }
    
    /// <summary>
    /// Get maximum tier of the building
    /// </summary>
    /// <returns>Maximum tier</returns>
    public int GetMaxTier()
    {
        return maxTier;
    }
    
    /// <summary>
    /// Check if building is at maximum tier
    /// </summary>
    /// <returns>True if at maximum tier</returns>
    public bool IsAtMaxTier()
    {
        return currentTier >= maxTier;
    }
    
    #endregion
    
    #region Blocking System
    
    /// <summary>
    /// Check if this building blocks enemy movement
    /// </summary>
    /// <returns>True if building blocks movement</returns>
    public bool DoesBlockMovement()
    {
        return blocksMovement;
    }
    
    #endregion
    
    #region Visualization
    
    /// <summary>
    /// Draw gizmos for visualization in the editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (isInitialized)
        {
            // Draw health bar
            Gizmos.color = Color.blue;
            
            // Draw attack range if this building can attack
            if (hasAttackCapability)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }
        }
    }
    
    #endregion
}