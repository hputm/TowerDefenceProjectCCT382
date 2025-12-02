using UnityEngine;

/// <summary>
/// Projectile class for handling projectile movement and damage delivery
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Target to move towards")]
    private Transform target;
    
    [Tooltip("Damage to apply on hit")]
    private float damage = 10f;
    
    [Tooltip("Speed of the projectile")]
    public float speed = 20f;
    
    [Tooltip("Lifetime of the projectile before it's automatically destroyed")]
    public float lifetime = 5f;
    
    private float lifetimeTimer = 0f;
    
    void Update()
    {
        lifetimeTimer += Time.deltaTime;
        
        // Automatically destroy if lifetime exceeded
        if (lifetimeTimer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }
        
        // Move towards target if it exists
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            
            // Rotate to face movement direction
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            
            // Check if we've reached the target
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < 0.5f)
            {
                HitTarget();
            }
        }
        else
        {
            // If target is destroyed, just move forward in the last known direction
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Initialize the projectile with target and damage values
    /// </summary>
    /// <param name="target">Target to move towards</param>
    /// <param name="damage">Damage to apply on hit</param>
    /// <param name="speed">Speed of the projectile</param>
    public void Initialize(Transform target, float damage, float speed = 20f)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
        
        // Rotate to face target
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    
    /// <summary>
    /// Called when the projectile hits its target
    /// </summary>
    private void HitTarget()
    {
        // Apply damage to target
        if (target != null)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            else
            {
                // Fallback to direct Enemy component if IDamageable is not implemented
                Enemy enemy = target.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
        
        // Destroy projectile
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Called when the projectile collides with something
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // If we hit an enemy, apply damage
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            else
            {
                // Fallback to direct Enemy component if IDamageable is not implemented
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
    }
}