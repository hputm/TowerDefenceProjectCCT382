using UnityEngine;

/*
 * Enemy Stats System
 * 
 * A system that defines health, attack range, and damage for different enemy types
 * 
 * Features:
 * - Configurable stats for different enemy types
 * - Tier-based variations for squad members
 * - Integration with enemy AI and health systems
 * - Flexible stat configuration per enemy type
 * 
 * Usage:
 * Attach to enemy game objects in prefabs
 * Configure stats in inspector for each enemy type
 * 
 * Enemy Types Supported:
 * - Refugees: Low stats
 * - Bandits: Medium stats
 * - Axeman: High damage melee
 * - Spearman: Medium range with good damage
 * - Knights: High health and damage
 * - Siege units: Very high stats
 */

//==================================================================================================

/*
 * 敌人属性系统
 * 
 * 定义不同敌人类型的血量、攻击范围和伤害的系统
 * 
 * 功能特点:
 * - 不同敌人类型的可配置属性
 * - 战团成员等级变化
 * - 与敌人AI和血量系统的集成
 * - 每种敌人类型的灵活属性配置
 * 
 * 用途:
 * 添加到敌人预制体对象上
 * 在检查器中为每种敌人类型配置属性
 * 
 * 支持的敌人类型:
 * - 难民: 低属性
 * - 强盗: 中等属性
 * - 斧手: 高伤害近战
 * - 矛手: 中等范围良好伤害
 * - 骑士: 高血量和伤害
 * - 攻城单位: 极高属性
 */

public enum EnemyType
{
    Refugee,        // 难民
    Bandit,         // 强盗
    Axeman,         // 斧手
    Spearman,       // 矛手
    Knight,         // 骑士
    SiegeUnit,      // 攻城单位
    SquadMember     // 战团成员
}

[System.Serializable]
public class EnemyStats
{
    [Header("Health Stats / 血量属性")]
    [Tooltip("Maximum health / 最大血量")]
    public float maxHealth = 100f;
    
    [Header("Attack Stats / 攻击属性")]
    [Tooltip("Attack range / 攻击范围")]
    public float attackRange = 2f;
    
    [Tooltip("Attack damage / 攻击伤害")]
    public float attackDamage = 10f;
    
    [Tooltip("Attack speed (attacks per second) / 攻击速度（每秒攻击次数）")]
    public float attackSpeed = 1f;
    
    [Header("Movement Stats / 移动属性")]
    [Tooltip("Movement speed / 移动速度")]
    public float moveSpeed = 2f;
    
    [Header("Special Properties / 特殊属性")]
    [Tooltip("Armor value (reduces incoming damage) / 护甲值（减少受到伤害）")]
    public float armor = 0f;
    
    [Tooltip("Special abilities or resistances / 特殊能力或抗性")]
    public string specialAbilities = "";
}

public class EnemyStatsSystem : MonoBehaviour
{
    [Header("Enemy Configuration / 敌人配置")]
    [Tooltip("Enemy type for stat configuration / 用于属性配置的敌人类型")]
    public EnemyType enemyType = EnemyType.Bandit;
    
    [Tooltip("Is this a squad member unit / 是否为战团单位成员")]
    public bool isSquadMember = false;
    
    [Tooltip("Squad member tier (for squad units) / 战团成员等级（用于战团单位）")]
    public int squadTier = 1;

    [Header("Base Enemy Stats / 基础敌人属性")]
    [Tooltip("Stats for Refugee type / 难民类型属性")]
    public EnemyStats refugeeStats = new EnemyStats 
    { 
        maxHealth = 30f, 
        attackRange = 1.5f, 
        attackDamage = 5f, 
        moveSpeed = 1.5f 
    };
    
    [Tooltip("Stats for Bandit type / 强盗类型属性")]
    public EnemyStats banditStats = new EnemyStats 
    { 
        maxHealth = 60f, 
        attackRange = 2f, 
        attackDamage = 12f, 
        moveSpeed = 2f 
    };
    
    [Tooltip("Stats for Axeman type / 斧手类型属性")]
    public EnemyStats axemanStats = new EnemyStats 
    { 
        maxHealth = 100f, 
        attackRange = 2.5f, 
        attackDamage = 25f, 
        moveSpeed = 1.8f 
    };
    
    [Tooltip("Stats for Spearman type / 矛手类型属性")]
    public EnemyStats spearmanStats = new EnemyStats 
    { 
        maxHealth = 80f, 
        attackRange = 3f, 
        attackDamage = 18f, 
        moveSpeed = 1.6f 
    };
    
    [Tooltip("Stats for Knight type / 骑士类型属性")]
    public EnemyStats knightStats = new EnemyStats 
    { 
        maxHealth = 200f, 
        attackRange = 2.2f, 
        attackDamage = 35f, 
        moveSpeed = 1.2f,
        armor = 10f
    };
    
    [Tooltip("Stats for Siege unit type / 攻城单位类型属性")]
    public EnemyStats siegeUnitStats = new EnemyStats 
    { 
        maxHealth = 500f, 
        attackRange = 5f, 
        attackDamage = 100f, 
        moveSpeed = 0.8f,
        armor = 50f
    };
    
    [Tooltip("Base stats for Squad member type / 战团成员类型基础属性")]
    public EnemyStats squadMemberStats = new EnemyStats 
    { 
        maxHealth = 70f, 
        attackRange = 2f, 
        attackDamage = 15f, 
        moveSpeed = 1.8f 
    };

    [Header("Squad Tier Multipliers / 战团等级倍数")]
    [Tooltip("Health multiplier for squad tier 1 / 战团1级血量倍数")]
    public float squadTier1HealthMultiplier = 1.0f;
    
    [Tooltip("Damage multiplier for squad tier 1 / 战团1级伤害倍数")]
    public float squadTier1DamageMultiplier = 1.0f;
    
    [Tooltip("Health multiplier for squad tier 2 / 战团2级血量倍数")]
    public float squadTier2HealthMultiplier = 1.5f;
    
    [Tooltip("Damage multiplier for squad tier 2 / 战团2级伤害倍数")]
    public float squadTier2DamageMultiplier = 1.3f;
    
    [Tooltip("Health multiplier for squad tier 3 / 战团3级血量倍数")]
    public float squadTier3HealthMultiplier = 2.0f;
    
    [Tooltip("Damage multiplier for squad tier 3 / 战团3级伤害倍数")]
    public float squadTier3DamageMultiplier = 1.8f;

    [Header("Current Stats / 当前属性")]
    [HideInInspector] public float currentMaxHealth;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentAttackRange;
    [HideInInspector] public float currentAttackDamage;
    [HideInInspector] public float currentAttackSpeed;
    [HideInInspector] public float currentMoveSpeed;
    [HideInInspector] public float currentArmor;

    [Header("Game References / 游戏引用")]
    [Tooltip("Reference to enemy health system / 敌人血量系统引用")]
    public EnemyHealth enemyHealth;
    
    [Tooltip("Reference to enemy movement system / 敌人移动系统引用")]
    public Enemy enemyMovement;

    void Start()
    {
        InitializeStats();
        ApplyStatsToComponents();
    }

    void InitializeStats()
    {
        // Get base stats for enemy type
        EnemyStats baseStats = GetBaseStatsForType(enemyType);
        
        // Apply base stats
        currentMaxHealth = baseStats.maxHealth;
        currentAttackRange = baseStats.attackRange;
        currentAttackDamage = baseStats.attackDamage;
        currentAttackSpeed = baseStats.attackSpeed;
        currentMoveSpeed = baseStats.moveSpeed;
        currentArmor = baseStats.armor;

        // Apply squad tier multipliers if this is a squad member
        if (isSquadMember)
        {
            float healthMultiplier = 1.0f;
            float damageMultiplier = 1.0f;

            switch (squadTier)
            {
                case 1:
                    healthMultiplier = squadTier1HealthMultiplier;
                    damageMultiplier = squadTier1DamageMultiplier;
                    break;
                case 2:
                    healthMultiplier = squadTier2HealthMultiplier;
                    damageMultiplier = squadTier2DamageMultiplier;
                    break;
                case 3:
                    healthMultiplier = squadTier3HealthMultiplier;
                    damageMultiplier = squadTier3DamageMultiplier;
                    break;
            }

            currentMaxHealth *= healthMultiplier;
            currentAttackDamage *= damageMultiplier;
        }

        // Initialize current health
        currentHealth = currentMaxHealth;
    }

    /// <summary>
    /// Get base stats for specified enemy type
    /// 获取指定敌人类型的基属性
    /// </summary>
    EnemyStats GetBaseStatsForType(EnemyType type)
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

    /// <summary>
    /// Apply stats to other component systems
    /// 将属性应用到其他组件系统
    /// </summary>
    void ApplyStatsToComponents()
    {
        // Apply to health system if available
        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }
        if (enemyHealth != null)
        {
            enemyHealth.SetMaxHealth(currentMaxHealth);
            enemyHealth.SetCurrentHealth(currentHealth);
            enemyHealth.SetArmor(currentArmor);
        }

        // Apply to movement system if available
        if (enemyMovement == null)
        {
            enemyMovement = GetComponent<Enemy>();
        }
        if (enemyMovement != null)
        {
            enemyMovement.speed = currentMoveSpeed;
        }
    }

    /// <summary>
    /// Get damage after applying armor reduction
    /// 应用护甲减免后获取伤害
    /// </summary>
    public float CalculateDamage(float incomingDamage)
    {
        float reducedDamage = incomingDamage - currentArmor;
        return Mathf.Max(0, reducedDamage); // Ensure damage is not negative
    }

    /// <summary>
    /// Apply damage to enemy
    /// 对敌人应用伤害
    /// </summary>
    public void TakeDamage(float damage)
    {
        float finalDamage = CalculateDamage(damage);
        currentHealth -= finalDamage;
        
        // Clamp health between 0 and max
        currentHealth = Mathf.Clamp(currentHealth, 0, currentMaxHealth);
        
        // Update health component if available
        if (enemyHealth != null)
        {
            enemyHealth.SetCurrentHealth(currentHealth);
        }

        // Check if enemy is defeated
        if (currentHealth <= 0)
        {
            OnEnemyDefeated();
        }
    }

    /// <summary>
    /// Called when enemy is defeated
    /// 敌人被击败时调用
    /// </summary>
    void OnEnemyDefeated()
    {
        Debug.Log($"Enemy {enemyType} defeated!", this);
        
        // Trigger gold drop system if available
        var goldDrop = GetComponent<EnemyGoldDrop>();
        if (goldDrop != null)
        {
            goldDrop.OnEnemyDefeated();
        }
        
        // You can add other defeat effects here
        // Animation, particle effects, etc.
        
        // Destroy the game object
        Destroy(gameObject);
    }

    /// <summary>
    /// Heal the enemy
    /// 治疗敌人
    /// </summary>
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, currentMaxHealth);
        
        if (enemyHealth != null)
        {
            enemyHealth.SetCurrentHealth(currentHealth);
        }
    }

    /// <summary>
    /// Get current health percentage
    /// 获取当前血量百分比
    /// </summary>
    public float GetHealthPercentage()
    {
        return currentHealth / currentMaxHealth;
    }

    /// <summary>
    /// Get stats summary for debugging
    /// 获取属性摘要用于调试
    /// </summary>
    public string GetStatsSummary()
    {
        return $"Type: {enemyType}, Health: {currentHealth}/{currentMaxHealth}, " +
               $"Damage: {currentAttackDamage}, Range: {currentAttackRange}, " +
               $"Speed: {currentMoveSpeed}, Armor: {currentArmor}";
    }

    // Gizmos for visualizing attack range
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentAttackRange);
        }
    }

    // Public getters for other systems
    public EnemyType GetEnemyType() => enemyType;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => currentMaxHealth;
    public float GetAttackRange() => currentAttackRange;
    public float GetAttackDamage() => currentAttackDamage;
    public float GetMoveSpeed() => currentMoveSpeed;
    public float GetArmor() => currentArmor;
    public bool IsSquadMember() => isSquadMember;
    public int GetSquadTier() => squadTier;
    public bool IsAlive() => currentHealth > 0;
}