# Tower Defense Game Script Manual

This document serves as a comprehensive guide to all scripts in the tower defense game project. It explains the purpose, functionality, and usage of each script to help frontend developers integrate and use them effectively.

## Table of Contents
1. [Core Systems](#core-systems)
2. [Building System](#building-system)
3. [Enemy System](#enemy-system)
4. [UI System](#ui-system)
5. [Resource Management](#resource-management)
6. [Game Management](#game-management)

## Core Systems

### BuildingBase.cs
**Purpose**: Base class for all buildings in the game including towers, road blocks, and defensive structures.

**Key Features**:
- Defines core properties like health, attack damage, and range
- Implements the IDamageable interface
- Provides events for building destruction and health changes
- Handles grid cell placement

**Enums**:
- `PlacementType`: Anywhere, RoadOnly, RoadEndsOnly, WaterOnly, ImpassableOnly

**Public Properties**:
- `buildingType`: Unique identifier for the building type
- `canAttack`: Whether the building can attack enemies
- `maxHealth`: Maximum health of the building
- `currentHealth`: Current health of the building
- `attackDamage`: Damage dealt by the building
- `attackRange`: Range within which the building can attack
- `attackCooldownTime`: Cooldown between attacks
- `isPlayerPlaceable`: Whether players can place this building
- `isStaticBuilding`: Whether this building is part of the level design
- `blocksMovement`: Whether enemies can move through this building
- `placementType`: Where this building can be placed (RoadEndsOnly, RoadOnly, Anywhere, WaterOnly, ImpassableOnly)

**Events**:
- `onBuildingDestroyed`: Triggered when the building is destroyed
- `onHealthChanged`: Triggered when the building's health changes
- `onBuildingSelected`: Triggered when the building is selected by the player

**Usage**:
Attach to any GameObject that represents a building in the game. Inherit from this class to create specialized buildings.

### IDamageable.cs
**Purpose**: Interface that defines objects that can take damage in the game.

**Methods**:
- `TakeDamage(float damage)`: Applies damage to the object
- `Heal(float amount)`: Heals the object
- `Die()`: Destroys the object when health reaches zero

**Usage**:
Implement this interface on any object that should be able to receive damage (enemies, buildings, etc.).

### Projectile.cs
**Purpose**: Handles projectile behavior for ranged attacks.

**Key Features**:
- Moves toward a target
- Deals damage on impact
- Supports different projectile types

**Public Properties**:
- `damage`: Amount of damage the projectile deals
- `speed`: Speed at which the projectile moves
- `target`: Target the projectile is moving toward

**Usage**:
Attach to projectile prefabs. Configure damage and speed values in the inspector.

## Building System

### ArrowTower.cs
**Purpose**: Implementation of the arrow tower building.

**Key Features**:
- Inherits from BuildingBase
- Shoots projectiles at enemies
- Visual representation of the tower

**Usage**:
Attach to arrow tower GameObjects. Configure stats in the BuildingBase component.

### DefenseBuilding.cs
**Purpose**: Implementation of defensive buildings.

**Key Features**:
- Inherits from BuildingBase
- Blocks enemy movement
- High health for defense

**Usage**:
Attach to defensive building GameObjects. Configure stats in the BuildingBase component.

### RoadBlock.cs
**Purpose**: Implementation of road block buildings.

**Key Features**:
- Inherits from BuildingBase
- Blocks enemy movement
- Lower cost than defensive buildings

**Usage**:
Attach to road block GameObjects. Configure stats in the BuildingBase component.

### Castle.cs
**Purpose**: Implementation of the player's castle.

**Key Features**:
- Inherits from BuildingBase
- Static building that cannot be moved
- Game ends if castle health reaches zero

**Usage**:
Place one instance in the scene as the player's base.

### BuildingPlacementSystem.cs
**Purpose**: Manages the placement of buildings by the player.

**Key Features**:
- Ghost preview of buildings during placement
- Grid-based placement restrictions
- Resource cost checking
- Support for multiple building types

**Enums**:
- `BuildingType`: ArrowTower, RoadBlock, DefenseBuilding
- `PlacementType`: RoadEndsOnly, RoadOnly, Anywhere

**Public Methods**:
- `SelectBuildingToPlace(BuildingType type)`: Selects which building type to place
- `PlaceBuilding(Vector3 position)`: Attempts to place a building at the given position
- `CancelPlacement()`: Cancels the current placement operation

**Usage**:
Attach to a GameObject in the scene. Connect UI buttons to the SelectBuildingToPlace method to allow players to select buildings for placement.

## Enemy System

### Enemy.cs
**Purpose**: Base enemy class that all enemies inherit from.

**Key Features**:
- Movement along predefined paths
- Health management
- Death handling

**Events**:
- `onEnemyDeath`: Triggered when the enemy dies
- `onHealthChanged`: Triggered when the enemy's health changes

**Usage**:
Inherit from this class to create specialized enemy types.

### EnhancedEnemy.cs
**Purpose**: Advanced enemy implementation with additional features.

**Key Features**:
- Inherits from Enemy
- Squad behavior support
- Dynamic health scaling
- Gold dropping on death

**Usage**:
Use as a more advanced alternative to the basic Enemy class.

### EnemyAI.cs
**Purpose**: Controls enemy movement and behavior.

**Key Features**:
- Pathfinding along waypoints
- Target seeking
- Movement speed control

**Usage**:
Attach to enemy GameObjects to give them AI behavior.

### EnemySpawner.cs
**Purpose**: Spawns enemies in waves.

**Key Features**:
- Wave-based enemy spawning
- Configurable spawn timing
- Multiple enemy type support

**Public Methods**:
- `StartWave()`: Begins spawning enemies for the current wave
- `SetCurrentWave(int wave)`: Sets the current wave number
- `StopSpawning()`: Stops enemy spawning

**Usage**:
Place one instance in the scene. Configure enemy groups in the inspector.

### EnemyGroup.cs
**Purpose**: Defines groups of enemies to spawn together.

**Key Features**:
- Group-based enemy spawning
- Squad formation support
- Wave-specific configurations

**Usage**:
Configure in the EnemySpawner component to define what enemies appear in each wave.

## UI System

### UIManager.cs
**Purpose**: Central hub for all UI functionality.

**Key Features**:
- Resource display (gold, wood)
- Game state management (menu, pause, game over)
- Tower selection and upgrade panels
- Wave and lives display

**Public Methods**:
- `UpdateTowerUpgradeUI(TowerUpgradeSystem tower)`: Shows the upgrade panel for a tower
- `HideTowerUpgradePanel()`: Hides the tower upgrade panel
- `ShowMainMenu()`: Displays the main menu
- `ShowGameUI()`: Displays the main game UI
- `ShowPauseMenu()`: Displays the pause menu
- `ShowGameOver()`: Displays the game over screen
- `ShowVictory()`: Displays the victory screen

**Usage**:
Attach to a UI controller GameObject. Connect all UI elements in the inspector.

### TowerUpgradeUI.cs
**Purpose**: Simplified UI system focused on tower upgrades.

**Key Features**:
- Tower selection display
- Upgrade cost information
- Resource display

**Public Methods**:
- `UpdateTowerUpgradeUI(TowerUpgradeSystem tower)`: Updates the UI with tower information
- `ShowBuildSelectionPanel()`: Shows the building selection panel
- `UpgradeSelectedTower()`: Upgrades the currently selected tower
- `SellSelectedTower()`: Sells the currently selected tower

**Usage**:
Alternative to UIManager for simpler UI implementations.

### TowerSelector.cs
**Purpose**: Handles selection of towers in the game world.

**Key Features**:
- Mouse-based tower selection
- Integration with UI systems
- Building selection events

**Usage**:
Attach to a GameObject in the scene. Ensure a camera is tagged as MainCamera.

## Resource Management

### ResourceManager.cs
**Purpose**: Manages all resources in the game (gold, wood).

**Key Features**:
- Resource tracking
- Add/remove functionality
- Event system for UI updates

**Events**:
- `onGoldChanged`: Triggered when gold amount changes
- `onWoodChanged`: Triggered when wood amount changes

**Public Methods**:
- `GetGold()`: Returns current gold amount
- `GetWood()`: Returns current wood amount
- `AddGold(int amount)`: Adds gold to the resource pool
- `RemoveGold(int amount)`: Removes gold from the resource pool
- `AddWood(int amount)`: Adds wood to the resource pool
- `RemoveWood(int amount)`: Removes wood from the resource pool
- `HasEnoughGold(int amount)`: Checks if there's enough gold
- `HasEnoughWood(int amount)`: Checks if there's enough wood

**Usage**:
Place one instance in the scene. Access through the singleton Instance property.

## Game Management

### GameManager.cs
**Purpose**: Central game state manager.

**Key Features**:
- Game state management (menu, playing, paused, etc.)
- Wave progression
- Lives system
- Win/lose conditions

**Enums**:
- `GameState`: Menu, Playing, Paused, GameOver, Victory

**Events**:
- `onGameStateChanged`: Triggered when game state changes
- `onWaveChanged`: Triggered when wave number changes
- `onLivesChanged`: Triggered when lives count changes

**Public Methods**:
- `StartNewGame()`: Begins a new game
- `StartNextWave()`: Starts the next enemy wave
- `PauseGame()`: Pauses the game
- `ResumeGame()`: Resumes the game
- `ReturnToMenu()`: Returns to the main menu
- `GetCurrentState()`: Returns the current game state
- `GetCurrentWave()`: Returns the current wave number
- `GetLives()`: Returns the current lives count

**Usage**:
Place one instance in the scene. Control game flow through this component.

### GridManager.cs
**Purpose**: Manages the grid system for building placement.

**Key Features**:
- Grid generation
- Cell occupation tracking
- Pathfinding integration

**Public Methods**:
- `GetCellAtPosition(Vector3 worldPosition)`: Gets the grid cell at a world position
- `IsCellAvailable(GridCell cell)`: Checks if a cell is available for building placement
- `RegisterBuilding(BuildingBase building, GridCell cell)`: Registers a building with a grid cell

**Usage**:
Place one instance in the scene. Configure grid size and cell dimensions in the inspector.

### GridCell.cs
**Purpose**: Represents a single cell in the grid system.

**Key Features**:
- Building occupation tracking
- Position information
- Neighbor references

**Public Methods**:
- `SetOccupied(BuildingBase building)`: Marks the cell as occupied by a building
- `IsOccupied()`: Checks if the cell is occupied
- `IsWalkable()`: Checks if enemies can walk through this cell

**Usage**:
Managed automatically by GridManager.

### GridPathfinder.cs
**Purpose**: Handles pathfinding for enemies.

**Key Features**:
- A* pathfinding algorithm
- Dynamic obstacle handling
- Path caching

**Public Methods**:
- `FindPath(Vector3 start, Vector3 end)`: Finds a path between two points
- `UpdatePathfindingGraph()`: Updates the pathfinding graph when obstacles change

**Usage**:
Managed automatically by the system. Used by EnemyAI for navigation.

### HealthBar.cs
**Purpose**: Visual representation of health for enemies and buildings.

**Key Features**:
- Bar visualization of health percentage
- Automatic positioning
- Smooth transitions

**Usage**:
Attach to health bar UI elements. Automatically connects to IDamageable objects.

## Tower Upgrade System

### TowerUpgradeSystem.cs (tower_upgrade.cs)
**Purpose**: Manages tower upgrading functionality.

**Key Features**:
- Tier-based upgrade system
- Resource cost management
- Visual upgrades

**Enums**:
- `TowerTier`: Tier1, Tier2, Tier3

**Structs**:
- `UpgradeCost`: Defines resource costs for upgrades

**Public Methods**:
- `UpgradeTower()`: Attempts to upgrade the tower
- `SellTower()`: Sells the tower for a resource refund
- `CanUpgrade()`: Checks if the tower can be upgraded
- `GetCurrentTier()`: Returns the current upgrade tier
- `GetMaxTier()`: Returns the maximum tier for this tower type
- `GetNextUpgradeCost()`: Returns the cost to upgrade to the next tier
- `GetUpgradeProgress()`: Returns progress towards being able to afford the next upgrade
- `IsMaxTier()`: Checks if the tower is already at its maximum tier

**Usage**:
Attach to buildings that can be upgraded. Configure upgrade costs in the inspector.
