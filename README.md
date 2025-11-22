# Open Battle Tactics 🛡️🎲

> An open-source recreation and tactical engine inspired by the classic **Super Battle Tactics**.

![Phaser 3](https://img.shields.io/badge/Phaser-3.90-green.svg) ![Status](https://img.shields.io/badge/Status-Prototype-orange.svg) ![License](https://img.shields.io/badge/License-MIT-blue.svg)

## 📖 Overview

**Open Battle Tactics** is a turn-based strategy game where you command a squad of tanks. The twist? Your damage and initiative are determined by spinning wheels (dice rolls) each turn. You must use Action Points (AP) wisely to boost your tanks' rolls or activate special abilities to turn the tide of battle.

This project aims to preserve the gameplay mechanics of the original game while providing a modern, web-based engine for tactical battles.

## ✨ Features

- **Tactical Combat**: 4v4 Tank battles with positioning mechanics.
- **RNG Management**: "Risk vs Reward" gameplay. Roll for damage, then spend AP to boost low rolls.
- **Data-Driven**: Units and stats are loaded from extracted JSON data (based on the original game database).
- **Web Tech**: Built with [Phaser 3](https://phaser.io/) and [Vite](https://vitejs.dev/).

## 🚀 Getting Started

### Prerequisites

- **Node.js** (v16 or higher)
- **npm**

### Installation

1. **Clone the repository:**

    ```bash
    git clone https://github.com/maelemiel/open-battle-tactics.git
    cd open-battle-tactics
    ```

2. **Install dependencies:**

    ```bash
    npm install
    ```

3. **Run the development server:**

    ```bash
    npm run dev
    ```

4. Open your browser at `http://localhost:5173` (or the URL shown in the terminal).

## 🎮 How to Play

1. **Deployment Phase**:
    - The game automatically rolls the dice for all tanks at the start of the turn.
    - You have **3 Action Points (AP)**.
    - **Click on a Tank** to **Boost** it (Spending 1 AP).
    - Boosting sets the tank's roll to its Maximum potential + 2 (Critical Hit!).

2. **Combat Phase**:
    - Click the **FIGHT** button when you are ready.
    - Tanks attack in order of Initiative (Highest Roll goes first).
    - Damage deals direct HP loss. If HP reaches 0, the tank is destroyed.

3. **Victory**:
    - Destroy all enemy tanks to win!

## 🛠️ Project Structure

``` tree
src/
├── classes/        # Game Objects (Tank.js)
├── config/         # Game Configuration & JSON Data
├── logic/          # Core Game Logic (BattleEngine.js)
├── scenes/         # Phaser Scenes (BattleScene.js)
├── ui/             # User Interface (UIManager.js)
└── main.js         # Entry point & Phaser Config
```

## 🔮 Roadmap

- [ ] **Abilities System**: Implement active skills (Shield, Nuke, Heal).
- [ ] **Team Builder**: Allow players to select their squad before battle.
- [ ] **Animations**: Add projectile sprites and more impact effects.
- [ ] **Multiplayer**: PvP mode using WebSockets.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

*Note: This is a fan project. All original game assets and IP rights belong to their respective owners. This project is for educational and preservation purposes only.*
