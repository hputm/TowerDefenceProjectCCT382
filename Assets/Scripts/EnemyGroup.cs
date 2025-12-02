using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles group behavior for enemies that move together as a squad
/// </summary>
public class EnemyGroup : MonoBehaviour
{
    [Header("Group Settings")]
    [Tooltip("List of enemies in this group")]
    public List<EnhancedEnemy> groupMembers = new List<EnhancedEnemy>();
    
    [Tooltip("Distance at which group members stay together")]
    public float cohesionDistance = 2.0f;
    
    [Tooltip("Speed at which group members move toward formation positions")]
    public float formationSpeed = 1.0f;
    
    [Tooltip("Offset distance between group members")]
    public float memberSpacing = 1.5f;
    
    // Formation positions relative to group center
    private List<Vector3> formationOffsets = new List<Vector3>();
    
    // Leader of the group (first member)
    private EnhancedEnemy groupLeader;
    
    void Start()
    {
        InitializeGroup();
    }
    
    void Update()
    {
        if (groupMembers.Count == 0) return;
        
        // Update formation positions
        UpdateFormationPositions();
        
        // Apply group cohesion
        ApplyGroupCohesion();
    }
    
    /// <summary>
    /// Initialize the group and set up formation positions
    /// </summary>
    private void InitializeGroup()
    {
        // Remove any null members
        groupMembers.RemoveAll(member => member == null);
        
        if (groupMembers.Count == 0) return;
        
        // Set the first member as the leader
        groupLeader = groupMembers[0];
        
        // Create formation offsets
        CreateFormationOffsets();
        
        // Assign group references to members
        foreach (var member in groupMembers)
        {
            var groupComponent = member.gameObject.AddComponent<GroupMember>();
            groupComponent.group = this;
        }
    }
    
    /// <summary>
    /// Create formation positions relative to group center
    /// </summary>
    private void CreateFormationOffsets()
    {
        formationOffsets.Clear();
        
        // Simple line formation for now
        for (int i = 0; i < groupMembers.Count; i++)
        {
            // Position members in a line behind the leader
            Vector3 offset = new Vector3(0, 0, -i * memberSpacing);
            formationOffsets.Add(offset);
        }
    }
    
    /// <summary>
    /// Update formation positions based on leader's position
    /// </summary>
    private void UpdateFormationPositions()
    {
        if (groupLeader == null || formationOffsets.Count != groupMembers.Count) return;
        
        // Get leader's position and forward direction
        Vector3 leaderPosition = groupLeader.transform.position;
        Vector3 leaderForward = groupLeader.transform.forward;
        Vector3 leaderRight = groupLeader.transform.right;
        
        // Update each member's target formation position
        for (int i = 0; i < groupMembers.Count; i++)
        {
            if (groupMembers[i] == null) continue;
            
            // Calculate target position in formation
            Vector3 localOffset = formationOffsets[i];
            Vector3 worldOffset = leaderRight * localOffset.x + leaderForward * localOffset.z;
            Vector3 targetPosition = leaderPosition + worldOffset;
            
            // Move member toward target position if too far from formation
            Vector3 memberPosition = groupMembers[i].transform.position;
            float distanceToFormation = Vector3.Distance(memberPosition, targetPosition);
            
            if (distanceToFormation > cohesionDistance * 0.5f)
            {
                Vector3 directionToFormation = (targetPosition - memberPosition).normalized;
                groupMembers[i].transform.position += directionToFormation * formationSpeed * Time.deltaTime;
                
                // Rotate to face movement direction
                if (directionToFormation != Vector3.zero)
                {
                    groupMembers[i].transform.rotation = Quaternion.LookRotation(directionToFormation);
                }
            }
        }
    }
    
    /// <summary>
    /// Apply group cohesion to keep members together
    /// </summary>
    private void ApplyGroupCohesion()
    {
        if (groupMembers.Count <= 1) return;
        
        // Calculate group center
        Vector3 groupCenter = Vector3.zero;
        int aliveMembers = 0;
        
        foreach (var member in groupMembers)
        {
            if (member != null && !member.IsDead())
            {
                groupCenter += member.transform.position;
                aliveMembers++;
            }
        }
        
        if (aliveMembers == 0) return;
        
        groupCenter /= aliveMembers;
        
        // Apply cohesion force to each member
        foreach (var member in groupMembers)
        {
            if (member == null || member.IsDead()) continue;
            
            Vector3 toCenter = groupCenter - member.transform.position;
            float distanceToCenter = toCenter.magnitude;
            
            // Only apply cohesion if member is far from center
            if (distanceToCenter > cohesionDistance)
            {
                // We won't directly move the enemies here as they have their own movement logic
                // Instead, we can influence their pathfinding or target selection
            }
        }
    }
    
    /// <summary>
    /// Add a member to the group
    /// </summary>
    /// <param name="member">Enemy to add to group</param>
    public void AddMember(EnhancedEnemy member)
    {
        if (member != null && !groupMembers.Contains(member))
        {
            groupMembers.Add(member);
            CreateFormationOffsets();
        }
    }
    
    /// <summary>
    /// Remove a member from the group
    /// </summary>
    /// <param name="member">Enemy to remove from group</param>
    public void RemoveMember(EnhancedEnemy member)
    {
        if (groupMembers.Contains(member))
        {
            groupMembers.Remove(member);
            CreateFormationOffsets();
            
            // If leader was removed, assign new leader
            if (groupLeader == member && groupMembers.Count > 0)
            {
                groupLeader = groupMembers[0];
            }
        }
    }
    
    /// <summary>
    /// Check if the group is defeated (all members dead)
    /// </summary>
    /// <returns>True if all members are dead</returns>
    public bool IsGroupDefeated()
    {
        foreach (var member in groupMembers)
        {
            if (member != null && !member.IsDead())
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// Get the number of alive members in the group
    /// </summary>
    /// <returns>Number of alive members</returns>
    public int GetAliveMemberCount()
    {
        int count = 0;
        foreach (var member in groupMembers)
        {
            if (member != null && !member.IsDead())
            {
                count++;
            }
        }
        return count;
    }
    
    /// <summary>
    /// Get the group leader
    /// </summary>
    /// <returns>Group leader</returns>
    public EnhancedEnemy GetLeader()
    {
        return groupLeader;
    }
}

/// <summary>
/// Component to identify group members
/// </summary>
public class GroupMember : MonoBehaviour
{
    [HideInInspector] public EnemyGroup group;
}