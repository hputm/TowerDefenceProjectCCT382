using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enhanced enemy implementation that combines movement, combat, and stat definition
/// </summary>
public class EnhancedEnemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Type")]
    [Tooltip("Type of this enemy which determines base stats")]
    public EnemyType enemyType = EnemyType.Bandit;
    
    [Header("Squad Settings")]
    [Tooltip("Is this enemy part of a squad")]
    public bool isSquadMember = false;
    
    [Tooltip("Tier level for squad members")]
    public int squadTier = 1;

    [Header("Base Stats Configuration")]
    [Tooltip("Stats for Refugee type")]
    public EnemyStats refugeeStats = new EnemyStats 
    { 
        maxHealth = 30f, 
        attackRange = 1.5f, 
        attackDamage = 5f, 
        moveSpeed = 1.5f,
        minGold = 1,
        maxGold = 3
    };
    
    [Tooltip("Stats for Bandit type")]
    public EnemyStats banditStats = new EnemyStats 
    { 
        maxHealth = 60f, 
        attackRange = 2f, 
        attackDamage = 12f, 
        moveSpeed = 2f,
        minGold = 3,
        maxGold = 7
    };
    
    [Tooltip("Stats for Axeman type")]
    public EnemyStats axemanStats = new EnemyStats 
    { 
        maxHealth = 100f, 
        attackRange = 2.5f, 
        attackDamage = 25f, 
        moveSpeed = 1.8f,
        minGold = 5,
        maxGold = 12
    };
    
    [Tooltip("Stats for Spearman type")]
    public EnemyStats spearmanStats = new EnemyStats 
    { 
        maxHealth = 80f, 
        attackRange = 3f, 
        attackDamage = 18f, 
        moveSpeed = 1.6f,
        minGold = 5,
        maxGold = 12
    };
    
    [Tooltip("Stats for Knight type")]
    public EnemyStats knightStats = new EnemyStats 
    { 
        maxHealth = 200f, 
        attackRange = 2.2f, 
        attackDamage = 35f, 
        moveSpeed = 1.2f,
        minGold = 15,
        maxGold = 30,
        armor = 10f
    };
    
    [Tooltip("Stats for Siege unit type")]
    public EnemyStats siegeUnitStats = new EnemyStats 
    { 
        maxHealth = 500f, 
        attackRange = 5f, 
        attackDamage = 100f, 
        moveSpeed = 0.8f,
        minGold = 50,
        maxGold = 100,
        armor = 50f
    };
    
    [Tooltip("Base stats for Squad member type")]
    public EnemyStats squadMemberStats = new EnemyStats 
    { 
        maxHealth = 70f, 
        attackRange = 2f, 
        attackDamage = 15f, 
        moveSpeed = 1.8f,
        minGold = 8,
        maxGold = 18
    };

    [Header("Squad Multipliers")]
    [Tooltip("Health multiplier for squad tier 1")]
    public float squadTier1HealthMultiplier = 1.0f;
    
    [Tooltip("Damage multiplier for squad tier 1")]
    public float squadTier1DamageMultiplier = 1.0f;
    
    [Tooltip("Gold multiplier for squad tier 1")]
    public float squadTier1GoldMultiplier = 1.0f;
    
    [Tooltip("Health multiplier for squad tier 2")]
    public float squadTier2HealthMultiplier = 1.5f;
    
    [Tooltip("Damage multiplier for squad tier 2")]
    public float squadTier2DamageMultiplier = 1.3f;
    
    [Tooltip("Gold multiplier for squad tier 2")]
    public float squadTier2GoldMultiplier = 1.2f;
    
    [Tooltip("Health multiplier for squad tier 3")]
    public float squadTier3HealthMultiplier = 2.0f;
    
    [Tooltip("Damage multiplier for squad tier 3")]
    public float squadTier3DamageMultiplier = 1.8f;
    
    [Tooltip("Gold multiplier for squad tier 3")]
    public float squadTier3GoldMultiplier = 1.5f;

    [Header("Current Stats")]
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackSpeed = 1f;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float armor;
    [HideInInspector] public int minGold;
    [HideInInspector] public int maxGold;

    // Pathfinding
    private List<Vector3> path = new List<Vector3>();
    private int currentPathIndex = 0;
    private bool hasReachedEndpoint = false;
    
    // Building targeting
    private BuildingBase targetBuilding;
    private float attackCooldown = 0f;
    
    // Group reference
    private GroupMember groupMember;
    
    // Events
    public System.Action<float> onHealthChanged;
    public System.Action onEnemyDeath;
    
    void Start()
    {
        InitializeStats();
        FindPath();
        groupMember = GetComponent<GroupMember>();
    }

    void Update()
    {
        if (path == null || path.Count == 0)
            return;
            
        // Update attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        
        // Handle targeting and attacking buildings
        HandleBuildingTargeting();
        
        // Move along path if not attacking a building
        if (targetBuilding == null || !targetBuilding.IsBlockingMovement())
        {
            MoveAlongPath();
        }
    }
    
    /// <summary>
    /// Initialize enemy stats based on type and squad settings
    /// </summary>
    private void InitializeStats()
    {
        // Get base stats for enemy type
        EnemyStats baseStats = GetBaseStatsForType(enemyType);
        
        // Apply base stats
        maxHealth = baseStats.maxHealth;
        attackRange = baseStats.attackRange;
        attackDamage = baseStats.attackDamage;
        attackSpeed = baseStats.attackSpeed;
        moveSpeed = baseStats.moveSpeed;
        armor = baseStats.armor;
        minGold = baseStats.minGold;
        maxGold = baseStats.maxGold;

        // Apply squad tier multipliers if this is a squad member
        if (isSquadMember)
        {
            float healthMultiplier = 1.0f;
            float damageMultiplier = 1.0f;
            float goldMultiplier = 1.0f;

            switch (squadTier)
            {
                case 1:
                    healthMultiplier = squadTier1HealthMultiplier;
                    damageMultiplier = squadTier1DamageMultiplier;
                    goldMultiplier = squadTier1GoldMultiplier;
                    break;
                case 2:
                    healthMultiplier = squadTier2HealthMultiplier;
                    damageMultiplier = squadTier2DamageMultiplier;
                    goldMultiplier = squadTier2GoldMultiplier;
                    break;
                case 3:
                    healthMultiplier = squadTier3HealthMultiplier;
                    damageMultiplier = squadTier3DamageMultiplier;
                    goldMultiplier = squadTier3GoldMultiplier;
                    break;
            }

            maxHealth *= healthMultiplier;
            attackDamage *= damageMultiplier;
            minGold = Mathf.RoundToInt(minGold * goldMultiplier);
            maxGold = Mathf.RoundToInt(maxGold * goldMultiplier);
        }

        // Initialize current health
        currentHealth = maxHealth;
    }
    
    /// <summary>
    /// Get base stats for specified enemy type
    /// </summary>
    private EnemyStats GetBaseStatsForType(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Refugee:
                return refugeeStats;
            case EnemyType.Bandit:
                return banditStats;
            case EnemyType.Axeman:
                return axemanStats;
            case EnemyType.Spearman:
                return spearmanStats;
            case EnemyType.Knight:
                return knightStats;
            case EnemyType.SiegeUnit:
                return siegeUnitStats;
            case EnemyType.SquadMember:
                return squadMemberStats;
            default:
                return banditStats; // Default fallback
        }
    }
    
    #region Pathfinding
    
    /// <summary>
    /// Find a path from current position to the endpoint
    /// </summary>
    private void FindPath()
    {
        // For now, we'll use a simple approach to find a path
        // In a full implementation, this would connect to your level's start and end points
        
        // Try to find the grid manager
        if (GridManager.Instance == null)
        {
            Debug.LogWarning("GridManager not found! Falling back to simple movement.", this);
            CreateSimplePath();
            return;
        }
        
        // Try to find road cells
        var roadCells = GridPathfinder.Instance.GetRoadCells();
        if (roadCells.Count < 2)
        {
            Debug.LogWarning("Not enough road cells found! Falling back to simple movement.", this);
            CreateSimplePath();
            return;
        }
        
        // Create a simple path along some road cells
        path.Clear();
        for (int i = 0; i < Mathf.Min(5, roadCells.Count); i++)
        {
            Vector3 worldPos = GridManager.Instance.GridToWorld(roadCells[i]);
            // Offset Y to be above ground
            worldPos.y = 0.5f;
            path.Add(worldPos);
        }
        
        // Add endpoint (castle or similar)
        Vector3 endPoint = path[path.Count - 1];
        endPoint.z += 3f; // Extend path a bit
        endPoint.y = 0.5f;
        path.Add(endPoint);
    }
    
    /// <summary>
    /// Create a simple path for fallback behavior
    /// </summary>
    private void CreateSimplePath()
    {
        path.Clear();
        
        // Simple path for testing
        Vector3 startPos = transform.position;
        startPos.y = 0.5f;
        path.Add(startPos);
        
        Vector3 midPoint = startPos;
        midPoint.z += 5f;
        midPoint.y = 0.5f;
        path.Add(midPoint);
        
        Vector3 endPoint = midPoint;
        endPoint.z += 5f;
        endPoint.y = 0.5f;
        path.Add(endPoint);
    }
    
    /// <summary>
    /// Move the enemy along the path
    /// </summary>
    private void MoveAlongPath()
    {
        if (hasReachedEndpoint || path.Count == 0 || currentPathIndex >= path.Count)
            return;
            
        // Get current target position
        Vector3 targetPos = path[currentPathIndex];
        
        // If part of a group, consider group formation
        if (groupMember != null && groupMember.group != null)
        {
            // Group members follow their formation positions
            // The leader follows the path directly
            if (groupMember.group.GetLeader() != this)
            {
                // Non-leaders follow formation - their movement is handled by EnemyGroup
                return;
            }
        }
        
        // Move towards target
        Vector3 direction = (targetPos - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Rotate to face movement direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Check if we've reached the current waypoint
        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            currentPathIndex++;
            
            // Check if we've reached the end of the path
            if (currentPathIndex >= path.Count)
            {
                hasReachedEndpoint = true;
                OnReachedEndpoint();
            }
        }
    }
    
    #endregion
    
    #region Building Targeting
    
    /// <summary>
    /// Handle targeting and attacking buildings
    /// </summary>
    private void HandleBuildingTargeting()
    {
        // If we're already attacking a building, check if it still exists and is blocking
        if (targetBuilding != null)
        {
            // Check if building still exists and is blocking
            if (targetBuilding.IsDestroyed() || !targetBuilding.IsBlockingMovement())
            {
                targetBuilding = null;
            }
            else
            {
                // Attack the building
                AttackBuilding(targetBuilding);
                return;
            }
        }
        
        // Look for nearby blocking buildings
        targetBuilding = FindBlockingBuilding();
        
        if (targetBuilding != null)
        {
            // Attack the building
            AttackBuilding(targetBuilding);
        }
    }
    
    /// <summary>
    /// Find a building that's blocking the path
    /// </summary>
    /// <returns>Blocking building, or null if none found</returns>
    private BuildingBase FindBlockingBuilding()
    {
        // Check for buildings in attack range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var collider in hitColliders)
        {
            BuildingBase building = collider.GetComponent<BuildingBase>();
            if (building != null && building.IsBlockingMovement() && !building.IsDestroyed())
            {
                // Check if building is on our path
                if (IsBuildingOnPath(building))
                {
                    return building;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Check if a building is on our path
    /// </summary>
    /// <param name="building">Building to check</param>
    /// <returns>True if building is on path</returns>
    private bool IsBuildingOnPath(BuildingBase building)
    {
        // Simple approximation - check if building is near our current path segment
        if (path.Count == 0 || currentPathIndex >= path.Count)
            return false;
            
        Vector3 currentTarget = path[Mathf.Min(currentPathIndex, path.Count - 1)];
        Vector3 toBuilding = building.transform.position - transform.position;
        Vector3 toTarget = currentTarget - transform.position;
        
        // Check if building is in front of us and close to our path
        float dot = Vector3.Dot(toBuilding.normalized, toTarget.normalized);
        float distanceToBuilding = toBuilding.magnitude;
        
        return dot > 0.5f && distanceToBuilding < attackRange + 2f;
    }
    
    /// <summary>
    /// Attack a building
    /// </summary>
    /// <param name="building">Building to attack</param>
    private void AttackBuilding(BuildingBase building)
    {
        if (building == null || attackCooldown > 0)
            return;
            
        // Apply damage to building
        building.TakeDamage(attackDamage);
        
        // Reset attack cooldown
        attackCooldown = 1f / attackSpeed;
    }
    
    #endregion
    
    #region IDamageable Implementation
    
    /// <summary>
    /// Apply damage to the enemy
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    public void TakeDamage(float damage)
    {
        // Apply armor reduction
        float reducedDamage = damage - armor;
        float finalDamage = Mathf.Max(0, reducedDamage); // Ensure damage is not negative
        
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0, currentHealth); // Clamp to minimum 0
        
        onHealthChanged?.Invoke(currentHealth);
        
        // Check if enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Heal the enemy
    /// </summary>
    /// <param name="amount">Amount of health to restore</param>
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Clamp to max health
        
        onHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// Get the current health of the enemy
    /// </summary>
    /// <returns>Current health</returns>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    /// <summary>
    /// Get the maximum health of the enemy
    /// </summary>
    /// <returns>Maximum health</returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    /// <summary>
    /// Check if the enemy is dead
    /// </summary>
    /// <returns>True if health is zero or below</returns>
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    
    #endregion
    
    /// <summary>
    /// Handle enemy death
    /// </summary>
    private void Die()
    {
        onEnemyDeath?.Invoke();
        
        // Remove from group if part of one
        if (groupMember != null && groupMember.group != null)
        {
            groupMember.group.RemoveMember(this);
        }
        
        // Drop gold
        DropGold();
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Drop gold based on enemy type and current stats
    /// </summary>
    private void DropGold()
    {
        var resourceManager = ResourceManager.Instance;
        if (resourceManager != null)
        {
            // Generate random gold amount within range
            int goldAmount = Random.Range(minGold, maxGold + 1);
            
            // Apply squad multiplier if this is a squad member
            if (isSquadMember)
            {
                float goldMultiplier = GetSquadGoldMultiplier();
                goldAmount = Mathf.RoundToInt(goldAmount * goldMultiplier);
            }
            
            // Add gold to resource manager
            resourceManager.AddGold(goldAmount);
        }
    }
    
    /// <summary>
    /// Get squad gold multiplier based on tier
    /// </summary>
    private float GetSquadGoldMultiplier()
    {
        switch (squadTier)
        {
            case 1: return squadTier1GoldMultiplier;
            case 2: return squadTier2GoldMultiplier;
            case 3: return squadTier3GoldMultiplier;
            default: return 1.0f;
        }
    }
    
    /// <summary>
    /// Called when the enemy reaches the endpoint
    /// </summary>
    private void OnReachedEndpoint()
    {
        // Notify game manager that enemy reached the keep
        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.OnEnemyReachedKeep();
        }
        
        // Remove from group if part of one
        if (groupMember != null && groupMember.group != null)
        {
            groupMember.group.RemoveMember(this);
        }
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Draw gizmos for debugging
    /// </summary>
    void OnDrawGizmos()
    {
        // Draw path
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
            
            // Draw waypoints
            Gizmos.color = Color.magenta;
            foreach (var point in path)
            {
                Gizmos.DrawSphere(point, 0.2f);
            }
        }
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}