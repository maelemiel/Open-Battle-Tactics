# Quick Start - Rebuild Super Battle Tactics

## Prerequisites
- Unity or Godot knowledge
- SQLite DB experience
- Game dev fundamentals
- Git, GitHub knowledge

## Setup Env
- Option A: Unity (Recommended)
  - Install Unity 2021.3 LTS+
  - Add Android Build Support
  - Add iOS Build Support (optional)
- Option B: Godot
  - Get Godot 4.x: https://godotengine.org/download

## Clone Repo
- `git clone https://github.com/maelemiel/Open-Battle-Tactics.git`
- `cd Open-Battle-Tactics`

## Data
- Install sqlite3:
  - Ubuntu/Debian: `sudo apt-get install sqlite3`
  - macOS: `brew install sqlite3`
  - Windows: https://sqlite.org/download.html
- Explore DB:
  - `cd assets`
  - `sqlite3 dataModel.db`
  - Commands: `.tables`, `.schema unit`, `.schema ability`
  - Queries: `SELECT * FROM unit LIMIT 5;`, `SELECT * FROM ability LIMIT 5;`
- Export JSON:
  - `pip install sqlite-utils`
  - `sqlite-utils dataModel.db "SELECT u.*, l.en_us as name FROM unit u JOIN localization l ON u.key_name = l.key" --json > ../game_data/units.json`
  - `sqlite-utils dataModel.db "SELECT a.*, ln.en_us as name, ld.en_us as description FROM ability a JOIN localization ln ON a.key_name = ln.key JOIN localization ld ON a.key_description = ld.key" --json > ../game_data/abilities.json`

## Create Project
- Unity:
  - New 2D/3D project
  - Paths: `Assets/Scripts/Units/`, `Assets/Scripts/Abilities/`, `Assets/Scripts/Combat/`, `Assets/Scripts/UI/`, `Assets/Scripts/Data/`
  - Put JSON in `Assets/Resources/GameData/`
- Godot:
  - Paths: `scenes/combat/`, `scenes/menu/`, `scripts/units/`, `scripts/systems/`
  - Put JSON in `resources/game_data/`

## Implement Systems
- Structures (C#):
  - Unit.cs: id, name, UnitType (Assault=1, Command=2, Operative=3, Helicopter=4), rarity, health, damage, initiative, stats, abilities
  - UnitStats: attack, defense, speed, range
  - Ability.cs: id, name, description, AbilityType (Ultra=1, PreCombat=2, Reactive=3, Passive=4), actionPoints, targetGroup, effects
- Loader (C#):
  - Load `Resources.Load<TextAsset>("GameData/units")`
  - Parse JSON to Dictionary
- Combat Manager (C#):
  - Sort by initiative
  - Loop phases: FirstStrike, UltraAbilities, PreCombat, PlayerAttack, EnemyAttack, Reactive, EndTurn
  - Calculate damage: `Mathf.Max(1, baseDamage - defense / 2)`
- Minimal UI:
  - BattleUI.cs: playerUnitContainer, enemyUnitContainer, spawn unit cards
  - UnitCard.cs: Update name, HP
- Test:
  - Load units 11001 (VOLT), 11002 (LONGSHOT)
  - Start combat

## Iteration Plan
- Phase 1: Core (Load data, combat math, simple UI)
- Phase 2: Content (Import 370 units, 34 abilities, art)
- Phase 3: Progression (Unlocks, research, economy)
- Phase 4: Multiplayer (PvP, Leaderboards)

## Resources
- Docs: `./REVERSE_ENGINEERING_SUMMARY.md`, `./DATABASE_SCHEMA.md`
- Links: https://learn.unity.com/, https://docs.godotengine.org/, https://www.sqlitetutorial.net/

## Rules
- Start small (5-10 units)
- Iterate fast, test often
- Write original art, no ripped assets
- Use MIT/GPL-3.0 license
- Rename to avoid trademarks
