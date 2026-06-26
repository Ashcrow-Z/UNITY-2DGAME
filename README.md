# Dungeon of SIP

A 2D top-down dungeon shooter built in Unity. Clear procedurally generated levels, defeat enemies with mouse-aimed shooting, survive two levels, and compete for the best score on the local leaderboard.

## Requirements

- **Unity 2022.3.62f3** (LTS)
- Windows / macOS / Linux editor supported

## Quick Start

1. Clone or open this repository in Unity **2022.3.62f3**.
2. Wait for scripts to compile.
3. If scenes or project settings need to be rebuilt, open the menu:
  - **DungeonGame > Build All Scenes**
4. Open `Assets/Scenes/MainMenu.unity`.
5. Press **Play** in the Editor, or build a standalone executable from **File > Build Settings**.

> **Note:** Start from `MainMenu.unity`, not `SampleScene.unity`.



## Controls


| Input                 | Action                |
| --------------------- | --------------------- |
| `W` / `A` / `S` / `D` | Move                  |
| Mouse                 | Aim                   |
| Left Click            | Shoot                 |
| Left Shift            | Sprint (uses stamina) |
| `Esc` / `Space`       | Pause / Resume        |




## Gameplay Overview

- **Two levels** with increasing difficulty.
- **Procedural 23×23 dungeon** with indestructible borders and random obstacles.
- **Player:** 5 HP per life, 3 lives, stamina-based sprint, mouse-aimed attacks, temporary invincibility after damage.
- **Enemies:** Normal and Reinforced types with patrol, chase, and combat behavior.
- **Level 2:** Faster enemies, respawning enemies, and collectible props.
- **Scoring:** Points for defeating enemies; timer and score tracked on the HUD.
- **Leaderboard:** Local high-score list with player name input.



## Features



### Core Gameplay

- Top-down movement and shooting
- Independent movement and aiming
- Bullet pooling for performance
- Destructible and indestructible walls
- Health bars for player and enemies



### Enemy AI

- State machine: **Idle / Patrol**, **Chase**, **Combat**
- Patrol: short random moves with 1–2 second pauses
- Chase: **A** pathfinding on the dungeon grid (Manhattan heuristic)
- Combat: maintain distance, strafe, and fire at the player
- Detection and attack ranges to avoid constant full-map pursuit



### UI

- Main menu with animated background particles
- Settings panel (BGM / SFX volume, saved with `PlayerPrefs`)
- Pause menu: Continue, Main Menu, Quick Restart (current level)
- HUD: score, timer, lives, enemy count, current level, stamina bar
- Game Over, Victory, and Leaderboard screens



### Audio

- Procedurally generated BGM and SFX (no external audio files required)
- Menu music and level-specific background tracks
- Sound effects for shooting, hits, explosions, pickups, damage, victory, defeat, and UI clicks
- Volume controls integrated with the settings menu



## Project Structure

```
Assets/
├── Scenes/
│   ├── MainMenu.unity      # Entry scene
│   ├── GameLevel.unity     # Gameplay scene
│   └── SampleScene.unity   # Default Unity scene (not used)
└── Scripts/
    ├── Core/               # GameManager, LevelManager, bootstrap, audio, setup
    ├── Player/             # Movement, health, shooting
    ├── Enemy/              # Enemy types, AI, pathfinding
    ├── Combat/             # Bullets and object pooling
    ├── Map/                # Map generation and obstacles
    ├── Props/              # Pickups and spawner
    ├── UI/                 # Menus, HUD, health bars
    └── Editor/             # GameSceneBuilder (scene/setup automation)
```



## Key Systems


| System             | Description                                              |
| ------------------ | -------------------------------------------------------- |
| `GameBootstrap`    | Creates persistent managers before the first scene loads |
| `GameSetup`        | Runtime prefab generation and physics layer setup        |
| `MapGenerator`     | Procedural map with BFS reachability validation          |
| `EnemyAI`          | A pathfinding and patrol/chase navigation                |
| `AudioManager`     | BGM/SFX playback and volume persistence                  |
| `ProceduralAudio`  | Runtime-generated audio clips                            |
| `GameSceneBuilder` | Editor tool to build scenes, tags, layers, and UI        |




## Scene Setup Tool

The custom editor menu **DungeonGame > Build All Scenes** automates:

- Tag and layer configuration
- 2D physics collision matrix
- Main menu UI (menu, settings, leaderboard)
- Game level scene (HUD, pause, game over, victory)
- Build settings for both scenes

Use this after cloning the project or if scene references appear out of sync.

## Development Notes

- **Sprites** are generated at runtime via `SpriteGenerator` (colored shapes), so no imported art assets are required.
- **Prefabs** are created at runtime by `GameSetup` and parented under a persistent object so they survive scene transitions.
- **Player–enemy collision** is ignored at the physics layer level so units do not push each other, while wall collision remains active.
- **Audio** is synthesized in code; quality is functional but intentionally simple.



## Known Limitations

- Visuals and audio are procedural rather than hand-authored assets.
- Leaderboard data is stored locally on the device.
- If gameplay objects or UI seem missing, run **DungeonGame > Build All Scenes** and reopen `MainMenu.unity`.



## Assignment Context

This project was developed for testing myself after leaning or finishing:

- **Module:** CPT306 – Game Design Theory and Practice
- **Assignment:** CW1 – Creating a 2D Game

For detailed design rationale, enemy behavior, and setup notes, refer to the project’s **Game Specification** document if included with your submission.

## License

Coursework project for academic submission. Replace this section if you add a specific license later.