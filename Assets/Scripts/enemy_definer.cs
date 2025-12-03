using UnityEngine;

/*
 * Enemy Definer
 * 
 * A system that defines health, attack range, damage, and gold rewards for different enemy types
 * 
 * Features:
 * - Configurable stats for different enemy types
 * - Tier-based variations for squad members
 * - Gold drop system with random ranges
 * - Integration with enemy AI and health systems
 * - Flexible stat configuration per enemy type
 * 
 * Usage:
 * Attach to enemy game objects in prefabs
 * Configure stats and gold drops in inspector for each enemy type
 * 
 * Enemy Types Supported:
 * - Refugees: Low stats, low gold
 * - Bandits: Medium stats, medium gold
 * - Axeman: High damage melee, good gold
 * - Spearman: Medium range with good stats, good gold
 * - Knights: High health and damage, high gold
 * - Siege units: Very high stats, very high gold
 */

//==================================================================================================

/*
 * 敌人定义器
 * 
 * 定义不同敌人类型的血量、攻击范围、伤害和金币奖励的系统
 * 
 * 功能特点:
 * - 不同敌人类型的可配置属性
 * - 战团成员等级变化
 * - 随机范围的金币掉落系统
 * - 与敌人AI和血量系统的集成
 * - 每种敌人类型的灵活属性配置
 * 
 * 用途:
 * 添加到敌人预制体对象上
 * 在检查器中为每种敌人类型配置属性和金币掉落
 * 
 * 支持的敌人类型:
 * - 难民: 低属性，低金币
 * - 强盗: 中等属性，中等金币
 * - 斧手: 高伤害近战，良好金币
 * - 矛手: 中等范围良好属性，良好金币
 * - 骑士: 高血量和伤害，高金币
 * - 攻城单位: 极高属性，极高金币
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
    
    [Header("Gold Drop Stats / 金币掉落属性")]
    [Tooltip("Minimum gold drop / 最小金币掉落")]
    public int minGold = 1;
    
    [Tooltip("Maximum gold drop / 最大金币掉落")]
    public int maxGold = 5;
    
    [Header("Special Properties / 特殊属性")]
    [Tooltip("Armor value (reduces incoming damage) / 护甲值（减少受到伤害）")]
    public float armor = 0f;
    
    [Tooltip("Special abilities or resistances / 特殊能力或抗性")]
    public string specialAbilities = "";
}

public class EnemyDefiner : MonoBehaviour
{
    [Header("Enemy Configuration / 敌人配置")]
    [Tooltip("Enemy type for stat and gold configuration / 用于属性和金币配置的敌人类型")]
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
        moveSpeed = 1.5f,
        minGold = 1,
        maxGold = 3
    };
    
    [Tooltip("Stats for Bandit type / 强盗类型属性")]
    public EnemyStats banditStats = new EnemyStats 
    { 
        maxHealth = 60f, 
        attackRange = 2f, 
        attackDamage = 12f, 
        moveSpeed = 2f,
        minGold = 3,
        maxGold = 7
    };
    
    [Tooltip("Stats for Axeman type / 斧手类型属性")]
    public EnemyStats axemanStats = new EnemyStats 
    { 
        maxHealth = 100f, 
        attackRange = 2.5f, 
        attackDamage = 25f, 
        moveSpeed = 1.8f,
        minGold = 5,
        maxGold = 12
    };
    
    [Tooltip("Stats for Spearman type / 矛手类型属性")]
    public EnemyStats spearmanStats = new EnemyStats 
    { 
        maxHealth = 80f, 
        attackRange = 3f, 
        attackDamage = 18f, 
        moveSpeed = 1.6f,
        minGold = 5,
        maxGold = 12
    };
    
    [Tooltip("Stats for Knight type / 骑士类型属性")]
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
    
    [Tooltip("Stats for Siege unit type / 攻城单位类型属性")]
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
    
    [Tooltip("Base stats for Squad member type / 战团成员类型基础属性")]
    public EnemyStats squadMemberStats = new EnemyStats 
    { 
        maxHealth = 70f, 
        attackRange = 2f, 
        attackDamage = 15f, 
        moveSpeed = 1.8f,
        minGold = 8,
        maxGold = 18
    };

    [Header("Squad Tier Multipliers / 战团等级倍数")]
    [Tooltip("Health multiplier for squad tier 1 / 战团1级血量倍数")]
    public float squadTier1HealthMultiplier = 1.0f;
    
    [Tooltip("Damage multiplier for squad tier 1 / 战团1级伤害倍数")]
    public float squadTier1DamageMultiplier = 1.0f;
    
    [Tooltip("Gold multiplier for squad tier 1 / 战团1级金币倍数")]
    public float squadTier1GoldMultiplier = 1.0f;
    
    [Tooltip("Health multiplier for squad tier 2 / 战团2级血量倍数")]
    public float squadTier2HealthMultiplier = 1.5f;
    
    [Tooltip("Damage multiplier for squad tier 2 / 战团2级伤害倍数")]
    public float squadTier2DamageMultiplier = 1.3f;
    
    [Tooltip("Gold multiplier for squad tier 2 / 战团2级金币倍数")]
    public float squadTier2GoldMultiplier = 1.2f;
    
    [Tooltip("Health multiplier for squad tier 3 / 战团3级血量倍数")]
    public float squadTier3HealthMultiplier = 2.0f;
    
    [Tooltip("Damage multiplier for squad tier 3 / 战团3级伤害倍数")]
    public float squadTier3DamageMultiplier = 1.8f;
    
    [Tooltip("Gold multiplier for squad tier 3 / 战团3级金币倍数")]
    public float squadTier3GoldMultiplier = 1.5f;

    [Header("Current Stats / 当前属性")]
    [HideInInspector] public float currentMaxHealth;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentAttackRange;
    [HideInInspector] public float currentAttackDamage;
    [HideInInspector] public float currentAttackSpeed;
    [HideInInspector] public float currentMoveSpeed;
    [HideInInspector] public float currentArmor;
    [HideInInspector] public int currentMinGold;
    [HideInInspector] public int currentMaxGold;

    [Header("Game References / 游戏引用")]
    [Tooltip("Reference to enemy health system / 敌人血量系统引用")]
    public Enemy enemyHealth; // 使用 Enemy 组件代替不存在的 EnemyHealth
    
    [Tooltip("Reference to the main Enemy component for movement speed / 主 Enemy 组件引用，用于移动速度")]
    public Enemy enemyCore; // 重命名以明确这是主敌人行为组件
    
    [Tooltip("Reference to resource manager for gold drops / 资源管理器引用用于金币掉落")]
    public ResourceManager resourceManager;

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
        float maxHealth = baseStats.maxHealth;
        currentAttackRange = baseStats.attackRange;
        currentAttackDamage = baseStats.attackDamage;
        currentAttackSpeed = baseStats.attackSpeed;
        currentMoveSpeed = baseStats.moveSpeed;
        currentArmor = baseStats.armor;
        currentMinGold = baseStats.minGold;
        currentMaxGold = baseStats.maxGold;

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
            currentAttackDamage *= damageMultiplier;
            currentMinGold = Mathf.RoundToInt(currentMinGold * goldMultiplier);
            currentMaxGold = Mathf.RoundToInt(currentMaxGold * goldMultiplier);
        }

        // Use the SetMaxHealth method to properly initialize and synchronize with EnemyHealth component
        SetMaxHealth(maxHealth);
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
            enemyHealth = GetComponent<Enemy>();
        }
        if (enemyHealth != null)
        {
            // TODO: Implement SetMaxHealth in Enemy class
            // enemyHealth.SetMaxHealth(currentMaxHealth);
            
            // TODO: Implement SetCurrentHealth in Enemy class
            // enemyHealth.SetCurrentHealth(currentHealth);
            
            // TODO: Implement SetArmor in Enemy class
            // enemyHealth.SetArmor(currentArmor);
            
            // Temporary workaround - just log the intended actions
            Debug.Log($"Would set enemy max health to: {currentMaxHealth}, current health to: {currentHealth}, armor to: {currentArmor}", this);
        }

        // Apply movement speed to the core Enemy component
        if (enemyCore == null)
        {
            enemyCore = GetComponent<Enemy>();
        }
        if (enemyCore != null)
        {
            enemyCore.speed = currentMoveSpeed; // 直接设置速度
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
            // Calculate how much health to add/subtract to reach desired value
            float healthDiff = currentHealth - enemyHealth.GetCurrentHealth();
            if (healthDiff > 0)
            {
                enemyHealth.Heal(healthDiff);
            }
            else if (healthDiff < 0)
            {
                enemyHealth.TakeDamage(-healthDiff);
            }
        }

        // Check if enemy is defeated
        if (currentHealth <= 0)
        {
            OnEnemyDefeated();
        }
    }

    /// <summary>
    /// Called when enemy is defeated - handles gold drop
    /// 敌人被击败时调用 - 处理金币掉落
    /// </summary>
    void OnEnemyDefeated()
    {
        Debug.Log($"Enemy {enemyType} defeated!", this);
        
        // Calculate and drop gold
        DropGold();
        
        // You can add other defeat effects here
        // Animation, particle effects, etc.
        
        // Destroy the game object
        Destroy(gameObject);
    }

    /// <summary>
    /// Drop gold based on enemy type and current stats
    /// 根据敌人类型和当前属性掉落金币
    /// </summary>
    void DropGold()
    {
        if (resourceManager == null)
        {
            resourceManager = ResourceManager.Instance;
        }

        if (resourceManager != null)
        {
            // Generate random gold amount within range
            int goldAmount = Random.Range(currentMinGold, currentMaxGold + 1);
            
            // Apply squad multiplier if this is a squad member
            if (isSquadMember)
            {
                goldAmount = Mathf.RoundToInt(goldAmount * GetSquadGoldMultiplier());
            }
            
            // Add gold to resource manager
            resourceManager.AddGold(goldAmount);
            
            Debug.Log($"Enemy dropped {goldAmount} gold", this);
            
            // You can add visual effects for gold pickup here
            // Create gold pickup effect, play sound, etc.
        }
    }

    /// <summary>
    /// Get squad gold multiplier based on tier
    /// 获取基于等级的战团金币倍数
    /// </summary>
    float GetSquadGoldMultiplier()
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
    /// Heal the enemy
    /// 治疗敌人
    /// </summary>
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, currentMaxHealth);
        
        if (enemyHealth != null)
        {
            // TODO: Implement SetCurrentHealth in Enemy class
            // enemyHealth.SetCurrentHealth(currentHealth);
            
            // Temporary workaround - just log the intended action
            Debug.Log($"Would set enemy current health to: {currentHealth}", this);
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
               $"Speed: {currentMoveSpeed}, Gold: {currentMinGold}-{currentMaxGold}, " +
               $"Armor: {currentArmor}";
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
    public int GetMinGold() => currentMinGold;
    public int GetMaxGold() => currentMaxGold;
    public bool IsSquadMember() => isSquadMember;
    public int GetSquadTier() => squadTier;
    public bool IsAlive() => currentHealth > 0;
    
    // Public setters for dynamic modifications
    // Public setters for dynamic modifications
    public void SetCurrentHealth(float health) 
    { 
        currentHealth = Mathf.Clamp(health, 0, currentMaxHealth); 
        if (enemyHealth != null) 
        {
            // Calculate how much health to add/subtract to reach desired value
            float healthDiff = currentHealth - enemyHealth.GetCurrentHealth();
            if (healthDiff > 0)
            {
                enemyHealth.Heal(healthDiff);
            }
            else if (healthDiff < 0)
            {
                enemyHealth.TakeDamage(-healthDiff);
            }
        }
        // 如果没有找到 EnemyHealth 组件，则只更新内部状态。
        // 更新 Enemy 组件中的生命值超出了本脚本的职责范围，
        // 因为 Enemy 组件可能不包含生命值逻辑。
    }
    
    public void SetMaxHealth(float maxHealth) 
    { 
        // 计算当前血量比例以在更改最大生命值时保持比例
        float healthRatio = currentHealth / Mathf.Max(currentMaxHealth, 1f);
        currentMaxHealth = maxHealth;
        currentHealth = maxHealth * healthRatio;
        
        if (enemyHealth != null)
        {
            // 使用相同的比例来调整 Enemy 组件的血量
            float enemyHealthRatio = enemyHealth.GetCurrentHealth() / Mathf.Max(enemyHealth.GetMaxHealth(), 1f);
            float newCurrentHealth = currentMaxHealth * enemyHealthRatio;
            
            // 使用现有方法应用更改
            float healthDiff = newCurrentHealth - enemyHealth.GetCurrentHealth();
            if (healthDiff > 0)
            {
                enemyHealth.Heal(healthDiff);
            }
            else if (healthDiff < 0)
            {
                enemyHealth.TakeDamage(-healthDiff);
            }
            
            Debug.Log($"Adjusting enemy max health to {currentMaxHealth}, current health to {newCurrentHealth}", this);
        }
    }
    
    public void SetArmor(float armor) 
    { 
        currentArmor = armor;
        if (enemyHealth != null)
        {
            // Enemy class doesn't have SetArmor method
            // Just log the intended change for now
            Debug.Log($"Setting armor to: {currentArmor}", this);
        }
    }
}