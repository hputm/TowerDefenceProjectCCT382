# Unity Tower Defense Game Script Usage Guide

This document aims to provide development teams with guidance on how to properly use various scripts in the Unity project, including script functionality introduction, usage methods, and mounting objects.

## Table of Contents
1. [Core Management Classes](#core-management-classes)
   - [GameManager](#gamemanager)
   - [ResourceManager](#resourcemanager)
   - [BuildingPlacementSystem](#buildingplacementsystem)
   - [GridManager](#gridmanager)
   - [EnemySpawner](#enemyspawner)

2. [Game Entity Classes](#game-entity-classes)
   - [Building Classes](#building-classes)
   - [Enemy Classes](#enemy-classes)
   - [Projectile](#projectile-class)

3. [Utility Classes](#utility-classes)
   - [UI Classes](#ui-classes)
   - [GridPathfinder](#gridpathfinder-class)
   - [Waypoints](#waypoints-class)

---

## Core Management Classes

### GameManager

#### Function Description
GameManager is the core controller of the entire game, responsible for managing game states (menu, playing, paused, victory, defeat), player lives, wave progress, and more.

#### Usage Method
1. Create an empty GameObject named "GameManager"
2. Attach the [GameManager.cs](file:///Users/tomchen/Documents/CCT382/Towerdefence/TowerDefenceProjectCCT382/Assets/Scripts/GameManager.cs) script to this object
3. Configure the following parameters in the Inspector panel:
   - Initial Lives: Starting lives
   - Keep: Reference to player's main castle object
   - Resource Manager: Reference to resource manager
   - Enemy Spawner: Reference to enemy spawner

#### Notes
- This is a singleton class, there should only be one instance in the entire scene
- Do not manually delete or disable this component

### ResourceManager

#### Function Description
ResourceManager is responsible for managing resources in the game, such as gold, wood, etc., as well as building unlock status.

#### Usage Method
1. Create an empty GameObject named "ResourceManager"
2. Attach the [ResourceManager.cs](file:///Users/tomchen/Documents/CCT382/Towerdefence/TowerDefenceProjectCCT382/Assets/Scripts/ResourceManager.cs) script to this object
3. Configure initial resource values and building unlock costs in the Inspector panel

#### Notes
- This is also a singleton class
- All resource changes should be done through the methods of this class

### BuildingPlacementSystem

#### Function Description
BuildingPlacementSystem handles the placement logic of buildings, including placement preview, validity checking, and actual creation of buildings.

#### Usage Method
1. Create an empty GameObject named "BuildingPlacementSystem"
2. Attach the [BuildingPlacementSystem.cs](file:///Users/tomchen/Documents/CCT382/Towerdefence/TowerDefenceProjectCCT382/Assets/Scripts/BuildingPlacementSystem.cs) script to this object
3. Configure building prefab mappings and placement-related parameters

#### Notes
- Needs to work together with the grid system (GridManager)
- Provides methods for entering and exiting placement mode for other systems to call

### GridManager

#### Function Description
GridManager manages grid cells in the system, including roads, empty land, and impassable areas, and provides conversion functions between grid coordinates and world coordinates.

#### Usage Method
1. Create an empty GameObject named "GridManager"
2. Attach the [GridManager.cs](file:///Users/tomchen/Documents/CCT382/Towerdefence/TowerDefenceProjectCCT382/Assets/Scripts/GridManager.cs) script to this object
3. Set grid dimensions, cell size, and references to various cell prefabs
4. Call InitializeGrid() method to initialize the grid when the game starts

#### Notes
- It is the foundation system for grid-based tower defense games
- Other systems need to go through it to get grid information at specific locations

### EnemySpawner

#### Function Description
EnemySpawner is responsible for enemy generation, supporting wave mode, weighted random generation, and progressive difficulty functions.

#### Usage Method
1. Create an empty GameObject named "EnemySpawner"
2. Attach the [EnemySpawner.cs](file:///Users/tomchen/Documents/CCT382/Towerdefence/TowerDefenceProjectCCT382/Assets/Scripts/EnemySpawner.cs) script to this object
3. Set enemy spawn points, enemy prefab list, and other spawn parameters

#### Notes
- Need to configure enemy prefabs with EnemyDefiner component
- Can dynamically adjust enemy strength and spawn frequency according to waves

---

## Game Entity Classes

### Building Classes

#### BuildingBase (Base Class)
Base class for all buildings, providing generic functions such as health and attack capabilities.

#### ArrowTower (Arrow Tower)
Inherits from BuildingBase, implements a defensive tower with ranged attack functionality.

Usage:
1. Create arrow tower prefab
2. Attach the [ArrowTower.cs](file:///Users/tomchen/Documents/CCT382/Towerdefence/TowerDefenceProjectCCT382/Assets/Scripts/ArrowTower.cs) script to the prefab
3. Configure tier attributes and attack parameters
4. Register this prefab in BuildingPlacementSystem

#### DefenseBuilding (Defense Building)
Inherits from BuildingBase, implements a building with melee defense functionality.

#### RoadBlock (Road Block)
Inherits from BuildingBase, cannot attack but can block enemy advancement.

#### Castle (Castle)
Player's main base, game over if destroyed.

### Enemy Classes

#### Enemy (Basic Enemy)
Base class for all enemies, implementing basic movement and health systems.

#### EnhancedEnemy (Enhanced Enemy)
Inherits from Enemy, has more complex AI behavior and animation control.

#### EnemyAI (Enemy AI)
Handles artificial intelligence behavior for enemies.

### Projectile Class

#### Projectile (Projectile)
Handles the flight and damage logic of projectiles generated by ranged attacks like arrow towers.

Usage:
1. Create projectile prefab
2. Attach the [Projectile.cs](file:///Users/tomchen/Documents/CCT382/Towerdefence/TowerDefenceProjectCCT382/Assets/Scripts/Projectile.cs) script to the prefab
3. Configure flight speed and other parameters
4. Reference this prefab in the arrow tower class

---

## Utility Classes

### UI Classes

#### UIManager
Responsible for overall UI management and updates.

#### TowerUpgradeUI
Specifically handles display and interaction of tower upgrade interface.

#### TowerSelector
Handles tower selection logic.

### GridPathfinder Class

#### GridPathfinder
Implements grid-based pathfinding algorithm, enabling enemies to move along roads.

### Waypoints Class

#### Waypoints
Simple waypoint marking system.