using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Enhanced enemy AI that moves along roads and targets blocking buildings
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed of the enemy")]
    public float moveSpeed = 2f;
    
    [Header("Combat Settings")]
    [Tooltip("Attack range for targeting buildings")]
    public float attackRange = 1.5f;
    
    [Tooltip("Attack damage to buildings")]
    public float attackDamage = 10f;
    
    [Tooltip("Attack speed (attacks per second)")]
    public float attackSpeed = 0.5f;
    
    [Header("Targeting Settings")]
    [Tooltip("Whether this enemy targets blocking buildings")]
    public bool targetsBuildings = true;
    
    [Tooltip("Whether this enemy continues to the end point even if buildings are present")]
    public bool continuesToEndpoint = true;
    
    // Internal variables
    private List<Vector3> path = new List<Vector3>();
    private int currentPathIndex = 0;
    private BuildingBase targetBuilding;
    private float attackCooldown = 0f;
    private bool hasReachedEndpoint = false;
    
    // Components
    private Enemy enemy;
    
    void Start()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("EnemyAI requires an Enemy component!", this);
            enabled = false;
            return;
        }
        
        // Find a path when spawned
        FindPath();
    }
    
    void Update()
    {
        if (path == null || path.Count == 0)
            return;
            
        // Update attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        
        // Handle targeting and attacking buildings
        if (targetsBuildings)
        {
            HandleBuildingTargeting();
        }
        
        // Move along path if not attacking a building
        if (targetBuilding == null || !targetBuilding.IsBlockingMovement())
        {
            MoveAlongPath();
        }
    }
    
    /// <summary>
    /// Find a path from current position to the endpoint
    /// </summary>
    private void FindPath()
    {
        // For now, we'll use a simple approach to find a path
        // In a full implementation, this would connect to your level's start and end points
        
        // Try to find the grid manager
        if (GridManager.Instance == null)
        {
            Debug.LogWarning("GridManager not found! Falling back to simple movement.", this);
            CreateSimplePath();
            return;
        }
        
        // Try to find road cells
        var roadCells = GridPathfinder.Instance.GetRoadCells();
        if (roadCells.Count < 2)
        {
            Debug.LogWarning("Not enough road cells found! Falling back to simple movement.", this);
            CreateSimplePath();
            return;
        }
        
        // Create a simple path along some road cells
        path.Clear();
        for (int i = 0; i < Mathf.Min(5, roadCells.Count); i++)
        {
            Vector3 worldPos = GridManager.Instance.GridToWorld(roadCells[i]);
            // Offset Y to be above ground
            worldPos.y = 0.5f;
            path.Add(worldPos);
        }
        
        // Add endpoint (castle or similar)
        Vector3 endPoint = path[path.Count - 1];
        endPoint.z += 3f; // Extend path a bit
        endPoint.y = 0.5f;
        path.Add(endPoint);
    }
    
    /// <summary>
    /// Create a simple path for fallback behavior
    /// </summary>
    private void CreateSimplePath()
    {
        path.Clear();
        
        // Simple path for testing
        Vector3 startPos = transform.position;
        startPos.y = 0.5f;
        path.Add(startPos);
        
        Vector3 midPoint = startPos;
        midPoint.z += 5f;
        midPoint.y = 0.5f;
        path.Add(midPoint);
        
        Vector3 endPoint = midPoint;
        endPoint.z += 5f;
        endPoint.y = 0.5f;
        path.Add(endPoint);
    }
    
    /// <summary>
    /// Move the enemy along the path
    /// </summary>
    private void MoveAlongPath()
    {
        if (hasReachedEndpoint || path.Count == 0 || currentPathIndex >= path.Count)
            return;
            
        // Get current target position
        Vector3 targetPos = path[currentPathIndex];
        
        // Move towards target
        Vector3 direction = (targetPos - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Rotate to face movement direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Check if we've reached the current waypoint
        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            currentPathIndex++;
            
            // Check if we've reached the end of the path
            if (currentPathIndex >= path.Count)
            {
                hasReachedEndpoint = true;
                OnReachedEndpoint();
            }
        }
    }
    
    /// <summary>
    /// Handle targeting and attacking buildings
    /// </summary>
    private void HandleBuildingTargeting()
    {
        // If we're already attacking a building, check if it still exists and is blocking
        if (targetBuilding != null)
        {
            // Check if building still exists and is blocking
            if (targetBuilding.IsDestroyed() || !targetBuilding.IsBlockingMovement())
            {
                targetBuilding = null;
            }
            else
            {
                // Attack the building
                AttackBuilding(targetBuilding);
                return;
            }
        }
        
        // Look for nearby blocking buildings
        targetBuilding = FindBlockingBuilding();
        
        if (targetBuilding != null)
        {
            // Attack the building
            AttackBuilding(targetBuilding);
        }
    }
    
    /// <summary>
    /// Find a building that's blocking the path
    /// </summary>
    /// <returns>Blocking building, or null if none found</returns>
    private BuildingBase FindBlockingBuilding()
    {
        // Check for buildings in attack range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var collider in hitColliders)
        {
            BuildingBase building = collider.GetComponent<BuildingBase>();
            if (building != null && building.IsBlockingMovement() && !building.IsDestroyed())
            {
                // Check if building is on our path
                if (IsBuildingOnPath(building))
                {
                    return building;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Check if a building is on our path
    /// </summary>
    /// <param name="building">Building to check</param>
    /// <returns>True if building is on path</returns>
    private bool IsBuildingOnPath(BuildingBase building)
    {
        // Simple approximation - check if building is near our current path segment
        if (path.Count == 0 || currentPathIndex >= path.Count)
            return false;
            
        Vector3 currentTarget = path[Mathf.Min(currentPathIndex, path.Count - 1)];
        Vector3 toBuilding = building.transform.position - transform.position;
        Vector3 toTarget = currentTarget - transform.position;
        
        // Check if building is in front of us and close to our path
        float dot = Vector3.Dot(toBuilding.normalized, toTarget.normalized);
        float distanceToBuilding = toBuilding.magnitude;
        
        return dot > 0.5f && distanceToBuilding < attackRange + 2f;
    }
    
    /// <summary>
    /// Attack a building
    /// </summary>
    /// <param name="building">Building to attack</param>
    private void AttackBuilding(BuildingBase building)
    {
        if (building == null || attackCooldown > 0)
            return;
            
        // Apply damage to building
        building.TakeDamage(attackDamage);
        
        // Reset attack cooldown
        attackCooldown = 1f / attackSpeed;
    }
    
    /// <summary>
    /// Called when the enemy reaches the endpoint
    /// </summary>
    private void OnReachedEndpoint()
    {
        // Notify game manager that enemy reached the keep
        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.OnEnemyReachedKeep();
        }
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Draw gizmos for debugging
    /// </summary>
    void OnDrawGizmos()
    {
        // Draw path
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
            
            // Draw waypoints
            Gizmos.color = Color.magenta;
            foreach (var point in path)
            {
                Gizmos.DrawSphere(point, 0.2f);
            }
        }
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}