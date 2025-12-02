using UnityEngine;
using System.Collections.Generic;

/*
 * Siege Enemy Spawner
 * 
 * A tower defense enemy spawning system supporting multiple enemy types, wave modes, and squad units
 * 
 * Features:
 * - Multiple enemy type configuration
 * - Wave-based spawning
 * - Squad unit support (treated as special enemy types)
 * - Weighted random spawning
 * - Scene population limits
 * - Integration with enemy definer system
 * 
 * Usage:
 * Create an empty object in scene and attach this script
 * Configure spawn points and enemy prefabs
 * 
 * Configuration Guide:
 * spawnPoints: Add 1-3 empty objects as spawn points
 * enemyPrefabs: Add various enemy prefabs (must have EnemyDefiner component)
 *   - Each prefab should have EnemyDefiner configured with stats
 *   - Supports different enemy types: Refugees, Bandits, Axeman, Spearman, Knights, Siege units
 * spawnInterval: Spawn interval time
 * spawnCountRange: Number range per wave
 * maxEnemiesInScene: Max enemy limit in scene
 * enableWaveMode: Enable wave mode
 * 
 * Game Integration:
 * For "Siege of the Keep" tower defense game, supporting medieval siege warfare enemy system
 * 
 * Special Features:
 * - Squad support: 2-3 member team prefabs treated as special enemy types
 * - Progressive difficulty: stronger enemies with each wave
 * - Weighted spawning: different enemy types by probability
 */

//==================================================================================================

/*
 * 围城敌人生成器
 * 
 * 用于塔防游戏的敌人生成系统，支持多种敌人类型、波次模式和战团单位
 * 
 * 功能特点:
 * - 支持多种敌人类型配置
 * - 波次生成模式
 * - 战团单位支持（作为特殊敌人类型处理）
 * - 权重随机生成
 * - 场景数量限制
 * - 与敌人定义器系统集成
 * 
 * 用途:
 * 在场景中创建空物体并添加此脚本
 * 配置生成点和敌人预制体
 * 
 * 配置说明:
 * spawnPoints: 添加1-3个空物体作为敌人生成点
 * enemyPrefabs: 添加各种敌人预制体（必须包含EnemyDefiner组件）
 *   - 每个预制体应配置EnemyDefiner属性
 *   - 支持不同敌人类型：难民、强盗、斧手、矛手、骑士、攻城单位
 * spawnInterval: 生成间隔时间
 * spawnCountRange: 每波生成数量范围
 * maxEnemiesInScene: 场景最大敌人数量限制
 * enableWaveMode: 是否启用波次模式
 * 
 * 游戏集成:
 * 适用于"Siege of the Keep"塔防游戏，支持中世纪围城战的敌人系统
 * 
 * 特色功能:
 * - 战团支持：2-3人小队预制体作为特殊敌人类型处理
 * - 波次递增：随波次增加更强敌人
 * - 权重生成：按概率生成不同类型敌人
 */

[System.Serializable]
public class EnemyPrefabData
{
    [Tooltip("Enemy prefab (must have EnemyDefiner component) / 敌人预制体（必须包含EnemyDefiner组件）")]
    public GameObject prefab;
    
    [Tooltip("Spawn weight (higher value = higher probability) / 生成权重（数值越大，生成概率越高）")]
    [Range(0, 100)]
    public float spawnWeight = 1f;
    
    [Tooltip("Enemy type name / 敌人类型名称")]
    public string enemyType = "Enemy";
    
    [Tooltip("Is this a squad unit / 是否为战团单位")]
    public bool isSquad = false;
    
    [Tooltip("Squad size (for squad units) / 战团大小（用于战团单位）")]
    public int squadSize = 1;
    
    [Tooltip("Squad tier (for squad units) / 战团等级（用于战团单位）")]
    public int squadTier = 1;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Configuration / 生成配置")]
    [Tooltip("Enemy spawn points (at least 1 required) / 敌人生成点（至少设置1个）")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("Enemy prefab configuration (each must have EnemyDefiner) / 敌人预制体配置（每个都必须包含EnemyDefiner）")]
    public List<EnemyPrefabData> enemyPrefabs = new List<EnemyPrefabData>();

    [Header("Spawn Timing / 生成时机")]
    [Tooltip("Spawn interval (seconds) / 生成间隔（秒）")]
    public float spawnInterval = 5f;
    
    [Tooltip("Number range per wave / 每波生成的敌人数量范围")]
    public Vector2Int spawnCountRange = new Vector2Int(1, 3);

    [Header("Population Limits / 人口限制")]
    [Tooltip("Max enemy limit in scene (0 = no limit) / 场景中最大敌人数量限制（0表示无限制）")]
    public int maxEnemiesInScene = 0;

    [Header("Wave Control / 波次控制")]
    [Tooltip("Enable wave mode / 是否启用波次模式")]
    public bool enableWaveMode = true;
    
    [Tooltip("Current wave number / 当前波次")]
    public int currentWave = 1;
    
    [Tooltip("Wave interval time / 波次间隔时间")]
    public float waveInterval = 10f;

    [Header("Advanced Settings / 高级设置")]
    [Tooltip("Enable difficulty scaling (higher waves = stronger enemies) / 启用难度缩放（高波次=更强敌人）")]
    public bool enableDifficultyScaling = true;
    
    [Tooltip("Difficulty multiplier per wave / 每波难度倍数")]
    public float difficultyMultiplierPerWave = 0.1f;

    // Private variables
    float m_Timer;
    float m_WaveTimer;
    bool m_IsInWave = false;

    void Start()
    {
        ValidateConfiguration();
    }

    void Update()
    {
        if (spawnPoints.Count == 0 || enemyPrefabs.Count == 0)
            return;

        if (enableWaveMode)
        {
            HandleWaveMode();
        }
        else
        {
            HandleContinuousSpawning();
        }
    }

    void ValidateConfiguration()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("请至少设置一个生成点 / Please set at least one spawn point", this);
            enabled = false;
            return;
        }

        if (enemyPrefabs.Count == 0)
        {
            Debug.LogError("请至少设置一种敌人预制体 / Please set at least one enemy prefab", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            if (enemyPrefabs[i].prefab == null)
            {
                Debug.LogError($"敌人预制体 {i} 未设置 / Enemy prefab {i} not set", this);
                enabled = false;
                return;
            }

            // Check if prefab has EnhancedEnemy component
            var enemy = enemyPrefabs[i].prefab.GetComponent<EnhancedEnemy>();
            if (enemy == null)
            {
                Debug.LogWarning($"敌人预制体 {enemyPrefabs[i].prefab.name} 缺少 EnhancedEnemy 组件 / Enemy prefab {enemyPrefabs[i].prefab.name} missing EnhancedEnemy component", this);
            }
        }
    }

    void HandleWaveMode()
    {
        if (!m_IsInWave)
        {
            m_WaveTimer += Time.deltaTime;
            if (m_WaveTimer >= waveInterval)
            {
                StartWave();
                m_WaveTimer = 0f;
            }
        }
        else
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= spawnInterval)
            {
                SpawnEnemies();
                m_Timer = 0f;
                
                // Check if current wave is completed
                if (m_Timer > spawnInterval * 2) // Adjust based on actual needs
                {
                    m_IsInWave = false;
                    currentWave++;
                }
            }
        }
    }

    void HandleContinuousSpawning()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= spawnInterval)
        {
            SpawnEnemies();
            m_Timer = 0f;
        }
    }

    void StartWave()
    {
        m_IsInWave = true;
        m_Timer = 0f;
        Debug.Log($"开始第 {currentWave} 波敌人生成 / Starting wave {currentWave} enemy spawning");
    }

    public void SpawnEnemies()
    {
        // Limit total enemies in scene
        if (maxEnemiesInScene > 0)
        {
            int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (currentEnemies >= maxEnemiesInScene)
            {
                Debug.Log("场景中敌人数量已达上限，暂停生成 / Enemy limit reached, pausing spawning");
                return;
            }
        }

        int spawnCount = Random.Range(spawnCountRange.x, spawnCountRange.y + 1);
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < spawnCount; i++)
        {
            if (availablePoints.Count == 0) 
            {
                Debug.LogWarning("可用生成点不足 / Insufficient spawn points available", this);
                break;
            }

            int pointIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[pointIndex];
            availablePoints.RemoveAt(pointIndex);

            // Randomly select enemy type by weight
            EnemyPrefabData selectedEnemy = SelectEnemyPrefabByWeight();

            if (selectedEnemy.prefab != null)
            {
                // Spawn enemy
                GameObject enemy = Instantiate(selectedEnemy.prefab, spawnPoint.position, spawnPoint.rotation);
                
                // Configure enemy based on wave and squad settings
                ConfigureEnemy(enemy, selectedEnemy);
                
                // Add wave info
                var enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    // You can add wave number to enemy if needed
                }
                
                Debug.Log($"生成敌人: {selectedEnemy.enemyType} (波次: {currentWave}) / Spawned enemy: {selectedEnemy.enemyType} (Wave: {currentWave})");
            }
        }
    }

    /// <summary>
    /// Configure enemy based on current wave and squad settings
    /// </summary>
    void ConfigureEnemy(GameObject enemy, EnemyPrefabData enemyData)
    {
        var enhancedEnemy = enemy.GetComponent<EnhancedEnemy>();
        if (enhancedEnemy != null)
        {
            // Apply squad settings if this is a squad member
            if (enemyData.isSquad)
            {
                enhancedEnemy.isSquadMember = true;
                enhancedEnemy.squadTier = enemyData.squadTier;
            }

            // Apply difficulty scaling based on current wave
            if (enableDifficultyScaling && currentWave > 1)
            {
                float difficultyScale = 1.0f + (currentWave * difficultyMultiplierPerWave);
                
                // Scale health
                enhancedEnemy.maxHealth *= difficultyScale;
                enhancedEnemy.currentHealth = enhancedEnemy.maxHealth;
                
                // Scale damage
                enhancedEnemy.attackDamage *= difficultyScale;
                
                // Scale other stats as needed
                enhancedEnemy.attackRange *= (1.0f + (currentWave * difficultyMultiplierPerWave * 0.05f)); // Smaller range increase
            }
        }
    }

    /// <summary>
    /// Randomly select enemy prefab by weight
    /// 根据权重随机选择敌人预制体
    /// </summary>
    EnemyPrefabData SelectEnemyPrefabByWeight()
    {
        // Calculate total weight
        float totalWeight = 0f;
        foreach (var enemyData in enemyPrefabs)
        {
            totalWeight += enemyData.spawnWeight;
        }

        if (totalWeight <= 0f) return enemyPrefabs[0];

        // Generate random value
        float randomValue = Random.Range(0f, totalWeight);

        // Select prefab by weight
        float currentWeight = 0f;
        foreach (var enemyData in enemyPrefabs)
        {
            currentWeight += enemyData.spawnWeight;
            if (randomValue <= currentWeight)
            {
                return enemyData;
            }
        }

        // If not found (theoretically impossible), return first
        return enemyPrefabs[0];
    }

    // Draw spawn point markers in scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Changed to red, more suitable for enemy theme
        foreach (var point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, 0.5f);
                Gizmos.DrawLine(point.position, point.position + point.up * 1f);
            }
        }
    }

    // Public methods for external control
    public void StartNewWave()
    {
        currentWave++;
        m_IsInWave = true;
        m_Timer = 0f;
    }

    public int GetCurrentWave() => currentWave;
    public bool IsInWave() => m_IsInWave;
    public bool IsSpawningEnabled() => enabled;
    
    public void SetSpawningEnabled(bool enabled) => this.enabled = enabled;
    public void SetSpawnInterval(float interval) => spawnInterval = interval;
    public void SetMaxEnemies(int max) => maxEnemiesInScene = max;
    public void SetSpawnCountRange(Vector2Int range) => spawnCountRange = range;
    public void SetCurrentWave(int wave) => currentWave = wave;
    public void SetDifficultyScaling(bool enabled) => enableDifficultyScaling = enabled;
    
    public float GetSpawnInterval() => spawnInterval;
    public int GetMaxEnemies() => maxEnemiesInScene;
    public Vector2Int GetSpawnCountRange() => spawnCountRange;
    public int GetTotalEnemyTypes() => enemyPrefabs.Count;
}