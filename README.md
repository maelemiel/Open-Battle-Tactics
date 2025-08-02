# 🎮 Open Battle Tactics

**An open-source recreation of Super Battle Tactics - Relive the nostalgic turn-based tank combat experience!**

## 🎯 About the Project

**Open Battle Tactics** is a faithful open-source recreation of the beloved mobile game **Super Battle Tactics** that captivated an entire generation of gamers. Our mission is to preserve this nostalgic experience while enhancing it through community contributions and modern web technologies.

### 🎲 Why This Project?

- **🎮 Nostalgia**: Relive the magical moments of the original game
- **🛡️ Preservation**: Save this iconic game for future generations  
- **🚀 Innovation**: Enhance the experience with new features and improvements
- **👥 Community**: Unite fans around a collaborative project
- **📚 Learning**: Learn game development through open-source collaboration
- **🌐 Accessibility**: Make the game playable on modern devices and browsers

### 🏆 Project Goals

- Create a pixel-perfect recreation of the original Super Battle Tactics gameplay
- Build a thriving community of contributors and players
- Document the reverse engineering process for educational purposes
- Develop a modular, extensible codebase for future enhancements
- Preserve the game's assets and mechanics for historical reference

## ✨ Features

- ✅ **Authentic 4v4 Turn-Based Combat** with original mechanics
- ✅ **Dice System** with first-strike mechanics (blue bar values 15-20)
- ✅ **4 Tank Types**: Assault, Defense, Speed, Hover with unique characteristics
- ✅ **Special Abilities**: Re-Spin, Targeting, Mini Strike, Bombard
- ✅ **Tank TV Interface** faithful to the cartoon aesthetic
- ✅ **Local Multiplayer** for challenging friends
- ✅ **Statistics Tracking** with win/loss records
- ✅ **Responsive Design** for desktop and mobile browsers

### 🚧 In Development
- 🔄 **Online Multiplayer** with matchmaking system
- 🔄 **Progressive Tank Unlocking** and customization
- 🔄 **Campaign Mode** with challenging AI opponents
- 🔄 **Community Tank Editor** for user-generated content
- 🔄 **Native Mobile Apps** for Android and iOS

### 🎯 Planned Features
- 📋 **Guild System** with community events
- 📋 **Mod Support** through workshop integration
- 📋 **Advanced AI** with multiple difficulty levels
- 📋 **Tournament System** with global leaderboards
- 📋 **Achievement System** with unlockable rewards

## 🎮 How to Play

### 🎯 Basic Rules
1. **Formation**: Each team deploys 4 tanks with different roles
2. **Turn-based**: Players activate abilities then roll dice for combat
3. **First Strike**: Tanks with blue bar values (15-20) attack first
4. **Victory**: Eliminate all enemy tanks to win!

### 🎲 Tank Types
| Type | HP | Damage | Specialty |
|------|----|---------|-----------| 
| **🔴 Assault** | Medium | High | Pure damage dealer |
| **🔵 Defense** | High | Low | Tanky survivor |
| **🟢 Speed** | Low | Medium | Frequent first strikes |
| **🟣 Hover** | Medium | Medium | Balanced flying unit |

### ⚡ Special Abilities
- **Re-Spin** (1 pt): Reroll a tank's die
- **Targeting** (1 pt): Target a specific enemy
- **Mini Strike** (2 pts): Immediate bonus attack
- **Bombard** (4 pts): Area attack hitting all enemies

> 💡 **Pro Tip**: Manage your 3 ability points per turn strategically!

---

## �️ Tech Stack

### 🎨 Frontend
- **HTML5 Canvas** - Graphics rendering and animations
- **CSS3** - Modern responsive styling
- **JavaScript ES6+** - Game logic and interactivity
- **Phaser.js** *(optional)* - 2D game framework

### ⚙️ Backend *(for multiplayer)*
- **Node.js** - Game server runtime
- **Socket.io** - Real-time communication
- **Express.js** - REST API framework
- **PostgreSQL** - Database for user data

### 🔧 Development Tools
- **Webpack** - Module bundling and building
- **ESLint** - Code quality and consistency
- **Prettier** - Automatic code formatting
- **Jest** - Unit testing framework
- **GitHub Actions** - CI/CD pipeline

## 📁 Project Structure

```
open-battle-tactics/
├── src/
│   ├── scenes/           # Phaser scenes (Menu, Battle, GameOver)
│   ├── classes/          # Game classes (Tank, Player, AI)
│   ├── assets/           # Images, sounds, data
│   └── utils/            # Utilities and helpers
├── public/
│   ├── manifest.json     # PWA manifest
│   ├── sw.js            # Service worker
│   └── icons/           # PWA icons
├── docs/                # Documentation
└── reverse-engineering/ # RE analysis branch
```

## 🛠️ Development

### Prerequisites

- Node.js 16+ and npm

### Quick Start

```bash
# Clone the repository
git clone https://github.com/maelemiel/Open-Battle-Tactics.git
cd Open-Battle-Tactics

# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

### Scripts

- `npm run dev` - Start development server (port 3000)
- `npm run build` - Build for production
- `npm run preview` - Preview production build

## 🔮 Roadmap

### 🎯 Version 0.1 - MVP

- [ ] Basic 4v4 combat system
- [ ] Tank TV user interface
- [ ] 8 default tanks (4 per team)
- [ ] Essential special abilities
- [ ] Complete automated testing

### 🎯 Version 0.2 - Enhanced Content

- [ ] 12 additional tank variants
- [ ] Solo campaign mode
- [ ] Progression system
- [ ] Mobile web support

### 🎯 Version 0.3 - Multiplayer

- [ ] Online matchmaking
- [ ] Global leaderboards
- [ ] Integrated chat system
- [ ] Match replay system

### 🎯 Version 1.0 - Full Release

- [ ] Native mobile applications
- [ ] Guild system
- [ ] Community tank editor
- [ ] Workshop integration

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🎨 Assets

This project uses placeholder graphics and sounds. You can replace them with your own assets in the `src/assets/` directory.

## 🐛 Bug Reports

Found a bug? Please create an issue with:
- Description of the problem
- Steps to reproduce
- Expected vs actual behavior
- Browser/device information

---

**Live Demo**: [Coming Soon]  
**Documentation**: [docs/README.md](docs/README.md)  
**Author**: [@maelemiel](https://github.com/maelemiel)
