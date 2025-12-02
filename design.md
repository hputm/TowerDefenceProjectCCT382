# Siege of the Keep - Game Design Document

## Overview

"Siege of the Keep" is a Unity-based tower defense game that reinterprets the genre through the lens of medieval siege warfare and settlement management. Drawing inspiration from the systemic depth of Manor Lords and the layered threat ecology of Mount & Blade, the game blends tactical defense placement with strategic resource and population planning. The player assumes the role of a medieval lord tasked with defending a central keep against escalating waves of enemies—from desperate refugees and bandits to professional armies—while simultaneously managing the internal growth of their settlement.

Critically, the game distinguishes between two types of structures: defensive buildings, which are physically placed on the terrain and can be destroyed, and civilian/economic buildings, which are managed abstractly through a UI panel and never appear on the battlefield.

## Defensive Architecture

Defensive architecture forms the tactical backbone of gameplay. Players directly place structures such as Arrow Towers, Watchposts, Barracks, Cannon Emplacements, and Barricades along enemy approach routes. Each of these buildings can host a limited number of militia and is fully destructible during combat—a key strategic consideration.

For example, a forward Arrow Tower may provide early damage against incoming waves but risks being overrun and destroyed by a heavy assault. To mitigate attrition, players can manually redeploy stationed troops from compromised frontline positions to fallback defenses deeper in the map, preserving valuable manpower. This dynamic encourages layered, depth-based defense planning rather than static "wall-of-towers" strategies.

## Population Management

The operation of these defenses depends entirely on the player's available population. Civilian infrastructure—Housing, Farms, Workshops—is not rendered on the map. Instead, their quantities and effects are displayed in a centralized management panel. Housing determines total population capacity; only with sufficient residents can the player recruit militia.

The Militia Training Camp (a defensive building placed on-map) serves as the recruitment hub and initial garrison point, but actual deployment occurs when troops are assigned to specific defensive structures like Watchposts or Barracks.

A strict ratio governs this system: at most one-third of the total population can serve as active militia without destabilizing the settlement's economy. Over-recruitment reduces income and may cause unrest.

## Manpower Allocation

Manpower allocation is granular and consequential. Each building type has specific capacity:
- Arrow Towers: Hold 3–4 militiamen
- Watchposts: Hold 2 militiamen
- Barracks: Hold up to 6 (serving as a mobile reserve)
- Cannon Emplacements: Require 2 specialized operators

These units are drawn from the global militia pool. If a building is destroyed, its garrison is not lost—they return to the reserve pool unless killed in combat—and can be reassigned. This system emphasizes force preservation as much as enemy elimination. Wise lords know when to abandon a crumbling barricade and retreat their soldiers to stronger redoubts.

## Enemy Design

Enemy design follows a dual progression model: temporal and reactive. Early waves feature low-threat units like refugees or bandits. However, as the player's settlement grows—indicated by high housing counts, strong defenses, or accumulated wealth—it draws the attention of more dangerous foes.

Coastal maps may see pirate landings; mountain passes might funnel knightly warbands; and sustained prosperity could trigger a full-scale siege by a rival kingdom. This reactive AI means aggressive expansion carries real risk, reinforcing the game's core tension between growth and vulnerability.

## Battlefield Environment

The battlefield itself is shaped by terrain. Pre-designed maps include mountain fortresses (with narrow chokepoints), island keeps (limited land access), and coastal citadels (dual land-sea threats). Terrain dictates valid build zones and enemy pathing, ensuring each map demands unique defensive layouts.

Destroyed buildings leave rubble that may slightly alter movement paths, adding emergent tactical nuance during prolonged sieges.

## Visual and Audio Design

Visually, the game uses a grounded medieval aesthetic with stylized textures and period-appropriate architecture. Destructible buildings feature progressive damage states—cracked walls, burning roofs—before collapsing entirely.

Audio cues differentiate enemy types (e.g., pirate chants, cavalry charges), while UI indicators show real-time garrison status and population metrics. The central keep remains indestructible until its health bar is depleted—its fall ends the game.

## Summary

"Siege of the Keep" elevates tower defense by integrating destructible, player-placed fortifications with a deep population-driven economy. It challenges players not only to build wisely but to adapt dynamically—abandoning lost positions, conserving troops, and balancing domestic growth against military exposure. By merging the strategic depth of settlement management with the tactical immediacy of siege warfare, the game offers a fresh, historically grounded experience that honors its inspirations while carving a distinct identity in the genre.

---

# Development Plan

The prototype phase will focus on validating the core tower defense loop within a medieval siege setting, establishing foundational systems that can later support deeper economic and tactical mechanics. The primary goal is to implement a playable vertical slice featuring multiple defensive building types, enemy waves with escalating difficulty, terrain-based pathfinding, and a simplified population model—all within a single representative map (e.g., a mountain fortress).

## Scope & Limitations

The prototype will include the following key elements:

### Placeable Defensive Structures
- Arrow Towers
- Watchposts
- Barracks
- Cannon Emplacements
- Barricades

Each with distinct attack patterns and visual feedback.

### Basic Population System
- Total population increases randomly over time when spare housing capacity exists (as shown in the UI panel)
- Housing and Training Camp are abstracted as panel entries, not physical objects

### Core Enemy Waves
- Refugees
- Bandits
- Knights
- Basic siege unit

Each with unique stats and visual identity.

### Functional Resource System
- Gold earned per defeated enemy
- Used to construct or upgrade defenses

### Pathfinding
- Unity-based pathfinding (NavMesh) for enemies
- Terrain blocking (e.g., mountains) enforced via baked navigation areas

## Deferred Mechanics

Several advanced mechanics will be deferred to post-prototype development due to time and complexity constraints:

1. Troop redeployment or retreat logic will not be implemented; garrisons remain fixed in their assigned buildings until destruction.
2. Building destruction will not produce persistent debris or enable reconstruction—once a tower is destroyed, it is removed permanently, and its garrison is lost (simplifying balance and state management).
3. Full economic simulation (e.g., taxation rates, civilian labor allocation, unrest) is out of scope; income will be tied directly to enemy kills and passive housing count, without granular production chains.
4. Dynamic enemy escalation based on player prosperity will be replaced by pre-scripted wave sequences for prototype stability.
5. Art development will likely not be part of the prototype; it will remain as greybox or use existing online assets.