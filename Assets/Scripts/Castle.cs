using UnityEngine;

/// <summary>
/// Castle implementation.
/// Represents the player's main building that must be protected.
/// Cannot be placed by the player - it's a static part of the level.
/// </summary>
public class Castle : BuildingBase
{
    [Header("Castle Specific")]
    [Tooltip("Whether the castle can counter-attack enemies that reach it")]
    public bool hasCounterAttack = true;
    
    [Tooltip("Damage dealt to enemies that reach the castle")]
    public float counterAttackDamage = 20f;
    
    [Tooltip("Time between counter-attacks")]
    public float counterAttackCooldown = 1f;
    
    protected float counterAttackTimer = 0f;
    
    protected override void Start()
    {
        // Castle is a static building that cannot be placed by the player
        isStaticBuilding = true;
        isPlayerPlaceable = false;
        buildingType = "Castle";
        canAttack = hasCounterAttack;
        
        base.Start();
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (!isInitialized) return;
        
        // Update counter-attack timer
        if (counterAttackTimer > 0)
        {
            counterAttackTimer -= Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Handle enemies that reach the castle
    /// </summary>
    /// <param name="enemy">Enemy that reached the castle</param>
    public virtual void OnEnemyReachCastle(Enemy enemy)
    {
        // Notify game manager that an enemy reached the keep
        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.OnEnemyReachedKeep();
        }
        
        // Counter-attack the enemy if possible
        if (hasCounterAttack && counterAttackTimer <= 0 && currentHealth > 0)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(counterAttackDamage);
                counterAttackTimer = counterAttackCooldown;
            }
        }
    }
    
    #region Health System Overrides
    
    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        
        // Notify game manager that the castle was destroyed
        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.OnKeepDestroyed();
        }
    }
    
    #endregion
    
    #region Attack System Overrides
    
    /// <summary>
    /// Castle can only counter-attack, not actively seek targets
    /// </summary>
    /// <returns>False - castle doesn't actively attack</returns>
    public override bool CanAttack()
    {
        return false; // Castle doesn't actively attack enemies
    }
    
    #endregion
    
    #region Placement System Overrides
    
    /// <summary>
    /// Castle cannot be placed by the player
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="normal">Surface normal</param>
    /// <returns>False - castle cannot be placed</returns>
    public override bool CanBePlacedAt(Vector3 position, Vector3 normal)
    {
        // Castle is static and cannot be placed by the player
        return false;
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Get counter-attack damage
    /// </summary>
    /// <returns>Counter-attack damage</returns>
    public float GetCounterAttackDamage()
    {
        return counterAttackDamage;
    }
    
    #endregion
}