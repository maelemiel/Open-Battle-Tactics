# Open Battle Tactics

- **[📊 Quick Summary](./SUMMARY.md)**

## ⚠️ Branches
- `main` - Docs, `game_data/` JSON exports, scripts, partial asset extraction.
- `reverse-engineering` - Full APK contents (`AndroidManifest.xml`, `lib/`, `res/`, `META-INF/`, original APK zip).

## 📋 Info
- Super Battle Tactics asset extract project.
- Turn-based mobile strategy game (iOS/Android).
- Goal: analyze, document, preserve, reconstruct open source.

## 📁 Structure (main branch)
- `assets/` - Original game assets
  - `adapters.config`
  - `dataModel.db` - Data model database (67 tables, integrity OK)
  - `keyValue.db` - Key-value database
  - `bin/Data/` - AssetRipper PNG export only (original Unity data files live on `reverse-engineering` branch)
- `extracted_assets/`
  - `AudioClip/` - 3 .wav files, all 0 bytes (broken extraction)
  - `Shader/` - 6 files, all 0 bytes (broken extraction)
  - `TextAsset/` - 23 .txt files, all 0 bytes (broken extraction)
  - `Texture2D/` - 21 .png files, valid
- `game_data/` - 559 JSON exports from `dataModel.db` (all valid)
- `scripts/` - Export scripts

## ⚠️ Not in this repo
- `AndroidManifest.xml`, `lib/`, `res/`, `classes.dex`, original APK: `reverse-engineering` branch.
- Extracted C# scripts (`AssetRipper/`): gitignored, not published.

## 🔧 Tech
- Engine: Unity (`libunity.so`, `libmono.so`)
- Platform: Android
- Arch: Multi-arch (ARM, x86)
- Audio: Kamcord
- DB: SQLite

## 📊 Content
- Audio: 3 clips (all 0 bytes, need re-extraction)
- Textures: 21 2D textures (valid)
- Shaders: 6 custom shaders (all 0 bytes, need re-extraction)
- Text: 23 config/data files (all 0 bytes, need re-extraction)
- Game data: 67 tables exported to JSON (370 units, 34 abilities)

## 🎯 Goals
- [x] Extract assets (partial: textures only)
- [x] Analyze mechanics
- [x] Document systems
- [x] Document DB schema
- [x] Export DB to JSON (`game_data/`)
- [ ] Re-extract audio/shaders/text (currently 0 bytes)
- [ ] Recover C# game logic (local only, gitignored)
- [ ] Reconstruct open source

## 📖 Docs

### Devs
- **[🚀 QUICK START GUIDE](./QUICKSTART.md)**
  - Setup (Unity/Godot)
  - Extract/load data
  - Core systems (combat, units, abilities)
  - Code examples
  - Tutorial
- **[📦 DATA EXTRACTION GUIDE](./DATA_EXTRACTION.md)**
  - Unity extract tools (AssetStudio, UnityPy)
  - SQLite export JSON
  - Python scripts
  - Troubleshooting
- **[📊 DATABASE SCHEMA REFERENCE](./DATABASE_SCHEMA.md)**
  - 67 tables
  - Schema definitions
  - Sample queries
  - Stats/tips

### Analysis
- **[📋 REVERSE ENGINEERING SUMMARY (Français)](./REVERSE_ENGINEERING_SUMMARY.md)**
  - DB schema (67 tables, 370 units, 34 abilities)
  - Mechanics (combat, progression, gacha, events)
  - Architecture
  - Open source recommendations
  - Roadmap
  - References

## 📚 Resources
- [Wiki](https://super-battle-tactics.fandom.com/wiki/Super_Battle_Tactics_Wiki)
- [MobyGames](https://www.mobygames.com/game/158792/super-battle-tactics/)
- [Behance Assets](https://www.behance.net/gallery/70749049/Super-Battle-Tactics-Game-Assets)
- [Behance Gallery](https://www.behance.net/gallery/122971387/SUPER-BATTLE-TACTICS-(iOSAndroid))
- [ArtStation Art 1](https://www.artstation.com/artwork/L4BmdR)
- [ArtStation Art 2](https://www.artstation.com/artwork/meWkZ)
- [ArtStation Art 3](https://www.artstation.com/artwork/9KDXQ)

## ⚖️ License
- Educational/research/preservation only.
- Assets owned by respective owners (Mobage/DeNA). No affiliation.
- Original art kept public for reference/archival until community replacements exist (OpenGFX model).
- Reconstruction goal: replace all original assets with open content.

## 🤝 Contribute
- Open issues/PRs to improve docs, add analysis, fix errors.
