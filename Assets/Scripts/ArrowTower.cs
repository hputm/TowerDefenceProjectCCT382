using UnityEngine;

/// <summary>
/// Arrow Tower building implementation.
/// Inherits from BuildingBase and adds tower-specific functionality.
/// Can only be placed at road ends.
/// </summary>
public class ArrowTower : BuildingBase
{
    [Header("Arrow Tower Specific")]
    [Tooltip("Projectile prefab for ranged attacks")]
    public GameObject projectilePrefab;
    
    [Tooltip("Current upgrade tier of the tower")]
    public int currentTier = 1;
    
    [Tooltip("Maximum tier this tower can reach")]
    public int maxTier = 3;
    
    // Tier-based stats
    [System.Serializable]
    public class TierStats
    {
        public float health = 100f;
        public float attackRange = 5f;
        public float attackSpeed = 1f;
        public float attackDamage = 10f;
    }
    
    [Header("Tier Statistics")]
    public TierStats tier1Stats = new TierStats { health = 100f, attackRange = 5f, attackSpeed = 1f, attackDamage = 10f };
    public TierStats tier2Stats = new TierStats { health = 150f, attackRange = 7f, attackSpeed = 1.2f, attackDamage = 20f };
    public TierStats tier3Stats = new TierStats { health = 200f, attackRange = 9f, attackSpeed = 1.5f, attackDamage = 35f };
    
    protected override void Start()
    {
        // Set placement constraint - towers can only be placed at road ends
        placementType = PlacementType.RoadEndsOnly;
        buildingType = "Arrow Tower";
        canAttack = true;
        
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
        
        // Find and attack targets
        FindAndAttackTarget();
    }
    
    /// <summary>
    /// Apply stats based on current tier
    /// </summary>
    protected virtual void ApplyTierStats()
    {
        TierStats stats = GetTierStats(currentTier);
        
        maxHealth = stats.health;
        attackRange = stats.attackRange;
        attackSpeed = stats.attackSpeed;
        attackDamage = stats.attackDamage;
        
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
            case 3: return tier3Stats;
            default: return tier1Stats;
        }
    }
    
    /// <summary>
    /// Find target enemy and attack if in range
    /// </summary>
    protected virtual void FindAndAttackTarget()
    {
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
        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;

        // Get all enemies (you might want to use a more efficient method)
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
        return Vector3.Distance(transform.position, enemy.position) <= attackRange;
    }
    
    /// <summary>
    /// Attack the target enemy
    /// </summary>
    /// <param name="target">Target to attack</param>
    protected virtual void AttackTarget(Transform target)
    {
        if (!CanAttack() || target == null) return;
        
        // Ranged attack - spawn projectile
        SpawnProjectile(target);
        
        // Reset attack cooldown
        attackCooldown = 1f / attackSpeed;
    }
    
    /// <summary>
    /// Spawn projectile for ranged attack
    /// </summary>
    /// <param name="target">Target to shoot at</param>
    protected virtual void SpawnProjectile(Transform target)
    {
        if (projectilePrefab != null && target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            
            // Set projectile properties (you'll need to implement this in your Projectile class)
            var projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(target, attackDamage);
            }
        }
    }
    
    #region Upgrade System
    
    /// <summary>
    /// Upgrade the tower to the next tier
    /// </summary>
    /// <returns>True if upgrade was successful</returns>
    public virtual bool UpgradeTower()
    {
        // Check if already at max tier
        if (currentTier >= maxTier)
        {
            Debug.Log("Tower is already at maximum tier", this);
            return false;
        }
        
        // Check if player can afford upgrade (would integrate with ResourceManager)
        if (!CanAffordUpgrade())
        {
            Debug.Log("Cannot afford tower upgrade", this);
            return false;
        }
        
        // Spend resources
        SpendUpgradeResources();
        
        // Perform upgrade
        currentTier++;
        ApplyTierStats();
        
        Debug.Log($"Tower upgraded to tier {currentTier}", this);
        return true;
    }
    
    /// <summary>
    /// Check if the player can afford to upgrade the tower
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
            case 1: return 100;  // Cost to upgrade from tier 1 to 2
            case 2: return 250;  // Cost to upgrade from tier 2 to 3
            default: return 0;   // Already at max tier
        }
    }
    
    /// <summary>
    /// Get current tier of the tower
    /// </summary>
    /// <returns>Current tier</returns>
    public int GetCurrentTier()
    {
        return currentTier;
    }
    
    /// <summary>
    /// Get maximum tier of the tower
    /// </summary>
    /// <returns>Maximum tier</returns>
    public int GetMaxTier()
    {
        return maxTier;
    }
    
    /// <summary>
    /// Check if tower is at maximum tier
    /// </summary>
    /// <returns>True if at maximum tier</returns>
    public bool IsAtMaxTier()
    {
        return currentTier >= maxTier;
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
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
    
    #endregion
}