using UnityEngine;

/// <summary>
/// Interface for objects that can take damage and die
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Apply damage to this object
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    void TakeDamage(float damage);
    
    /// <summary>
    /// Heal this object
    /// </summary>
    /// <param name="amount">Amount of health to restore</param>
    void Heal(float amount);
    
    /// <summary>
    /// Get the current health of this object
    /// </summary>
    /// <returns>Current health</returns>
    float GetCurrentHealth();
    
    /// <summary>
    /// Get the maximum health of this object
    /// </summary>
    /// <returns>Maximum health</returns>
    float GetMaxHealth();
    
    /// <summary>
    /// Check if this object is dead/destroyed
    /// </summary>
    /// <returns>True if health is zero or below</returns>
    bool IsDead();
}