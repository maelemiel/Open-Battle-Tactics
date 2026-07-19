# Open Battle Tactics

- **[📊 Quick Summary](./SUMMARY.md)**

## 📋 Info
- Super Battle Tactics asset extract project.
- Turn-based mobile strategy game (iOS/Android).
- Goal: analyze, document, preserve, reconstruct open source.

## 📁 Structure
- `AndroidManifest.xml` - Android app manifest
- `assets/` - Original game assets
  - `adapters.config`
  - `dataModel.db` - Data model database
  - `keyValue.db` - Key-value database
  - `bin/Data/`
- `extracted_assets/`
  - `AudioClip/` - Audio (.wav)
  - `Shader/` - Game shaders
  - `TextAsset/` - Text (.txt)
  - `Texture2D/` - Textures/images (.png)
- `AssetRipper/ExportedProject/Assets/Scripts/` - Extracted C# game logic
- `lib/` - Native libraries (`armeabi/`, `armeabi-v7a/`, `x86/`)
- `res/` - Android resources (`drawable/`, `layout/`, `raw/`)

## 🔧 Tech
- Engine: Unity (`libunity.so`, `libmono.so`)
- Platform: Android
- Arch: Multi-arch (ARM, x86)
- Audio: Kamcord
- DB: SQLite

## 📊 Content
- Audio: 3 clips
- Textures: 21 2D textures
- Shaders: 6 custom shaders
- Text: 23 config/data files
- Scripts: C# source code (combat logic, AI, abilities)

## 🎯 Goals
- [x] Extract assets
- [x] Analyze mechanics
- [x] Document systems
- [x] Document DB schema
- [x] Complete re-extraction
- [x] Reverse engineer gameplay (C# scripts recovered)
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
- Educational/research only.
- Assets owned by respective owners.

## 🤝 Contribute
- Open issues/PRs to improve docs, add analysis, fix errors.

*Branch: `reverse-engineering`*
