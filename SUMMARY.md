# Super Battle Tactics RE Summary

## Database (Complete)
* Tables: 67 (Full schema)
* Units: 370 (Stats, types, rarities)
* Abilities: 34 (4 types)
* Unit Types: 14 (Assault, Command, Operative, etc.)
* Rarity Levels: 5 (Common to Legendary)
* Languages: 14 (Full localization)
* Events: Multiple
* Gacha: 7 tables (Pools, prizes, rates)
* AI: 3 tables (Armies, handlers)
* Items: Full catalog

## Assets (Partial)
* Textures: 21 PNG files (valid)
* Audio: 3 WAV files (all 0 bytes, need re-extraction)
* Shaders: 6 files (all 0 bytes, need re-extraction)
* Text: 23 files (all 0 bytes, need re-extraction)
* Unity Data: on `reverse-engineering` branch (`assets/bin/Data/`); main has AssetRipper PNG export only

## Code & Build
* APK: 52 MB on `reverse-engineering` branch (not on main)
* DEX: `reverse-engineering` branch
* Native Libs: `reverse-engineering` branch
* Manifest: `reverse-engineering` branch
* C# scripts: extracted locally via AssetRipper, gitignored (not published)

## Documentation
* Complete analysis: [REVERSE_ENGINEERING_SUMMARY.md](./REVERSE_ENGINEERING_SUMMARY.md)
* Rebuild guide: [QUICKSTART.md](./QUICKSTART.md)
* DB reference: [DATABASE_SCHEMA.md](./DATABASE_SCHEMA.md)
* Asset extraction: [DATA_EXTRACTION.md](./DATA_EXTRACTION.md)
* Overview: [README.md](./README.md)

## Documented Systems
* Combat: Phases (First Strike, Ultra, Pre-Combat, Attack, Reactive), Initiative, targeting, damage math, abilities.
* Progression: Unlocking, research, tiers, levels, upgrades, divisions, promotions, leaderboards.
* Economy: Items, prices, gems, currency, scrap, blueprints, gacha.
* Content: 370 units, 34 abilities, events, AI opponents, missions.

## Immediate Use
* Repo: `git clone https://github.com/maelemiel/Open-Battle-Tactics.git`
* Access DB: `sqlite3 assets/dataModel.db`
* View units: `sqlite3 assets/dataModel.db "SELECT * FROM unit LIMIT 10;"`
* Export JSON: `pip install sqlite-utils && sqlite-utils assets/dataModel.db "SELECT * FROM unit" --json > units.json`

## Reconstruction Viability
* Data Completeness: 95%
* Mechanics: 90%
* Assets: 40% (Need creation)
* Tech Info: 85% (Unity + SQLite)
* External Refs: 80%
* Overall Feasibility: High
* Effort: Solo (12-18mo), Team (6-12mo), Comm (4-8mo) for v1.0.

## Missing Assets (Need Creation)
* Critical: Unit sprites/models, UI, SFX, music, ability icons, combat animations.
* Important: VFX, background art, portraits, menu animations.
* Optional: 3D models, voice acting, cutscenes.

## External Resources
* Wiki: [Fandom](https://super-battle-tactics.fandom.com)
* DB Page: [MobyGames](https://www.mobygames.com/game/158792/super-battle-tactics/)
* Art: [Behance 1](https://www.behance.net/gallery/70749049/Super-Battle-Tactics-Game-Assets), [Behance 2](https://www.behance.net/gallery/122971387/SUPER-BATTLE-TACTICS-(iOSAndroid)), [ArtStation 1](https://www.artstation.com/artwork/L4BmdR), [ArtStation 2](https://www.artstation.com/artwork/meWkZ), [ArtStation 3](https://www.artstation.com/artwork/9KDXQ)

## Original Tech Stack
* Engine: Unity (C#/Mono)
* DB: SQLite 3.x
* Platform: Android APK
* Backend: Mobage/DeNA (Offline)

## Legal
* Game discontinued.
* Original assets copyrighted.
* Data/mechanics reimplementation OK.
* Original art/audio mandatory.
* Rename project.
* MIT/GPL recommended.

## Next Steps
* Read [REVERSE_ENGINEERING_SUMMARY.md](./REVERSE_ENGINEERING_SUMMARY.md), [QUICKSTART.md](./QUICKSTART.md).
* Export JSON ([DATA_EXTRACTION.md](./DATA_EXTRACTION.md)), extract Unity assets.
* Build prototype (5-10 units, basic combat).
* Expand game loop, UI.
* Add art, test, open source release.

## Stats
* Tables: 67
* Units: 370
* Abilities: 34
* Unit Types: 14
* Rarities: 5
* Languages: 14
* Docs: 5 (~50KB)
* Code Samples: 10+
* Python Scripts: 5+
* SQL Queries: 20+
* Updated: September 8, 2026
