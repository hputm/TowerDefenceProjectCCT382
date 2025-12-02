using UnityEngine;

/// <summary>
/// Road Block implementation.
/// Inherits from DefenseBuilding but has no attack capability.
/// Can only be placed on roads and serves purely as a blocker.
/// </summary>
public class RoadBlock : DefenseBuilding
{
    protected override void Start()
    {
        // Set placement constraint - road blocks can only be placed on roads
        placementType = PlacementType.RoadOnly;
        buildingType = "Road Block";
        hasAttackCapability = false;  // Road blocks cannot attack
        blocksMovement = true;        // Road blocks always block movement
        
        base.Start();
    }
    
    protected override void InitializeBuilding()
    {
        // Make sure attack capability is disabled
        hasAttackCapability = false;
        canAttack = false;
        
        base.InitializeBuilding();
    }
    
    // Override Update to prevent any attack behavior
    protected override void Update()
    {
        // Call base update but skip attack logic
        if (!isInitialized) return;
        
        // Update base building functionality (health, etc.)
        base.Update();
        
        // Skip attack logic since road blocks can't attack
    }
    
    // Override attack methods to ensure they do nothing
    protected override void FindAndAttackTarget() { }
    protected override Transform FindClosestEnemy() { return null; }
    protected override bool IsEnemyInRange(Transform enemy) { return false; }
    protected override void AttackTarget(Transform target) { }
    
    #region Upgrade System
    
    /// <summary>
    /// Upgrade the road block to the next tier
    /// </summary>
    /// <returns>True if upgrade was successful</returns>
    public override bool UpgradeBuilding()
    {
        // Check if already at max tier
        if (currentTier >= maxTier)
        {
            Debug.Log("Road Block is already at maximum tier", this);
            return false;
        }
        
        // Check if player can afford upgrade
        if (!CanAffordUpgrade())
        {
            Debug.Log("Cannot afford Road Block upgrade", this);
            return false;
        }
        
        // Spend resources
        SpendUpgradeResources();
        
        // Perform upgrade
        currentTier++;
        ApplyTierStats();
        
        Debug.Log($"Road Block upgraded to tier {currentTier}", this);
        return true;
    }
    
    /// <summary>
    /// Get the cost to upgrade to the next tier
    /// </summary>
    /// <returns>Gold cost for upgrade</returns>
    public override int GetUpgradeCost()
    {
        // Return different costs based on current tier
        switch (currentTier)
        {
            case 1: return 50;   // Cost to upgrade from tier 1 to 2
            default: return 0;   // Already at max tier
        }
    }
    
    #endregion
}