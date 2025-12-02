using UnityEngine;

/// <summary>
/// Represents a single cell in the grid-based map system
/// Determines what can be placed in this cell based on its type
/// </summary>
public class GridCell : MonoBehaviour
{
    [Header("Grid Cell Properties")]
    [Tooltip("Type of this grid cell")]
    public GridCellType cellType = GridCellType.Empty;
    
    [Tooltip("Whether this cell is at the end of a road segment")]
    public bool isRoadEnd = false;
    
    [Tooltip("Reference to any building placed in this cell")]
    [HideInInspector] public BuildingBase occupiedBy;
    
    // Events
    public System.Action<GridCellType> onCellTypeChanged;
    public System.Action<BuildingBase> onOccupancyChanged;
    
    public enum GridCellType
    {
        Empty,          // Empty ground - no buildings allowed
        Road,           // Road cell - can place Defense Buildings and Road Blocks
        RoadEnd,        // End of road - can place Arrow Towers, Defense Buildings, and Road Blocks
        Impassable,     // Mountain/water/etc. - no buildings allowed
        Buildable       // Normal buildable area - future use
    }
    
    void Start()
    {
        // If this cell is marked as a road end, update the cell type accordingly
        if (isRoadEnd && cellType == GridCellType.Road)
        {
            cellType = GridCellType.RoadEnd;
            onCellTypeChanged?.Invoke(cellType);
        }
    }
    
    /// <summary>
    /// Check if a building can be placed in this cell
    /// </summary>
    /// <param name="building">The building to check</param>
    /// <returns>True if the building can be placed here</returns>
    public bool CanPlaceBuilding(BuildingBase building)
    {
        if (occupiedBy != null)
        {
            // Cell is already occupied
            return false;
        }
        
        // Check based on building type and cell type
        switch (building.placementType)
        {
            case BuildingBase.PlacementType.RoadEndsOnly:
                // Arrow Towers can only be placed at road ends
                return cellType == GridCellType.RoadEnd;
                
            case BuildingBase.PlacementType.RoadOnly:
                // Defense Buildings and Road Blocks can be placed on roads or road ends
                return cellType == GridCellType.Road || cellType == GridCellType.RoadEnd;
                
            case BuildingBase.PlacementType.Anywhere:
                // For future use - buildings that can be placed anywhere
                return cellType != GridCellType.Impassable;
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Place a building in this cell
    /// </summary>
    /// <param name="building">The building to place</param>
    /// <returns>True if placement was successful</returns>
    public bool PlaceBuilding(BuildingBase building)
    {
        if (!CanPlaceBuilding(building))
        {
            return false;
        }
        
        occupiedBy = building;
        onOccupancyChanged?.Invoke(building);
        return true;
    }
    
    /// <summary>
    /// Remove a building from this cell
    /// </summary>
    public void RemoveBuilding()
    {
        occupiedBy = null;
        onOccupancyChanged?.Invoke(null);
    }
    
    /// <summary>
    /// Get the type of this cell
    /// </summary>
    /// <returns>GridCellType of this cell</returns>
    public GridCellType GetCellType()
    {
        return cellType;
    }
    
    /// <summary>
    /// Set the type of this cell
    /// </summary>
    /// <param name="type">New cell type</param>
    public void SetCellType(GridCellType type)
    {
        GridCellType oldType = cellType;
        cellType = type;
        
        // If setting to road end, also update the flag
        if (type == GridCellType.RoadEnd)
        {
            isRoadEnd = true;
        }
        
        // Notify listeners if type changed
        if (oldType != cellType)
        {
            onCellTypeChanged?.Invoke(cellType);
        }
    }
    
    /// <summary>
    /// Check if this cell is occupied by a building
    /// </summary>
    /// <returns>True if occupied</returns>
    public bool IsOccupied()
    {
        return occupiedBy != null;
    }
    
    /// <summary>
    /// Get the building occupying this cell
    /// </summary>
    /// <returns>Occupying building or null if none</returns>
    public BuildingBase GetOccupyingBuilding()
    {
        return occupiedBy;
    }
    
    /// <summary>
    /// Check if this cell is at a road end
    /// </summary>
    /// <returns>True if this is a road end cell</returns>
    public bool IsRoadEnd()
    {
        return isRoadEnd || cellType == GridCellType.RoadEnd;
    }
}