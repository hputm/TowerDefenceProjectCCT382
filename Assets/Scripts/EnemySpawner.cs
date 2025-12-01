using UnityEngine;
using System.Collections.Generic;

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
 * 
 * Usage:
 * Create an empty object in scene and attach this script
 * Configure spawn points and enemy prefabs
 * 
 * Configuration Guide:
 * spawnPoints: Add 1-3 empty objects as spawn points
 * enemyPrefabs: Add various enemy prefabs
 *   - 3 Spearman types
 *   - 2 Axeman types  
 *   - 1 Two-man squad (as special enemy type)
 *   - 2 Three-man squads (as special enemy types)
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
 * 
 * 用途:
 * 在场景中创建空物体并添加此脚本
 * 配置生成点和敌人预制体
 * 
 * 配置说明:
 * spawnPoints: 添加1-3个空物体作为敌人生成点
 * enemyPrefabs: 添加各种敌人预制体
 *   - 矛兵3种
 *   - 斧手2种  
 *   - 2人战团1个（作为特殊敌人类型）
 *   - 3人战团2个（作为特殊敌人类型）
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

public class SiegeEnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyPrefabData
    {
        [Tooltip("Enemy prefab")]
        public GameObject prefab;
        
        [Tooltip("Spawn weight (higher value = higher probability)")]
        [Range(0, 100)]
        public float spawnWeight = 1f;
        
        [Tooltip("Enemy type name")]
        public string enemyType = "Enemy";
    }

    [Header("Spawn Configuration")]
    [Tooltip("Enemy spawn points (at least 1 required)")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("Enemy prefab configuration")]
    public List<EnemyPrefabData> enemyPrefabs = new List<EnemyPrefabData>();

    [Tooltip("Number range per wave")]
    public Vector2Int spawnCountRange = new Vector2Int(1, 3);

    [Tooltip("Spawn interval (seconds)")]
    public float spawnInterval = 5f;

    [Tooltip("Max enemy limit in scene (0 = no limit)")]
    public int maxEnemiesInScene = 0;

    [Header("Wave Control")]
    [Tooltip("Enable wave mode")]
    public bool enableWaveMode = true;
    
    [Tooltip("Current wave number")]
    public int currentWave = 1;
    
    [Tooltip("Wave interval time")]
    public float waveInterval = 10f;

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
            Debug.LogError("Please set at least one spawn point", this);
            enabled = false;
            return;
        }

        if (enemyPrefabs.Count == 0)
        {
            Debug.LogError("Please set at least one enemy prefab", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            if (enemyPrefabs[i].prefab == null)
            {
                Debug.LogError($"Enemy prefab {i} not set", this);
                enabled = false;
                return;
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
        Debug.Log($"Starting wave {currentWave} enemy spawning");
    }

    public void SpawnEnemies()
    {
        // Limit total enemies in scene
        if (maxEnemiesInScene > 0)
        {
            int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (currentEnemies >= maxEnemiesInScene)
            {
                Debug.Log("Enemy limit reached, pausing spawning");
                return;
            }
        }

        int spawnCount = Random.Range(spawnCountRange.x, spawnCountRange.y + 1);
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < spawnCount; i++)
        {
            if (availablePoints.Count == 0) 
            {
                Debug.LogWarning("Insufficient spawn points available", this);
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
                
                // Add wave info or other components to enemy
                var enemyComponent = enemy.GetComponent<EnemyAI>(); // Assume you have EnemyAI component
                if (enemyComponent != null)
                {
                    enemyComponent.waveNumber = currentWave;
                }
                
                Debug.Log($"Spawned enemy: {selectedEnemy.enemyType} (Wave: {currentWave})");
            }
        }
    }

    /// <summary>
    /// Randomly select enemy prefab by weight
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

    // Public method: manually start new wave
    public void StartNewWave()
    {
        currentWave++;
        m_IsInWave = true;
        m_Timer = 0f;
    }

    // Public method: get current wave info
    public int GetCurrentWave() => currentWave;
    public bool IsInWave() => m_IsInWave;
}

public class SiegeEnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyPrefabData
    {
        [Tooltip("Enemy prefab / 敌人预制体")]
        public GameObject prefab;
        
        [Tooltip("Spawn weight (higher value = higher probability) / 生成权重（数值越大，生成概率越高）")]
        [Range(0, 100)]
        public float spawnWeight = 1f;
        
        [Tooltip("Enemy type name / 敌人类型名称")]
        public string enemyType = "Enemy";
        
        [Tooltip("Is squad unit (multiple units in one prefab) / 是否为战团单位（一个预制体包含多个单位）")]
        public bool isSquad = false;
        
        [Tooltip("Squad size (only for squad units) / 战团中单位数量（仅对战团有效）")]
        public int squadSize = 1;
    }

    [Header("Spawn Configuration / 生成配置")]
    [Tooltip("Enemy spawn points (at least 1 required) / 敌人生成点（至少设置1个）")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("Enemy prefab configuration / 敌人预制体配置")]
    public List<EnemyPrefabData> enemyPrefabs = new List<EnemyPrefabData>();

    [Tooltip("Number range per wave / 每波生成的敌人数量范围")]
    public Vector2Int spawnCountRange = new Vector2Int(1, 3);

    [Tooltip("Spawn interval (seconds) / 生成间隔（秒）")]
    public float spawnInterval = 5f;

    [Tooltip("Max enemy limit in scene (0 = no limit) / 场景中最大敌人数量限制（0表示无限制）")]
    public int maxEnemiesInScene = 0;

    [Header("Wave Control / 波次控制")]
    [Tooltip("Enable wave mode / 是否启用波次模式")]
    public bool enableWaveMode = true;
    
    [Tooltip("Current wave number / 当前波次")]
    public int currentWave = 1;
    
    [Tooltip("Wave interval time / 波次间隔时间")]
    public float waveInterval = 10f;

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
                
                // 检查是否完成当前波次 / Check if current wave is completed
                if (m_Timer > spawnInterval * 2) // 可以根据实际需求调整 / Adjust based on actual needs
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
        // 限制场景中的敌人总数 / Limit total enemies in scene
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

            // 根据权重随机选择敌人类型 / Randomly select enemy type by weight
            EnemyPrefabData selectedEnemy = SelectEnemyPrefabByWeight();

            if (selectedEnemy.prefab != null)
            {
                // 生成敌人 / Spawn enemy
                GameObject enemy = Instantiate(selectedEnemy.prefab, spawnPoint.position, spawnPoint.rotation);
                
                // 可以为敌人添加波次信息或其他组件 / Add wave info or other components to enemy
                var enemyComponent = enemy.GetComponent<EnemyAI>(); // 假设你有EnemyAI组件 / Assume you have EnemyAI component
                if (enemyComponent != null)
                {
                    enemyComponent.waveNumber = currentWave;
                }
                
                Debug.Log($"生成敌人: {selectedEnemy.enemyType} (波次: {currentWave}) / Spawned enemy: {selectedEnemy.enemyType} (Wave: {currentWave})");
            }
        }
    }

    /// <summary>
    /// 根据权重随机选择敌人预制体 / Randomly select enemy prefab by weight
    /// </summary>
    EnemyPrefabData SelectEnemyPrefabByWeight()
    {
        // 计算总权重 / Calculate total weight
        float totalWeight = 0f;
        foreach (var enemyData in enemyPrefabs)
        {
            totalWeight += enemyData.spawnWeight;
        }

        if (totalWeight <= 0f) return enemyPrefabs[0];

        // 生成随机值 / Generate random value
        float randomValue = Random.Range(0f, totalWeight);

        // 根据权重选择预制体 / Select prefab by weight
        float currentWeight = 0f;
        foreach (var enemyData in enemyPrefabs)
        {
            currentWeight += enemyData.spawnWeight;
            if (randomValue <= currentWeight)
            {
                return enemyData;
            }
        }

        // 如果没有找到（理论上不会发生），返回第一个 / If not found (theoretically impossible), return first
        return enemyPrefabs[0];
    }

    // 场景视图中绘制生成点标记 / Draw spawn point markers in scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 改为红色，更符合敌人主题 / Changed to red, more suitable for enemy theme
        foreach (var point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, 0.5f);
                Gizmos.DrawLine(point.position, point.position + point.up * 1f);
            }
        }
    }

    // 公共方法：手动开始新波次 / Public method: manually start new wave
    public void StartNewWave()
    {
        currentWave++;
        m_IsInWave = true;
        m_Timer = 0f;
    }

    // 公共方法：获取当前波次信息 / Public method: get current wave info
    public int GetCurrentWave() => currentWave;
    public bool IsInWave() => m_IsInWave;
}