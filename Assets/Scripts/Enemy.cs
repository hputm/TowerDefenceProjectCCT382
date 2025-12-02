using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Stats")]
    public float speed = 10f;
    public float maxHealth = 100f;
    public float currentHealth;
    
    private Transform target;
    private int wavepointIndex = 0;
    
    // Event to notify when health changes
    public System.Action<float> onHealthChanged;
    public System.Action onEnemyDeath;
    
    void Start()
    {
        target = Waypoints.points[0];
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 0.2f)
        {
            GetNextWaypoint();
        }
    }
    
    void GetNextWaypoint()
    {
        if (wavepointIndex >= Waypoints.points.Length - 1)
        {
            // Notify game manager that enemy reached the keep
            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.OnEnemyReachedKeep();
            }
            
            // Destroy the enemy
            Destroy(gameObject);
            return;
        }

        wavepointIndex++;
        target = Waypoints.points[wavepointIndex];
    }
    
    #region IDamageable Implementation
    
    /// <summary>
    /// Apply damage to the enemy
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
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
        
        // Notify resource manager about gold drop
        var resourceManager = ResourceManager.Instance;
        if (resourceManager != null)
        {
            // Drop some gold when enemy dies (this should be configurable in a full implementation)
            resourceManager.AddGold(10);
        }
        
        // Destroy the enemy
        Destroy(gameObject);
    }
}