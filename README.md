# Open Battle Tactics

> **[📊 Quick Summary](./SUMMARY.md)** - Complete project overview with statistics and quick links

## 📋 Description

This project contains extracted assets and resources from the mobile game **Super Battle Tactics**, a turn-based strategy game. This is a reverse engineering project aimed at analyzing and documenting the game's components for preservation and potential open source reconstruction.

## 🎮 About the Game

Super Battle Tactics is a mobile strategy game developed for iOS and Android. The game features turn-based tactical combat with strategic gameplay mechanics.

## 📁 Project Structure

```text
Open-Battle-Tactics/
├── AndroidManifest.xml          # Android application manifest
├── assets/                      # Original game assets
│   ├── adapters.config
│   ├── dataModel.db            # Data model database
│   ├── keyValue.db             # Key-value database
│   └── bin/Data/
├── extracted_assets/            # Extracted and organized assets
│   ├── AudioClip/              # Audio files (.wav)
│   ├── Shader/                 # Game shaders
│   ├── TextAsset/              # Text assets (.txt)
│   └── Texture2D/              # Textures and images (.png)
├── lib/                        # Native libraries
│   ├── armeabi/
│   ├── armeabi-v7a/
│   └── x86/
└── res/                        # Android resources
    ├── drawable/
    ├── layout/
    └── raw/
```

## 🔧 Identified Technologies

- **Engine**: Unity (based on presence of `libunity.so` and `libmono.so`)
- **Platform**: Android
- **Architecture**: Multi-architecture support (ARM, x86)
- **Audio**: Kamcord integration for recording
- **Database**: SQLite

## 📊 Extracted Content

- **Audio**: 3 audio clips
- **Textures**: 21 2D textures
- **Shaders**: 6 custom shaders
- **Text assets**: 23 configuration/data files

## 🎯 Project Goals

- [x] Game asset extraction
- [x] Game mechanics analysis
- [x] System documentation
- [x] Database schema documentation
- [ ] Complete asset re-extraction
- [ ] Gameplay reverse engineering
- [ ] Open source reconstruction

## 📖 Documentation

### For Developers

**[🚀 QUICK START GUIDE](./QUICKSTART.md)** - Get started rebuilding the game
- Project setup (Unity/Godot)
- Data extraction and loading
- Core system implementation (combat, units, abilities)
- Sample code and examples
- Step-by-step tutorial

**[📦 DATA EXTRACTION GUIDE](./DATA_EXTRACTION.md)** - Extract more data from the APK
- Unity asset extraction tools (AssetStudio, UnityPy)
- SQLite database export to JSON
- Python scripts for automated extraction
- Troubleshooting common issues

**[📊 DATABASE SCHEMA REFERENCE](./DATABASE_SCHEMA.md)** - Complete database documentation
- All 67 tables explained
- Schema definitions
- Sample queries
- Quick stats and extraction tips

### For Analysis

**[📋 REVERSE ENGINEERING SUMMARY (Français)](./REVERSE_ENGINEERING_SUMMARY.md)** - Comprehensive analysis
- Complete database schema (67 tables, 370 units, 34 abilities)
- Game mechanics breakdown (combat system, progression, gacha, events)
- Technical architecture
- Recommendations for open source rebuild
- Development roadmap
- Available resources and external references

## 📚 Resources and References

### Official Documentation

- [Super Battle Tactics Wiki](https://super-battle-tactics.fandom.com/wiki/Super_Battle_Tactics_Wiki)
- [MobyGames Page](https://www.mobygames.com/game/158792/super-battle-tactics/)

### Assets and Artwork

- [Game Assets on Behance](https://www.behance.net/gallery/70749049/Super-Battle-Tactics-Game-Assets)
- [iOS/Android Gallery](https://www.behance.net/gallery/122971387/SUPER-BATTLE-TACTICS-(iOSAndroid))
- [Artwork on ArtStation](https://www.artstation.com/artwork/L4BmdR)
- [Additional Artwork 1](https://www.artstation.com/artwork/meWkZ)
- [Additional Artwork 2](https://www.artstation.com/artwork/9KDXQ)

## ⚖️ License

This project is intended for educational and research purposes only. All assets belong to their respective owners.

## 🤝 Contributing

Contributions are welcome! Feel free to open an issue or pull request to:

- Improve documentation
- Add analysis
- Fix errors

---

*Reverse engineering project - Branch: `reverse-engineering`*
