using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the grid-based map system
/// Keeps track of all grid cells and their properties
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Size of each grid cell")]
    public float cellSize = 1.0f;
    
    [Tooltip("Grid dimensions")]
    public Vector2Int gridSize = new Vector2Int(20, 20);
    
    [Header("Cell Prefabs")]
    [Tooltip("Prefab for road cells")]
    public GameObject roadCellPrefab;
    
    [Tooltip("Prefab for road end cells")]
    public GameObject roadEndCellPrefab;
    
    [Tooltip("Prefab for empty cells")]
    public GameObject emptyCellPrefab;
    
    [Tooltip("Prefab for impassable cells")]
    public GameObject impassableCellPrefab;
    
    // Dictionary to store all grid cells for fast lookup
    private Dictionary<Vector2Int, GridCell> gridCells = new Dictionary<Vector2Int, GridCell>();
    
    // Singleton instance
    private static GridManager _instance;
    public static GridManager Instance 
    { 
        get 
        { 
            if (_instance == null)
            {
                _instance = FindObjectOfType<GridManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GridManager");
                    _instance = singletonObject.AddComponent<GridManager>();
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
    /// Initialize the grid with default cells
    /// </summary>
    public void InitializeGrid()
    {
        // Clear existing cells
        foreach (var cell in gridCells.Values)
        {
            if (cell != null)
                Destroy(cell.gameObject);
        }
        gridCells.Clear();
        
        // Create grid cells
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                CreateCell(gridPos);
            }
        }
    }
    
    /// <summary>
    /// Create a cell at the specified grid position
    /// </summary>
    /// <param name="gridPos">Grid position</param>
    private void CreateCell(Vector2Int gridPos)
    {
        // Convert grid position to world position
        Vector3 worldPos = GridToWorld(gridPos);
        
        // Determine which prefab to use (for demo purposes, we'll create a mix)
        GameObject cellPrefab = emptyCellPrefab;
        GridCell.GridCellType cellType = GridCell.GridCellType.Empty;
        
        // Simple pattern for demo - you would load this from a map file in a real implementation
        if ((gridPos.x == 5 || gridPos.x == 15) && gridPos.y >= 5 && gridPos.y <= 15)
        {
            // Vertical roads
            cellPrefab = roadCellPrefab;
            cellType = GridCell.GridCellType.Road;
        }
        else if (gridPos.x >= 5 && gridPos.x <= 15 && (gridPos.y == 5 || gridPos.y == 15))
        {
            // Horizontal roads
            cellPrefab = roadCellPrefab;
            cellType = GridCell.GridCellType.Road;
        }
        else if ((gridPos.x == 5 || gridPos.x == 15) && (gridPos.y == 5 || gridPos.y == 15))
        {
            // Road intersections (road ends)
            cellPrefab = roadEndCellPrefab;
            cellType = GridCell.GridCellType.RoadEnd;
        }
        else if (gridPos.x <= 2 || gridPos.x >= gridSize.x - 3 || gridPos.y <= 2 || gridPos.y >= gridSize.y - 3)
        {
            // Border cells are impassable
            cellPrefab = impassableCellPrefab;
            cellType = GridCell.GridCellType.Impassable;
        }
        
        // Instantiate the cell
        if (cellPrefab != null)
        {
            GameObject cellObject = Instantiate(cellPrefab, worldPos, Quaternion.identity, transform);
            cellObject.name = $"GridCell_{gridPos.x}_{gridPos.y}";
            
            // Get or add GridCell component
            GridCell gridCell = cellObject.GetComponent<GridCell>();
            if (gridCell == null)
            {
                gridCell = cellObject.AddComponent<GridCell>();
            }
            
            // Set cell properties
            gridCell.SetCellType(cellType);
            
            // Store in dictionary
            gridCells[gridPos] = gridCell;
        }
    }
    
    /// <summary>
    /// Get the grid cell at the specified grid position
    /// </summary>
    /// <param name="gridPos">Grid position</param>
    /// <returns>GridCell at position, or null if not found</returns>
    public GridCell GetCell(Vector2Int gridPos)
    {
        GridCell cell;
        if (gridCells.TryGetValue(gridPos, out cell))
        {
            return cell;
        }
        return null;
    }
    
    /// <summary>
    /// Get the grid cell at the specified world position
    /// </summary>
    /// <param name="worldPos">World position</param>
    /// <returns>GridCell at position, or null if not found</returns>
    public GridCell GetCell(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GetCell(gridPos);
    }
    
    /// <summary>
    /// Convert world position to grid position
    /// </summary>
    /// <param name="worldPos">World position</param>
    /// <returns>Grid position</returns>
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.z / cellSize); // Using Z because Unity's ground plane is XZ
        return new Vector2Int(x, y);
    }
    
    /// <summary>
    /// Convert grid position to world position
    /// </summary>
    /// <param name="gridPos">Grid position</param>
    /// <returns>World position</returns>
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        float x = gridPos.x * cellSize + cellSize / 2f;
        float z = gridPos.y * cellSize + cellSize / 2f; // Using Z because Unity's ground plane is XZ
        return new Vector3(x, 0, z);
    }
    
    /// <summary>
    /// Get all grid cells
    /// </summary>
    /// <returns>Collection of all grid cells</returns>
    public ICollection<GridCell> GetAllCells()
    {
        return gridCells.Values;
    }
    
    /// <summary>
    /// Get neighboring cells for a given grid position
    /// </summary>
    /// <param name="gridPos">Grid position</param>
    /// <returns>List of neighboring cells</returns>
    public List<GridCell> GetNeighbors(Vector2Int gridPos)
    {
        List<GridCell> neighbors = new List<GridCell>();
        
        // Check the four adjacent cells (up, down, left, right)
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0)    // Right
        };
        
        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPos = gridPos + direction;
            GridCell neighbor = GetCell(neighborPos);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }
        
        return neighbors;
    }
    
    /// <summary>
    /// Check if a position is valid for placing a building
    /// </summary>
    /// <param name="worldPos">World position to check</param>
    /// <param name="building">Building to place</param>
    /// <returns>True if position is valid</returns>
    public bool IsValidPlacementPosition(Vector3 worldPos, BuildingBase building)
    {
        GridCell cell = GetCell(worldPos);
        if (cell == null)
        {
            return false;
        }
        
        return cell.CanPlaceBuilding(building);
    }
}