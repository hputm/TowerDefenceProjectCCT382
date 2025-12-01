using UnityEngine;

/*
 * Tower Attack & Health System
 * 
 * A system that handles attack capabilities and health for defensive structures
 * 
 * Features:
 * - Configurable attack range and damage
 * - Health management with visual feedback
 * - Support for different tower types (Arrow Tower, Watchpost)
 * - Tier-based stats (different values for different upgrade levels)
 * - Enemy targeting and attack logic
 * 
 * Usage:
 * Attach to defensive structures (Arrow Towers, Watchposts)
 * Configure stats for each tier in inspector
 * 
 * Tower Types:
 * Arrow Tower: Ranged attack with increasing range and damage
 * Watchpost: Short-range attack with area effect
 */

//==================================================================================================

/*
 * 塔攻击与血量系统
 * 
 * 处理防御建筑攻击能力和血量的系统
 * 
 * 功能特点:
 * - 可配置的攻击范围和伤害
 * - 血量管理带视觉反馈
 * - 支持不同塔类型（箭塔、哨卡）
 * - 等级基础属性（不同升级等级的不同数值）
 * - 敌人目标选择和攻击逻辑
 * 
 * 用途:
 * 添加到防御建筑上（箭塔、哨卡）
 * 在检查器中为每级配置属性
 * 
 * 塔类型:
 * 箭塔: 范围攻击，范围和伤害递增
 * 哨卡: 近战攻击带区域效果
 */

public enum TowerType
{
    ArrowTower,   // 箭塔
    Watchpost     // 哨卡
}

[System.Serializable]
public class TowerStats
{
    [Header("Attack Stats / 攻击属性")]
    [Tooltip("Attack range / 攻击范围")]
    public float attackRange = 5f;
    
    [Tooltip("Attack damage / 攻击伤害")]
    public float attackDamage = 10f;
    
    [Tooltip("Attack speed (attacks per second) / 攻击速度（每秒攻击次数）")]
    public float attackSpeed = 1f;
    
    [Tooltip("Projectile speed (for ranged attacks) / 投射物速度（用于范围攻击）")]
    public float projectileSpeed = 10f;
    
    [Header("Health Stats / 血量属性")]
    [Tooltip("Maximum health / 最大血量")]
    public float maxHealth = 100f;
    
    [Header("Visual Effects / 视觉效果")]
    [Tooltip("Attack effect prefab / 攻击效果预制体")]
    public GameObject attackEffect;
    
    [Tooltip("Range visualization color / 范围可视化颜色")]
    public Color rangeColor = Color.red;
}

public class TowerAttackHealth : MonoBehaviour
{
    [Header("Tower Configuration / 塔配置")]
    [Tooltip("Tower type / 塔类型")]
    public TowerType towerType = TowerType.ArrowTower;
    
    [Tooltip("Current tier level / 当前等级")]
    public int currentTier = 1;

    [Header("Tower Stats by Tier / 各等级塔属性")]
    [Tooltip("Tier 1 stats / 1级属性")]
    public TowerStats tier1Stats = new TowerStats 
    { 
        attackRange = 5f, 
        attackDamage = 10f, 
        attackSpeed = 1f, 
        maxHealth = 100f,
        rangeColor = Color.red
    };
    
    [Tooltip("Tier 2 stats / 2级属性")]
    public TowerStats tier2Stats = new TowerStats 
    { 
        attackRange = 7f, 
        attackDamage = 20f, 
        attackSpeed = 1.2f, 
        maxHealth = 150f,
        rangeColor = Color.yellow
    };
    
    [Tooltip("Tier 3 stats / 3级属性")]
    public TowerStats tier3Stats = new TowerStats 
    { 
        attackRange = 9f, 
        attackDamage = 35f, 
        attackSpeed = 1.5f, 
        maxHealth = 200f,
        rangeColor = Color.green
    };

    [Header("Game References / 游戏引用")]
    [Tooltip("Reference to enemy manager / 敌人管理器引用")]
    public EnemyManager enemyManager;
    
    [Tooltip("Projectile prefab for ranged attacks / 范围攻击的投射物预制体")]
    public GameObject projectilePrefab;

    [Header("Current Stats / 当前属性")]
    [HideInInspector] public float currentAttackRange;
    [HideInInspector] public float currentAttackDamage;
    [HideInInspector] public float currentAttackSpeed;
    [HideInInspector] public float currentMaxHealth;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentProjectileSpeed;

    // Private variables
    private float attackCooldown = 0f;
    private Transform targetEnemy;
    private bool isInitialized = false;

    void Start()
    {
        InitializeTower();
    }

    void InitializeTower()
    {
        // Get current tier stats
        TowerStats currentStats = GetCurrentTierStats();
        if (currentStats == null)
        {
            Debug.LogError("Invalid tier configuration", this);
            return;
        }

        // Apply current stats
        currentAttackRange = currentStats.attackRange;
        currentAttackDamage = currentStats.attackDamage;
        currentAttackSpeed = currentStats.attackSpeed;
        currentMaxHealth = currentStats.maxHealth;
        currentHealth = currentMaxHealth;
        currentProjectileSpeed = currentStats.projectileSpeed;

        // Find enemy manager if not assigned
        if (enemyManager == null)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager == null)
            {
                Debug.LogWarning("EnemyManager not found, searching for alternative", this);
            }
        }

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        // Update attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Find and attack target
        FindAndAttackTarget();
    }

    /// <summary>
    /// Find target enemy and attack if in range
    /// 寻找目标敌人并在范围内攻击
    /// </summary>
    void FindAndAttackTarget()
    {
        if (targetEnemy == null || !IsEnemyInRange(targetEnemy))
        {
            // Find new target
            targetEnemy = FindClosestEnemy();
        }

        if (targetEnemy != null && attackCooldown <= 0f)
        {
            AttackTarget(targetEnemy);
            attackCooldown = 1f / currentAttackSpeed;
        }
    }

    /// <summary>
    /// Find the closest enemy in range
    /// 寻找范围内的最近敌人
    /// </summary>
    Transform FindClosestEnemy()
    {
        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;

        // Get all enemies (you might want to use a more efficient method)
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in allEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= currentAttackRange && distance < closestDistance)
            {
                closestEnemy = enemy.transform;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }

    /// <summary>
    /// Check if enemy is in attack range
    /// 检查敌人是否在攻击范围内
    /// </summary>
    bool IsEnemyInRange(Transform enemy)
    {
        return Vector3.Distance(transform.position, enemy.position) <= currentAttackRange;
    }

    /// <summary>
    /// Attack the target enemy
    /// 攻击目标敌人
    /// </summary>
    void AttackTarget(Transform target)
    {
        if (towerType == TowerType.ArrowTower)
        {
            // Ranged attack - spawn projectile
            SpawnProjectile(target);
        }
        else if (towerType == TowerType.Watchpost)
        {
            // Melee attack - direct damage
            ApplyDamageToEnemy(target, currentAttackDamage);
        }
    }

    /// <summary>
    /// Spawn projectile for ranged attack
    /// 生成投射物用于范围攻击
    /// </summary>
    void SpawnProjectile(Transform target)
    {
        if (projectilePrefab != null && target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            
            // Set projectile properties (you'll need to implement this)
            var projectileScript = projectile.GetComponent<TowerProjectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(target, currentAttackDamage, currentProjectileSpeed);
            }
        }
    }

    /// <summary>
    /// Apply damage to enemy
    /// 对敌人应用伤害
    /// </summary>
    void ApplyDamageToEnemy(Transform enemy, float damage)
    {
        var enemyHealth = enemy.GetComponent<EnemyHealth>(); // Replace with your enemy health component
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }

    /// <summary>
    /// Take damage from enemy attack
    /// 承受敌人攻击的伤害
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // Clamp health between 0 and max
        currentHealth = Mathf.Clamp(currentHealth, 0, currentMaxHealth);
        
        // Check if tower is destroyed
        if (currentHealth <= 0)
        {
            DestroyTower();
        }
    }

    /// <summary>
    /// Destroy the tower
    /// 销毁塔
    /// </summary>
    void DestroyTower()
    {
        Debug.Log($"Tower {gameObject.name} destroyed!", this);
        
        // You can add destruction effects here
        // Play destruction sound, spawn particles, etc.
        
        // Notify game manager or other systems
        var gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnTowerDestroyed(this);
        }
        
        // Destroy the game object
        Destroy(gameObject);
    }

    /// <summary>
    /// Get stats for current tier
    /// 获取当前等级的属性
    /// </summary>
    TowerStats GetCurrentTierStats()
    {
        switch (currentTier)
        {
            case 1: return tier1Stats;
            case 2: return tier2Stats;
            case 3: return tier3Stats;
            default: return tier1Stats;
        }
    }

    /// <summary>
    /// Upgrade tower to next tier
    /// 升级塔到下一级
    /// </summary>
    public bool UpgradeTower()
    {
        if (currentTier >= 3) return false; // Max tier reached
        
        currentTier++;
        InitializeTower(); // Reinitialize with new stats
        return true;
    }

    /// <summary>
    /// Get current health percentage
    /// 获取当前血量百分比
    /// </summary>
    public float GetHealthPercentage()
    {
        return currentHealth / currentMaxHealth;
    }

    // Gizmos for visualizing attack range
    void OnDrawGizmosSelected()
    {
        if (isInitialized)
        {
            TowerStats stats = GetCurrentTierStats();
            if (stats != null)
            {
                Gizmos.color = stats.rangeColor;
                Gizmos.DrawWireSphere(transform.position, currentAttackRange);
            }
        }
    }

    // Public getters for other systems
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => currentMaxHealth;
    public float GetAttackRange() => currentAttackRange;
    public float GetAttackDamage() => currentAttackDamage;
    public int GetCurrentTier() => currentTier;
    public TowerType GetTowerType() => towerType;
    public bool IsDestroyed() => currentHealth <= 0;
}