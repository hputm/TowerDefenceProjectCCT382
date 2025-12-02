using UnityEngine;

/// <summary>
/// Handles the placement of buildings in the game world.
/// Manages placement constraints, costs, and instantiation of buildings.
/// </summary>
public class BuildingPlacementSystem : MonoBehaviour
{
    public enum BuildingType
    {
        ArrowTower,
        RoadBlock,
        DefenseBuilding
    }
    
    [System.Serializable]
    public struct BuildingPrefab
    {
        public BuildingType type;
        public GameObject prefab;
    }
    
    [Header("Placement Settings")]
    [Tooltip("Layers that are considered valid for building placement")]
    public LayerMask placementLayerMask = ~0; // Default to all layers
    
    [Tooltip("Maximum distance for placement from camera")]
    public float maxPlacementDistance = 100f;
    
    [Header("Visual Feedback")]
    [Tooltip("Prefab for placement preview visualization")]
    public GameObject placementPreviewPrefab;
    
    [Tooltip("Color when placement is valid")]
    public Color validPlacementColor = Color.green;
    
    [Tooltip("Color when placement is invalid")]
    public Color invalidPlacementColor = Color.red;
    
    [Tooltip("The type of building to place / 要放置的建筑类型")]
    public BuildingType selectedBuildingType = BuildingType.ArrowTower;
    
    private GameObject placementPreview;
    private BuildingBase currentBuildingToPlace;
    private bool isPlacementMode = false;
    private GridCell currentHighlightedCell;
    
    // Event to notify when a building is placed
    public System.Action<BuildingBase> onBuildingPlaced;
    
    private static BuildingPlacementSystem _instance;
    public static BuildingPlacementSystem Instance 
    { 
        get 
        { 
            if (_instance == null)
            {
                _instance = FindObjectOfType<BuildingPlacementSystem>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("BuildingPlacementSystem");
                    _instance = singletonObject.AddComponent<BuildingPlacementSystem>();
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
    
    private void Update()
    {
        if (isPlacementMode && currentBuildingToPlace != null)
        {
            UpdatePlacementPreview();
        }
    }
    
    #region Placement Mode Management
    
    /// <summary>
    /// Enter placement mode with a specific building type
    /// </summary>
    /// <param name="buildingPrefab">Prefab of the building to place</param>
    public void EnterPlacementMode(BuildingBase buildingPrefab)
    {
        if (buildingPrefab == null) return;
        
        // Exit current placement mode if active
        if (isPlacementMode)
        {
            ExitPlacementMode();
        }
        
        currentBuildingToPlace = buildingPrefab;
        isPlacementMode = true;
        
        // Create placement preview
        CreatePlacementPreview();
        
        Debug.Log($"Entered placement mode for {buildingPrefab.GetBuildingType()}", this);
    }
    
    /// <summary>
    /// Exit placement mode
    /// </summary>
    public void ExitPlacementMode()
    {
        isPlacementMode = false;
        currentBuildingToPlace = null;
        currentHighlightedCell = null;
        
        // Destroy placement preview
        if (placementPreview != null)
        {
            Destroy(placementPreview);
            placementPreview = null;
        }
        
        Debug.Log("Exited placement mode", this);
    }
    
    /// <summary>
    /// Check if the system is currently in placement mode
    /// </summary>
    /// <returns>True if in placement mode</returns>
    public bool IsInPlacementMode()
    {
        return isPlacementMode;
    }
    
    #endregion
    
    #region Placement Preview
    
    /// <summary>
    /// Create the placement preview object
    /// </summary>
    private void CreatePlacementPreview()
    {
        if (currentBuildingToPlace == null) return;
        
        // Destroy existing preview if any
        if (placementPreview != null)
        {
            Destroy(placementPreview);
        }
        
        // Create new preview
        placementPreview = new GameObject("PlacementPreview");
        
        // Add visual components to preview
        // This would typically involve copying the visual components from the building prefab
        // For now, we'll just add a basic renderer for demonstration
        var renderer = placementPreview.AddComponent<MeshRenderer>();
        var filter = placementPreview.AddComponent<MeshFilter>();
        
        // Copy mesh and material from the building prefab if available
        var buildingRenderer = currentBuildingToPlace.GetComponent<Renderer>();
        var buildingFilter = currentBuildingToPlace.GetComponent<MeshFilter>();
        
        if (buildingFilter != null)
        {
            filter.mesh = buildingFilter.sharedMesh;
        }
        
        if (buildingRenderer != null)
        {
            renderer.material = new Material(buildingRenderer.sharedMaterial);
        }
        
        // Make it transparent
        if (renderer.material != null)
        {
            renderer.material.color = new Color(
                renderer.material.color.r,
                renderer.material.color.g,
                renderer.material.color.b,
                0.5f);
        }
    }
    
    /// <summary>
    /// Update the placement preview position and validity
    /// </summary>
    private void UpdatePlacementPreview()
    {
        if (placementPreview == null || currentBuildingToPlace == null) return;
        
        // Get placement position
        Vector3 placementPosition;
        Vector3 surfaceNormal;
        GridCell targetCell;
        
        if (GetPlacementPosition(out placementPosition, out surfaceNormal, out targetCell))
        {
            // Update highlighted cell
            if (currentHighlightedCell != targetCell)
            {
                currentHighlightedCell = targetCell;
            }
            
            // Snap to grid cell center
            if (targetCell != null)
            {
                Vector2Int gridPos = GridManager.Instance.WorldToGrid(placementPosition);
                placementPosition = GridManager.Instance.GridToWorld(gridPos);
            }
            
            // Update preview position
            placementPreview.transform.position = placementPosition;
            placementPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
            
            // Check if placement is valid
            bool isValid = targetCell != null && targetCell.CanPlaceBuilding(currentBuildingToPlace);
            
            // Update preview appearance
            UpdatePreviewAppearance(isValid);
        }
    }
    
    /// <summary>
    /// Update the visual appearance of the placement preview
    /// </summary>
    /// <param name="isValid">Whether the current placement is valid</param>
    private void UpdatePreviewAppearance(bool isValid)
    {
        if (placementPreview == null) return;
        
        var renderer = placementPreview.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            // Update color based on validity
            Color targetColor = isValid ? validPlacementColor : invalidPlacementColor;
            targetColor.a = 0.5f; // Keep transparency
            
            renderer.material.color = targetColor;
        }
    }
    
    #endregion
    
    #region Placement Position Calculation
    
    /// <summary>
    /// Calculate the placement position based on mouse/pointer position
    /// </summary>
    /// <param name="position">Output placement position</param>
    /// <param name="normal">Output surface normal</param>
    /// <param name="cell">Output grid cell</param>
    /// <returns>True if a valid placement position was found</returns>
    private bool GetPlacementPosition(out Vector3 position, out Vector3 normal, out GridCell cell)
    {
        position = Vector3.zero;
        normal = Vector3.up;
        cell = null;
        
        // Raycast from camera to find placement position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxPlacementDistance, placementLayerMask))
        {
            position = hit.point;
            normal = hit.normal;
            
            // Get the grid cell at this position
            cell = GridManager.Instance.GetCell(position);
            return true;
        }
        
        return false;
    }
    
    #endregion
    
    #region Building Placement
    
    /// <summary>
    /// Attempt to place the current building at the cursor position
    /// </summary>
    /// <returns>True if placement was successful</returns>
    public bool PlaceCurrentBuilding()
    {
        if (!isPlacementMode || currentBuildingToPlace == null) return false;
        
        Vector3 placementPosition;
        Vector3 surfaceNormal;
        GridCell targetCell;
        
        if (GetPlacementPosition(out placementPosition, out surfaceNormal, out targetCell))
        {
            // Snap to grid cell center
            if (targetCell != null)
            {
                Vector2Int gridPos = GridManager.Instance.WorldToGrid(placementPosition);
                placementPosition = GridManager.Instance.GridToWorld(gridPos);
            }
            
            // Check if placement is valid
            if (targetCell != null && targetCell.CanPlaceBuilding(currentBuildingToPlace))
            {
                // Check if player can afford the building
                if (CanAffordBuilding(currentBuildingToPlace))
                {
                    // Place the building
                    BuildingBase placedBuilding = PlaceBuilding(currentBuildingToPlace, placementPosition, surfaceNormal);
                    
                    // Register the building with the grid cell
                    if (placedBuilding != null && targetCell != null)
                    {
                        targetCell.PlaceBuilding(placedBuilding);
                        return true;
                    }
                }
                else
                {
                    Debug.Log("Cannot afford to place this building", this);
                }
            }
            else
            {
                Debug.Log("Invalid placement position", this);
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Place a building at a specific position
    /// </summary>
    /// <param name="buildingPrefab">Prefab of the building to place</param>
    /// <param name="position">Position to place the building</param>
    /// <param name="rotation">Rotation for the building</param>
    /// <returns>The instantiated building</returns>
    public BuildingBase PlaceBuilding(BuildingBase buildingPrefab, Vector3 position, Quaternion rotation)
    {
        if (buildingPrefab == null) return null;
        
        // Get the grid cell at this position
        GridCell cell = GridManager.Instance.GetCell(position);
        
        // Check if placement is valid
        if (cell == null || !cell.CanPlaceBuilding(buildingPrefab))
        {
            Debug.LogWarning($"Cannot place {buildingPrefab.GetBuildingType()} at {position}", this);
            return null;
        }
        
        // Check if player can afford the building
        if (!CanAffordBuilding(buildingPrefab))
        {
            Debug.LogWarning($"Cannot afford to place {buildingPrefab.GetBuildingType()}", this);
            return null;
        }
        
        // Spend resources
        SpendBuildingResources(buildingPrefab);
        
        // Instantiate the building
        BuildingBase newBuilding = Instantiate(buildingPrefab, position, rotation);
        
        // Register the building with the grid cell
        cell.PlaceBuilding(newBuilding);
        
        // Notify listeners
        onBuildingPlaced?.Invoke(newBuilding);
        
        Debug.Log($"Placed {newBuilding.GetBuildingType()} at {position}", this);
        return newBuilding;
    }
    
    /// <summary>
    /// Place a building at a specific position with surface alignment
    /// </summary>
    /// <param name="buildingPrefab">Prefab of the building to place</param>
    /// <param name="position">Position to place the building</param>
    /// <param name="surfaceNormal">Normal of the surface to align with</param>
    /// <returns>The instantiated building</returns>
    public BuildingBase PlaceBuilding(BuildingBase buildingPrefab, Vector3 position, Vector3 surfaceNormal)
    {
        if (buildingPrefab == null) return null;
        
        // Calculate rotation to align with surface
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
        
        return PlaceBuilding(buildingPrefab, position, rotation);
    }
    
    #endregion
    
    #region Resource Management
    
    /// <summary>
    /// Check if the player can afford to place a building
    /// </summary>
    /// <param name="building">Building to check</param>
    /// <returns>True if player has enough resources</returns>
    private bool CanAffordBuilding(BuildingBase building)
    {
        // Integrate with ResourceManager when available
        // For now, allow all placements
        return true;
    }
    
    /// <summary>
    /// Spend resources required for placing a building
    /// </summary>
    /// <param name="building">Building being placed</param>
    private void SpendBuildingResources(BuildingBase building)
    {
        // Integrate with ResourceManager when available
        // This is where you would deduct gold/other resources
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Get the building that is currently being placed
    /// </summary>
    /// <returns>Current building to place, or null if not in placement mode</returns>
    public BuildingBase GetCurrentBuildingToPlace()
    {
        return isPlacementMode ? currentBuildingToPlace : null;
    }
    
    /// <summary>
    /// Select which building type to place
    /// 选择要放置的建筑类型
    /// </summary>
    public void SelectBuildingToPlace(BuildingType type)
    {
        selectedBuildingType = type;
        
        // Update ghost preview
        if (ghostPreview != null)
        {
            Destroy(ghostPreview);
        }
        
        // Create new ghost preview
        GameObject prefabToUse = GetPrefabForType(type);
        if (prefabToUse != null)
        {
            ghostPreview = Instantiate(prefabToUse);
            ghostPreview.SetActive(false);
            
            // Make it transparent
            Renderer[] renderers = ghostPreview.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].color = new Color(materials[i].color.r, 
                                                materials[i].color.g, 
                                                materials[i].color.b, 0.5f);
                }
                renderer.materials = materials;
            }
        }
    }
    
    /// <summary>
    /// Get prefab for building type
    /// 根据建筑类型获取预制体
    /// </summary>
    GameObject GetPrefabForType(BuildingType type)
    {
        foreach (BuildingPrefab bp in buildingPrefabs)
        {
            if (bp.type == type)
            {
                return bp.prefab;
            }
        }
        return null;
    }
}
