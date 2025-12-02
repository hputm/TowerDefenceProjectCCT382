using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Finds paths for enemies along road grid cells
/// </summary>
public class GridPathfinder : MonoBehaviour
{
    private static GridPathfinder _instance;
    public static GridPathfinder Instance 
    { 
        get 
        { 
            if (_instance == null)
            {
                _instance = FindObjectOfType<GridPathfinder>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GridPathfinder");
                    _instance = singletonObject.AddComponent<GridPathfinder>();
                }
            }
            return _instance; 
        } 
    }
    
    private void Awake()
    {
        // Singleton pattern implementation
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Find a path from start to end position using road grid cells
    /// </summary>
    /// <param name="startPos">Start world position</param>
    /// <param name="endPos">End world position</param>
    /// <returns>List of waypoints along the path</returns>
    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        // Convert world positions to grid positions
        Vector2Int startGridPos = GridManager.Instance.WorldToGrid(startPos);
        Vector2Int endGridPos = GridManager.Instance.WorldToGrid(endPos);
        
        // Use BFS to find path
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        
        queue.Enqueue(startGridPos);
        visited.Add(startGridPos);
        cameFrom[startGridPos] = startGridPos;
        
        Vector2Int[] directions = {
            new Vector2Int(1, 0),  // Right
            new Vector2Int(-1, 0), // Left
            new Vector2Int(0, 1),  // Up
            new Vector2Int(0, -1)  // Down
        };
        
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            
            // Check if we reached the end
            if (current == endGridPos)
            {
                // Reconstruct path
                return ReconstructPath(cameFrom, current);
            }
            
            // Check neighbors
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = current + direction;
                
                // Skip if already visited
                if (visited.Contains(neighbor))
                    continue;
                
                // Check if neighbor is a valid road cell
                GridCell cell = GridManager.Instance.GetCell(neighbor);
                if (cell != null && (cell.GetCellType() == GridCell.GridCellType.Road || 
                                   cell.GetCellType() == GridCell.GridCellType.RoadEnd))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }
        
        // No path found
        return new List<Vector3>();
    }
    
    /// <summary>
    /// Reconstruct path from cameFrom dictionary
    /// </summary>
    /// <param name="cameFrom">Dictionary of positions and where they came from</param>
    /// <param name="current">Current position</param>
    /// <returns>List of waypoints</returns>
    private List<Vector3> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector3> path = new List<Vector3>();
        Vector2Int pos = current;
        
        // Trace back from end to start
        while (cameFrom.ContainsKey(pos) && cameFrom[pos] != pos)
        {
            // Convert grid position to world position
            Vector3 worldPos = GridManager.Instance.GridToWorld(pos);
            path.Add(worldPos);
            pos = cameFrom[pos];
        }
        
        // Add start position
        Vector3 startPos = GridManager.Instance.GridToWorld(pos);
        path.Add(startPos);
        
        // Reverse path to go from start to end
        path.Reverse();
        return path;
    }
    
    /// <summary>
    /// Get all road cells in the grid
    /// </summary>
    /// <returns>List of road cell positions</returns>
    public List<Vector2Int> GetRoadCells()
    {
        List<Vector2Int> roadCells = new List<Vector2Int>();
        
        foreach (var cell in GridManager.Instance.GetAllCells())
        {
            if (cell != null && 
                (cell.GetCellType() == GridCell.GridCellType.Road || 
                 cell.GetCellType() == GridCell.GridCellType.RoadEnd))
            {
                // Find the position of this cell in the grid
                foreach (var kvp in GetGridCellPositions())
                {
                    if (kvp.Value == cell)
                    {
                        roadCells.Add(kvp.Key);
                        break;
                    }
                }
            }
        }
        
        return roadCells;
    }
    
    /// <summary>
    /// Get all grid cell positions
    /// </summary>
    /// <returns>Dictionary of grid positions and their cells</returns>
    private Dictionary<Vector2Int, GridCell> GetGridCellPositions()
    {
        // This is a simplified approach - in a real implementation, 
        // GridManager should provide this functionality directly
        Dictionary<Vector2Int, GridCell> positions = new Dictionary<Vector2Int, GridCell>();
        
        foreach (var cell in GridManager.Instance.GetAllCells())
        {
            if (cell != null)
            {
                // Approximate grid position from world position
                Vector2Int gridPos = GridManager.Instance.WorldToGrid(cell.transform.position);
                positions[gridPos] = cell;
            }
        }
        
        return positions;
    }
}