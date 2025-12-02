# Siege of the Keep - Development Plan

This document outlines the development plan for implementing the "Siege of the Keep" tower defense game based on the design document. It details the required scripts, their responsibilities, and分工 among team members.

## Current Status

We currently have a partial implementation of the core systems:
- Basic enemy movement along waypoints ✅
- Tower attack and health system ✅
- Tower upgrade mechanics ✅
- Enemy definition and spawning system ✅
- Resource management system ✅
- Game state management ✅
- Building system with placement logic ✅
- Combat systems ✅
- Grid-based map system ✅
- Enemy AI system ✅ *(Partially implemented)*

## Required Script Components

### 1. Resource Management System ✅ *(Implemented)*

#### ResourceManager.cs
**Responsibility:** Centralized management of game resources (gold, wood, stone, population)
**Functions:**
- Track and update resource amounts
- Handle resource gain/loss events
- Provide interface for other systems to query resources
- Manage population statistics (total, available, militia)

#### GameCurrencyDisplay.cs
**Responsibility:** UI display of current resources
**Functions:**
- Show current gold, wood, stone amounts
- Display population information
- Update in real-time as resources change

### 2. Game State Management ✅ *(Partially Implemented)*

#### GameManager.cs
**Responsibility:** Overall game state control
**Functions:**
- Manage game states (Playing, Paused, Victory, Defeat)
- Control wave progression
- Track player lives/keep health
- Handle win/lose conditions
- Coordinate between systems

#### WaveManager.cs
**Responsibility:** Enemy wave sequencing and management
**Functions:**
- Define wave compositions
- Schedule enemy spawns
- Track wave progress
- Notify systems of wave events

### 3. UI Systems

#### UIManager.cs
**Responsibility:** Central UI coordination
**Functions:**
- Manage UI panels and elements
- Handle user interactions with UI
- Coordinate UI updates across systems
- Show game state notifications

#### TowerUpgradeUI.cs
**Responsibility:** Tower upgrade interface
**Functions:**
- Display upgrade options for selected towers
- Show upgrade costs and requirements
- Handle upgrade purchase interactions
- Visual feedback for available upgrades

#### BuildingPlacementUI.cs
**Responsibility:** Building placement interface
**Functions:**
- Show available building types
- Handle building selection
- Display building costs
- Manage placement confirmation/cancellation

### 4. Combat Systems ✅

#### Projectile.cs ✅
**Responsibility:** Projectile behavior and damage delivery
**Functions:**
- Move toward targets
- Handle collision detection
- Apply damage on hit
- Support different projectile types

#### IDamageable.cs ✅
**Responsibility:** Standardize damage handling
**Functions:**
- Define common damage handling interface
- Allow consistent damage application across entities

#### HealthBar.cs ✅
**Responsibility:** Visual health indication
**Functions:**
- Display health bars for enemies and towers
- Update in real-time with health changes
- Handle visibility logic

### 5. Building Systems ✅

#### BuildingBase.cs ✅
**Responsibility:** Base class for all buildings
**Functions:**
- Define common building properties (health, attack range, attack speed, attack damage)
- Handle placement logic
- Manage building state
- Provide foundation for inheritance

#### DefensiveBuilding.cs ✅
**Responsibility:** Base class for defensive structures
**Functions:**
- Implement common defensive behavior
- Handle garrison mechanics
- Manage destruction logic
- Support upgrade system

#### ArrowTower.cs ✅
**Responsibility:** Arrow tower implementation
**Functions:**
- Implement ranged attack logic with projectiles
- Support 3 upgrade tiers
- Placement restricted to road ends only
- Manage projectile firing

#### RoadBlock.cs ✅
**Responsibility:** Road block implementation
**Functions:**
- Purely defensive (no attack capability)
- Block enemy movement
- Have health that depletes over time or with attacks
- Placement restricted to roads only
- Can upgrade to defensive building

#### DefenseBuilding.cs ✅
**Responsibility:** Defensive building implementation
**Functions:**
- Can have attack capability or be pure blocker
- Placement restricted to roads only
- Support upgrade system

#### Castle.cs ✅
**Responsibility:** Player's main castle/keep
**Functions:**
- Track keep health
- Handle defeat condition when health reaches zero
- Visual feedback for damage state
- Static placement (cannot be moved by player)

#### BuildingPlacementSystem.cs ✅
**Responsibility:** Handles placement of buildings
**Functions:**
- Manage placement constraints (roads, road ends)
- Handle placement costs
- Visual feedback for valid/invalid placement
- Instantiate buildings

### 6. Grid Map System ✅

#### GridCell.cs ✅
**Responsibility:** Represents individual grid cells
**Functions:**
- Define properties of grid cells (road, road end, empty, impassable)
- Determine what can be placed in each cell
- Track occupancy by buildings

#### GridManager.cs ✅
**Responsibility:** Manage the entire grid system
**Functions:**
- Create and manage all grid cells
- Convert between world and grid coordinates
- Provide access to grid cells
- Support neighbor queries

#### GridPathfinder.cs ✅
**Responsibility:** Find paths along road grid cells
**Functions:**
- Find paths from start to end positions using road cells
- Support pathfinding for enemies

### 7. Population and Economy

#### PopulationManager.cs
**Responsibility:** Population tracking and management
**Functions:**
- Track total population
- Manage militia allocation
- Handle population growth
- Enforce population constraints

### 8. Enemy Systems ✅ *(Partially Implemented)*

#### Enemy.cs ✅
**Responsibility:** Basic enemy behavior
**Functions:**
- Move along predefined waypoints
- Handle health and damage
- Notify systems when reaching the keep or dying

#### EnemyAI.cs ✅
**Responsibility:** Enhanced enemy AI behavior
**Functions:**
- Move along roads defined by the grid system
- Target and attack buildings that block their path
- Navigate to the endpoint (castle/keep)

#### EnemyGroup.cs
**Responsibility:** Grouped enemy behavior (2-3 enemies together)
**Functions:**
- Coordinate movement of group members
- Handle group cohesion
- Manage group defeat conditions

#### EnemySpawner.cs
**Responsibility:** Spawn enemies in waves
**Functions:**
- Spawn enemies at appropriate times
- Manage wave progression
- Coordinate with GameManager

## Team Member Responsibilities

### Script Developer (You)
**Responsibilities:**
1. Implement all C# scripts listed above
2. Ensure proper communication between systems
3. Write clean, documented code following Unity best practices
4. Create and maintain script architecture documentation
5. Test script functionality and fix bugs

### Game Designer
**Responsibilities:**
1. Define balanced stats for towers, enemies, and resources
2. Design wave compositions and difficulty progression
3. Specify upgrade paths and costs
4. Create building placement rules and restrictions
5. Define win/lose conditions

### Game Builder/Level Designer
**Responsibilities:**
1. Create game scenes and level layouts
2. Set up NavMesh for enemy pathfinding
3. Place waypoints for enemy movement
4. Configure building placement areas
5. Set up UI canvas and interface elements
6. Configure lighting and basic visual effects
7. Create prefabs for towers, enemies, and projectiles

### QA Tester
**Responsibilities:**
1. Test game functionality and identify bugs
2. Verify balance and difficulty curve
3. Check UI responsiveness and clarity
4. Document issues and suggestions for improvement
5. Verify fix implementation

## Implementation Priority

### Phase 1: Core Systems (Highest Priority) ✅
1. ResourceManager.cs ✅
2. GameManager.cs ✅
3. Projectile.cs ✅
4. Keep.cs
5. BuildingBase.cs ✅
6. ArrowTower.cs ✅
7. DefenseBuilding.cs ✅
8. RoadBlock.cs ✅
9. Castle.cs ✅
10. BuildingPlacementSystem.cs ✅
11. IDamageable.cs ✅
12. HealthBar.cs ✅
13. GridCell.cs ✅
14. GridManager.cs ✅
15. GridPathfinder.cs ✅
16. EnemyAI.cs ✅

### Phase 2: UI Systems (High Priority)
1. UIManager.cs
2. GameCurrencyDisplay.cs
3. TowerUpgradeUI.cs

### Phase 3: Building Systems (Medium Priority)
1. BuildingBase.cs ✅
2. DefensiveBuilding.cs ✅
3. ArrowTower.cs ✅
4. RoadBlock.cs ✅
5. DefenseBuilding.cs ✅
6. Castle.cs ✅
7. BuildingPlacementSystem.cs ✅

### Phase 4: Population and Economy (Medium Priority)
1. PopulationManager.cs

### Phase 5: Enhanced Enemy Systems (Lower Priority)
1. EnemyGroup.cs
2. EnemySpawner.cs

## Integration Points

1. **Resource System** integrates with:
   - Tower purchasing/upgrading
   - Enemy gold drop on defeat
   - Population growth (passive income)

2. **Combat System** integrates with:
   - Tower attack mechanics
   - Enemy health/damage
   - Projectile physics

3. **UI System** integrates with:
   - Resource display
   - Tower upgrade interface
   - Building placement interface
   - Game state notifications

4. **Building System** integrates with:
   - Placement restrictions
   - Upgrade paths
   - Garrison mechanics
   - Destruction effects

5. **Grid System** integrates with:
   - Building placement validation
   - Pathfinding for enemies
   - Visual representation of map

6. **Enemy System** integrates with:
   - Grid-based pathfinding
   - Building attack logic
   - Game state management

## Testing Requirements

1. Unit tests for core systems (resources, population, combat calculations)
2. Integration tests for system interactions
3. Playtesting for balance and difficulty curve
4. Performance testing for large battles with many entities
5. Stress testing for edge cases and error conditions

## Milestones

### Milestone 1: Basic Prototype ✅
- Functional resource system ✅
- Basic tower placement and upgrading ✅
- Enemy waves with simple pathfinding ✅
- Win/lose conditions ✅
- Building system with placement logic ✅
- Combat systems with health bars ✅
- Grid-based map system ✅
- Enemy AI system ✅

### Milestone 2: Complete Core Systems
- All defensive building types implemented ✅
- Full UI system with resource display
- Balanced enemy waves and difficulty progression
- Population management system

### Milestone 3: Polish and Enhancement
- Visual and audio feedback improvements
- Advanced enemy AI and grouping
- Performance optimizations
- Bug fixes and balance adjustments

## Dependencies

1. Unity Engine (version to be determined)
2. NavMesh system for pathfinding
3. UI system (Unity UI or alternative)
4. Particle system for visual effects
5. Audio system for sound effects

This plan provides a roadmap for developing the "Siege of the Keep" game from the current partial implementation to a complete playable prototype. Regular team meetings and progress reviews will ensure we stay on track and adapt to any challenges that arise during development.