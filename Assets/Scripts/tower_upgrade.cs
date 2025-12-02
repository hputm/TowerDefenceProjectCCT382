using UnityEngine;
using System.Collections.Generic;

/*
 * Tower Upgrade System
 * 
 * A tower defense upgrade system supporting multiple tower types with different upgrade paths
 * 
 * Features:
 * - Multiple tower type support (Arrow Tower, Watchpost)
 * - Tier-based upgrades (Arrow Tower: 3 tiers, Watchpost: 2 tiers)
 * - Gold-based upgrade requirements (primary currency)
 * - Optional resource requirements (wood, stone - for future development)
 * - Visual upgrade effects
 * - Upgrade condition validation
 * 
 * Usage:
 * Attach to each tower game object that supports upgrades
 * Configure upgrade costs and conditions in inspector
 * 
 * Upgrade Conditions:
 * - Sufficient gold resources
 * - Current tower tier is not max tier
 * - Required conditions met (if any)
 * 
 * Tower Types Supported:
 * Arrow Tower: Tier 1 → Tier 2 → Tier 3
 * Watchpost: Tier 1 → Tier 2
 */

//==================================================================================================

/*
 * 防御塔升级系统
 * 
 * 塔防游戏升级系统，支持多种塔类型和不同升级路径
 * 
 * 功能特点:
 * - 多种塔类型支持（箭塔、哨卡）
 * - 等级升级（箭塔：3级，哨卡：2级）
 * - 金币为基础的升级需求（主要货币）
 * - 可选资源需求（木材、石材 - 待开发功能）
 * - 视觉升级效果
 * - 升级条件验证
 * 
 * 用途:
 * 添加到每个支持升级的塔对象上
 * 在检查器中配置升级成本和条件
 * 
 * 升级条件:
 * - 拥有足够金币资源
 * - 当前塔等级未达上限
 * - 满足所需条件（如果有）
 * 
 * 支持的塔类型:
 * 箭塔: 1级 → 2级 → 3级
 * 哨卡: 1级 → 2级
 */

// Removed duplicate TowerType enum - it's already defined in building_definer.cs
// TowerType 定义已在 building_definer.cs 中声明，此处移除重复定义

public enum TowerTier
{
    Tier1 = 1,
    Tier2 = 2,
    Tier3 = 3
}

[System.Serializable]
public class UpgradeCost
{
    [Tooltip("Required gold for upgrade / 升级所需金币")]
    public int goldRequired = 100;          // Required gold for upgrade
    
    [Header("Optional Resources (Future Development) / 可选资源（待开发）")]
    [Tooltip("Required wood for upgrade (optional) / 升级所需木材（可选）")]
    public int woodRequired = 0;            // Required wood for upgrade (optional)
    
    [Tooltip("Required stone for upgrade (optional) / 升级所需石材（可选）")]
    public int stoneRequired = 0;           // Required stone for upgrade (optional)
    
    [Header("Other Requirements / 其他要求")]
    [Tooltip("Required wave to unlock / 解锁所需波次")]
    public int requiredWave = 0;            // Required wave to unlock
    
    [Tooltip("Custom condition description / 自定义条件描述")]
    public string customCondition = "";     // Custom condition description
}

public class TowerUpgradeSystem : MonoBehaviour
{
    [Header("Tower Configuration / 塔配置")]
    public TowerType towerType;           // Tower type
    public TowerTier currentTier = TowerTier.Tier1; // Current tier
    public TowerTier maxTier = TowerTier.Tier3;     // Max tier for this tower

    [Header("Upgrade Costs / 升级成本")]
    [Tooltip("Cost to upgrade from Tier 1 to Tier 2 / 从1级升到2级的成本")]
    public UpgradeCost tier2Cost = new UpgradeCost { goldRequired = 100, requiredWave = 2 };
    
    [Tooltip("Cost to upgrade from Tier 2 to Tier 3 / 从2级升到3级的成本")]
    public UpgradeCost tier3Cost = new UpgradeCost { goldRequired = 250, requiredWave = 5 };
    
    [Header("Visual Components / 视觉组件")]
    [Tooltip("Visual representations for each tier / 每个等级的视觉表现")]
    public GameObject[] tierVisuals;      // Visual representations for each tier
    
    [Tooltip("Sprites for each tier (2D games) / 每个等级的精灵（2D游戏）")]
    public Sprite[] tierSprites;          // Sprites for each tier (2D games)
    
    [Tooltip("Renderers for visual effects / 渲染器用于视觉效果")]
    public Renderer[] tierRenderers;      // Renderers for visual effects

    [Header("Upgrade Effects / 升级效果")]
    [Tooltip("Damage multiplier per tier / 每级伤害倍数")]
    public float damageMultiplier = 1.0f; // Damage multiplier per tier
    
    [Tooltip("Range multiplier per tier / 每级射程倍数")]
    public float rangeMultiplier = 1.0f;  // Range multiplier per tier
    
    [Tooltip("Attack speed multiplier / 攻击速度倍数")]
    public float attackSpeedMultiplier = 1.0f; // Attack speed multiplier
    
    [Tooltip("Health multiplier per tier / 每级生命倍数")]
    public float healthMultiplier = 1.0f; // Health multiplier per tier

    [Header("Game References / 游戏引用")]
    [Tooltip("Reference to resource manager / 资源管理器引用")]
    public ResourceManager resourceManager; // Reference to resource manager
    
    [Tooltip("Reference to UI manager / UI管理器引用")]
    public building_definer uiManager;                 // Reference to UI manager

    [Header("Audio Settings / 音效设置")]
    [Tooltip("Upgrade sound effect / 升级音效")]
    public AudioClip upgradeSound;              // Upgrade sound effect
    
    [Tooltip("Audio source component / 音频源组件")]
    public AudioSource audioSource;             // Audio source component

    // Private variables
    private bool isUpgrading = false;
    private float upgradeProgress = 0f;

    void Start()
    {
        ValidateConfiguration();
        UpdateVisuals();
    }

    void ValidateConfiguration()
    {
        if (resourceManager == null)
        {
            resourceManager = ResourceManager.Instance;
            if (resourceManager == null)
            {
                Debug.LogError("ResourceManager not found in scene", this);
            }
        }

        /*if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogWarning("UIManager not found in scene", this);
            }
        }*/

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Set max tier based on tower type
        switch (towerType)
        {
            case TowerType.ArrowTower:
                maxTier = TowerTier.Tier3;
                break;
            case TowerType.Watchpost:
                maxTier = TowerTier.Tier2;
                break;
        }
    }

    /// <summary>
    /// Check if tower can be upgraded
    /// 检查塔是否可以升级
    /// </summary>
    public bool CanUpgrade()
    {
        // Check if already max tier
        if (currentTier >= maxTier)
        {
            Debug.Log($"Tower {name} is already at max tier", this);
            return false;
        }

        // Get required cost for next tier
        UpgradeCost requiredCost = GetRequiredCost(currentTier);
        if (requiredCost == null)
        {
            Debug.LogError($"No upgrade cost defined for tier {currentTier}", this);
            return false;
        }

        // Check gold requirements (primary currency)
        if (resourceManager != null)
        {
            if (resourceManager.GetGold() < requiredCost.goldRequired)
            {
                Debug.Log($"Insufficient gold for upgrade. Required: {requiredCost.goldRequired}, Available: {resourceManager.GetGold()}", this);
                return false;
            }
        }

        // Check optional resource requirements (for future development)
        if (requiredCost.woodRequired > 0 && resourceManager != null)
        {
            if (resourceManager.GetWood() < requiredCost.woodRequired)
            {
                Debug.Log($"Insufficient wood for upgrade. Required: {requiredCost.woodRequired}", this);
                return false;
            }
        }
        
        if (requiredCost.stoneRequired > 0 && resourceManager != null)
        {
            if (resourceManager.GetStone() < requiredCost.stoneRequired)
            {
                Debug.Log($"Insufficient stone for upgrade. Required: {requiredCost.stoneRequired}", this);
                return false;
            }
        }

        // Check wave requirement
        if (requiredCost.requiredWave > 0)
        {
            var gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null && gameManager.GetCurrentWave() < requiredCost.requiredWave)
            {
                Debug.Log($"Wave {requiredCost.requiredWave} required for upgrade", this);
                return false;
            }
        }

        // Check custom conditions
        if (!string.IsNullOrEmpty(requiredCost.customCondition))
        {
            if (!CheckCustomCondition(requiredCost.customCondition))
            {
                Debug.Log($"Custom condition not met: {requiredCost.customCondition}", this);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Attempt to upgrade the tower
    /// 尝试升级塔
    /// </summary>
    public bool UpgradeTower()
    {
        if (!CanUpgrade())
        {
            Debug.Log("Cannot upgrade tower - requirements not met", this);
            return false;
        }

        UpgradeCost requiredCost = GetRequiredCost(currentTier);
        
        // Deduct gold (primary resource)
        if (resourceManager != null)
        {
            resourceManager.SpendGold(requiredCost.goldRequired);
        }

        // Deduct optional resources (for future development)
        if (requiredCost.woodRequired > 0 && resourceManager != null)
        {
            resourceManager.SpendWood(requiredCost.woodRequired);
        }
        if (requiredCost.stoneRequired > 0 && resourceManager != null)
        {
            resourceManager.SpendStone(requiredCost.stoneRequired);
        }

        // Perform upgrade
        currentTier = (TowerTier)((int)currentTier + 1);
        
        // Apply upgrade effects
        ApplyUpgradeEffects();
        
        // Update visuals
        UpdateVisuals();
        
        // Play upgrade sound
        if (audioSource != null && upgradeSound != null)
        {
            audioSource.PlayOneShot(upgradeSound);
        }

        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateTowerUpgradeUI(this);
        }

        Debug.Log($"Tower {name} upgraded to {currentTier}. Gold spent: {requiredCost.goldRequired}", this);
        return true;
    }

    /// <summary>
    /// Get upgrade cost for specified tier
    /// 获取指定等级的升级成本
    /// </summary>
    UpgradeCost GetRequiredCost(TowerTier currentTier)
    {
        switch (currentTier)
        {
            case TowerTier.Tier1:
                return tier2Cost;
            case TowerTier.Tier2:
                return tier3Cost;
            default:
                return null;
        }
    }

    /// <summary>
    /// Apply upgrade effects based on new tier
    /// 根据新等级应用升级效果
    /// </summary>
    void ApplyUpgradeEffects()
    {
        // Get the tower's attack component (assuming it has one)
        var towerAttack = GetComponent<TowerAttack>(); // Replace with your tower attack component
        if (towerAttack != null)
        {
            // Apply damage multiplier based on tier
            float damageMultiplier = 1.0f;
            switch (currentTier)
            {
                case TowerTier.Tier2:
                    damageMultiplier = 1.5f; // 50% more damage
                    break;
                case TowerTier.Tier3:
                    damageMultiplier = 2.0f; // 100% more damage
                    break;
            }
            towerAttack.SetDamageMultiplier(damageMultiplier);
            
            // Apply range multiplier
            float rangeMultiplier = 1.0f;
            switch (currentTier)
            {
                case TowerTier.Tier2:
                    rangeMultiplier = 1.2f; // 20% more range
                    break;
                case TowerTier.Tier3:
                    rangeMultiplier = 1.4f; // 40% more range
                    break;
            }
            towerAttack.SetRangeMultiplier(rangeMultiplier);
        }

        // Update tower health if applicable
        var healthComponent = GetComponent<TowerHealth>(); // Replace with your health component
        if (healthComponent != null)
        {
            float healthMultiplier = 1.0f;
            switch (currentTier)
            {
                case TowerTier.Tier2:
                    healthMultiplier = 1.5f;
                    break;
                case TowerTier.Tier3:
                    healthMultiplier = 2.0f;
                    break;
            }
            healthComponent.SetHealthMultiplier(healthMultiplier);
        }
    }

    /// <summary>
    /// Update visual representation based on current tier
    /// 根据当前等级更新视觉表现
    /// </summary>
    void UpdateVisuals()
    {
        // Hide all tier visuals first
        if (tierVisuals != null)
        {
            foreach (var visual in tierVisuals)
            {
                if (visual != null)
                    visual.SetActive(false);
            }

            // Show current tier visual
            int index = (int)currentTier - 1;
            if (index >= 0 && index < tierVisuals.Length && tierVisuals[index] != null)
            {
                tierVisuals[index].SetActive(true);
            }
        }

        // Update sprite if available
        if (tierSprites != null && tierSprites.Length > 0)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                int index = (int)currentTier - 1;
                if (index >= 0 && index < tierSprites.Length)
                {
                    spriteRenderer.sprite = tierSprites[index];
                }
            }
        }

        // Update material color or other visual effects
        if (tierRenderers != null)
        {
            foreach (var renderer in tierRenderers)
            {
                if (renderer != null)
                {
                    // Apply different materials or colors based on tier
                    Material newMaterial = GetMaterialForTier(currentTier);
                    if (newMaterial != null)
                    {
                        renderer.material = newMaterial;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get material for specific tier
    /// 获取特定等级的材质
    /// </summary>
    Material GetMaterialForTier(TowerTier tier)
    {
        // This would return different materials based on tier
        // Implementation depends on your material setup
        return null; // Placeholder
    }

    /// <summary>
    /// Check custom upgrade conditions
    /// 检查自定义升级条件
    /// </summary>
    bool CheckCustomCondition(string condition)
    {
        // Implement custom condition checks here
        // Example: "defeated_boss", "built_5_towers", etc.
        return true; // Placeholder
    }

    /// <summary>
    /// Get upgrade cost for next tier
    /// 获取下一级升级成本
    /// </summary>
    public UpgradeCost GetNextUpgradeCost()
    {
        return GetRequiredCost(currentTier);
    }

    /// <summary>
    /// Get upgrade progress (for visual indicators)
    /// 获取升级进度（用于视觉指示器）
    /// </summary>
    public float GetUpgradeProgress()
    {
        if (!CanUpgrade()) return 0f;
        
        UpgradeCost cost = GetNextUpgradeCost();
        if (cost == null) return 0f;
        
        if (resourceManager == null) return 0f;
        
        // Calculate progress based on gold (primary resource)
        float goldProgress = (float)resourceManager.GetGold() / cost.goldRequired;
        
        // For optional resources (future development)
        float woodProgress = cost.woodRequired > 0 ? 
            (float)resourceManager.GetWood() / cost.woodRequired : 1f;
        float stoneProgress = cost.stoneRequired > 0 ? 
            (float)resourceManager.GetStone() / cost.stoneRequired : 1f;
        
        // Return the minimum progress (bottleneck resource)
        // Currently focused on gold, optional resources are secondary
        return Mathf.Min(goldProgress, woodProgress, stoneProgress);
    }

    // Public getters for UI and other systems
    public TowerType GetTowerType() => towerType;
    public TowerTier GetCurrentTier() => currentTier;
    public TowerTier GetMaxTier() => maxTier;
    public bool IsMaxTier() => currentTier >= maxTier;
}