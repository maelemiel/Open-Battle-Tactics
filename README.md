# Open Battle Tactics 🛡️🎲

> **An open-source recreation of Super Battle Tactics** — the beloved turn-based mobile game by DeNA that was shut down. The complete game database has been reverse engineered. We're rebuilding it. Join us.

<div align="center">

![Status](https://img.shields.io/badge/Status-In%20Development-orange.svg)
![Phaser 3](https://img.shields.io/badge/Engine-Phaser%203-green.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![Units](https://img.shields.io/badge/Units-370%20extracted-blue.svg)
![Abilities](https://img.shields.io/badge/Abilities-34%20extracted-purple.svg)
![DB Tables](https://img.shields.io/badge/DB%20Tables-67-teal.svg)

</div>

---

## 🎮 About Super Battle Tactics

Super Battle Tactics was a **4v4 turn-based tank strategy game** developed by DeNA Vancouver for iOS and Android. Players built squads of tanks (Assault, Command, Operative, Helicopter), equipped abilities, and battled other players in ranked PvP.

The servers shut down years ago — but the community never forgot it. This project exists to bring it back, open source, for everyone.

> *"Super battle tactics was a hell of a unique game"* — r/gamedev  
> *"Nostalgia just hit right now"* — r/MobileGaming

---

## ✅ What's Already Done

The complete game data has been **reverse engineered from the original APK**:

| Resource | Count | Status |
|---|---|---|
| Game Units | **370** | ✅ Complete |
| Abilities | **34** | ✅ Complete |
| Unit Types | **14** | ✅ Complete |
| Rarity Levels | **5** | ✅ Complete |
| Database Tables | **67** | ✅ Complete |
| Languages | **14** | ✅ Complete |
| Gacha System | 7 tables | ✅ Complete |
| AI System | 3 tables | ✅ Complete |
| Progression System | Full | ✅ Complete |
| Combat Mechanics | All phases | ✅ Documented |
| Textures / Sprites | Partial | ⚠️ Need help |
| Audio / Music | — | ❌ Need help |

The full database schema, all unit stats, ability descriptions in 14 languages, combat phases, progression tiers, leaderboard system, events and gacha tables are all documented and ready to use.

---

## 🚀 Prototype (Playable Now)

A basic combat prototype is already running in the browser using **Phaser 3**:

### Run it locally

```bash
git clone https://github.com/maelemiel/Open-Battle-Tactics.git
cd Open-Battle-Tactics
npm install
npm run dev
# Open http://localhost:5173
```

### How it works (current prototype)

1. **Deployment Phase** — dice are rolled for all tanks. You have **3 Action Points (AP)**.
2. Click a tank to **Boost** it (costs 1 AP) → sets its roll to max + 2 (Critical!)
3. Click **FIGHT** → tanks attack in initiative order (highest roll first)
4. Destroy all enemy tanks to win

---

## 🗺️ Roadmap

- [x] APK reverse engineering — full database extracted
- [x] Combat prototype (Phaser 3 + Vite)
- [x] Data-driven units loaded from extracted JSON
- [ ] **Ability system** — all 4 types (ULTRA, PRE-COMBAT, REACTIVE, PASSIVE)
- [ ] **Team builder** — pick your 4-unit squad before battle
- [ ] **All 370 units** playable with correct stats
- [ ] **Unit sprites** — pixel art or recreated from originals
- [ ] **PvP multiplayer** via WebSockets
- [ ] **Progression system** — tiers, divisions, level-up
- [ ] **Ranked leaderboard**
- [ ] **Event system**
- [ ] **Mobile-ready** (Android / iOS)

---

## 📁 Repository Structure

```
Open-Battle-Tactics/
├── src/                          # Game source (Phaser 3 + Vite)
│   ├── classes/                  # Tank.js and game objects
│   ├── logic/                    # BattleEngine.js — combat phases
│   ├── scenes/                   # BattleScene.js
│   └── ui/                       # UIManager.js
│
└── [branch: reverse-engineering] # All extracted game data
    ├── assets/
    │   ├── dataModel.db          # SQLite — 67 tables, 370 units
    │   └── bin/Data/             # Unity assets (99 MB)
    ├── extracted_assets/         # PNGs, WAVs, shaders
    ├── REVERSE_ENGINEERING_SUMMARY.md
    ├── DATABASE_SCHEMA.md
    ├── DATA_EXTRACTION.md
    └── QUICKSTART.md
```

> 💡 All game data lives on the [`reverse-engineering`](https://github.com/maelemiel/Open-Battle-Tactics/tree/reverse-engineering) branch.

---

## 🤝 How to Contribute

**You don't need to be a developer to help.** Here's what the project needs:

### 🎨 Artists
- Recreate unit sprites (tanks, helicopters) in pixel art or vector
- UI elements, ability icons, background art
- Reference artwork: [Behance](https://www.behance.net/gallery/70749049/Super-Battle-Tactics-Game-Assets) · [ArtStation](https://www.artstation.com/artwork/L4BmdR)

### 💻 Developers
- Implement the ability system (34 abilities, 4 types)
- Build the team selection UI
- WebSocket multiplayer
- Mobile export (Capacitor or React Native)

### 🎮 Former Players
- Share screenshots, videos, or memories of the game
- Help verify unit stats and ability behavior
- Test the prototype and report bugs

### 📖 Documentation
- Improve translations (14 languages supported)
- Write wiki pages for units and abilities

**→ Check [open issues](https://github.com/maelemiel/Open-Battle-Tactics/issues) for tasks labeled `good first issue` and `help wanted`.**

---

## 📚 Documentation

| Document | Description |
|---|---|
| [REVERSE_ENGINEERING_SUMMARY.md](https://github.com/maelemiel/Open-Battle-Tactics/blob/reverse-engineering/REVERSE_ENGINEERING_SUMMARY.md) | Full analysis of the original game (FR) |
| [DATABASE_SCHEMA.md](https://github.com/maelemiel/Open-Battle-Tactics/blob/reverse-engineering/DATABASE_SCHEMA.md) | All 67 tables documented with schemas |
| [QUICKSTART.md](https://github.com/maelemiel/Open-Battle-Tactics/blob/reverse-engineering/QUICKSTART.md) | Guide to rebuild with Unity or Godot |
| [DATA_EXTRACTION.md](https://github.com/maelemiel/Open-Battle-Tactics/blob/reverse-engineering/DATA_EXTRACTION.md) | How to extract more Unity assets |

---

## 🔗 External Resources

- [Super Battle Tactics Wiki (Fandom)](https://super-battle-tactics.fandom.com/wiki/Super_Battle_Tactics_Wiki)
- [MobyGames Page](https://www.mobygames.com/game/158792/super-battle-tactics/)
- [Original Artwork on Behance](https://www.behance.net/gallery/70749049/Super-Battle-Tactics-Game-Assets)
- [ArtStation — Unit Art](https://www.artstation.com/artwork/L4BmdR)

---

## ⚖️ Legal

This is a **fan preservation project**. The original game IP belongs to DeNA. Original assets are copyrighted — this project recreates game mechanics and data structures only. New artwork must be original. This project is not affiliated with DeNA.

---

<div align="center">

**Do you remember Super Battle Tactics? Help us bring it back.**  
⭐ Star the repo · 🐛 Open an issue · 🔀 Submit a PR

</div>
