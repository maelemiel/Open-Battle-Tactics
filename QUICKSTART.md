# Quick Start Guide - Rebuilding Super Battle Tactics

## 🚀 Getting Started

This guide will help you get started with rebuilding Super Battle Tactics as an open source project.

## Prerequisites

- Basic knowledge of Unity or Godot
- SQLite database experience
- Game development fundamentals
- Git and GitHub knowledge

## Step 1: Set Up Your Development Environment

### Option A: Unity (Recommended for compatibility)
```bash
# Download Unity Hub
# Install Unity 2021.3 LTS or newer
# Install required modules:
# - Android Build Support
# - iOS Build Support (optional)
```

### Option B: Godot (Open source alternative)
```bash
# Download Godot 4.x
# https://godotengine.org/download
```

## Step 2: Clone This Repository

```bash
git clone https://github.com/maelemiel/Open-Battle-Tactics.git
cd Open-Battle-Tactics
```

## Step 3: Explore the Data

### Access the SQLite Database
```bash
# Install SQLite (if not already installed)
# On Ubuntu/Debian:
sudo apt-get install sqlite3

# On macOS:
brew install sqlite3

# On Windows: Download from https://sqlite.org/download.html

# Open the database
cd assets
sqlite3 dataModel.db

# Explore tables
.tables
.schema unit
.schema ability

# Query sample data
SELECT * FROM unit LIMIT 5;
SELECT * FROM ability LIMIT 5;
```

### Export Data to JSON
```bash
# Install sqlite-utils
pip install sqlite-utils

# Export units with names
sqlite-utils dataModel.db \
  "SELECT u.*, l.en_us as name FROM unit u 
   JOIN localization l ON u.key_name = l.key" \
  --json > ../game_data/units.json

# Export abilities
sqlite-utils dataModel.db \
  "SELECT a.*, ln.en_us as name, ld.en_us as description 
   FROM ability a 
   JOIN localization ln ON a.key_name = ln.key 
   JOIN localization ld ON a.key_description = ld.key" \
  --json > ../game_data/abilities.json
```

## Step 4: Create a New Game Project

### Unity Project Setup
```bash
# Create new Unity project
# Project Type: 2D or 3D (depending on your vision)
# Template: Empty or 2D

# Project Structure:
MyProject/
├── Assets/
│   ├── Scripts/
│   │   ├── Units/
│   │   ├── Abilities/
│   │   ├── Combat/
│   │   ├── UI/
│   │   └── Data/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Resources/
│   │   └── GameData/  # Put exported JSON files here
│   └── Sprites/
```

### Godot Project Setup
```
project/
├── scenes/
│   ├── combat/
│   ├── menu/
│   └── ui/
├── scripts/
│   ├── units/
│   ├── abilities/
│   └── systems/
├── resources/
│   └── game_data/  # Put exported JSON files here
└── assets/
    └── sprites/
```

## Step 5: Implement Core Systems

### 5.1 Data Structures (C# for Unity)

```csharp
// Unit.cs
[System.Serializable]
public class Unit {
    public int id;
    public string name;
    public UnitType type;
    public int rarity;
    public int health;
    public int damage;
    public int initiative;
    
    // Stats
    public UnitStats stats;
    
    // Abilities
    public List<Ability> abilities;
}

public enum UnitType {
    Assault = 1,
    Command = 2,
    Operative = 3,
    Helicopter = 4
}

[System.Serializable]
public class UnitStats {
    public int attack;
    public int defense;
    public int speed;
    public int range;
}

// Ability.cs
[System.Serializable]
public class Ability {
    public int id;
    public string name;
    public string description;
    public AbilityType type;
    public int actionPoints;
    public int targetGroup;
    
    // Effects
    public List<AbilityEffect> effects;
}

public enum AbilityType {
    Ultra = 1,
    PreCombat = 2,
    Reactive = 3,
    Passive = 4
}
```

### 5.2 Data Loader

```csharp
// GameDataLoader.cs
using UnityEngine;
using System.Collections.Generic;

public class GameDataLoader : MonoBehaviour {
    public static GameDataLoader Instance;
    
    private Dictionary<int, Unit> units;
    private Dictionary<int, Ability> abilities;
    
    void Awake() {
        Instance = this;
        LoadGameData();
    }
    
    void LoadGameData() {
        // Load units from JSON
        TextAsset unitsJson = Resources.Load<TextAsset>("GameData/units");
        units = JsonUtility.FromJson<List<Unit>>(unitsJson.text)
            .ToDictionary(u => u.id);
        
        // Load abilities
        TextAsset abilitiesJson = Resources.Load<TextAsset>("GameData/abilities");
        abilities = JsonUtility.FromJson<List<Ability>>(abilitiesJson.text)
            .ToDictionary(a => a.id);
    }
    
    public Unit GetUnit(int id) => units.GetValueOrDefault(id);
    public Ability GetAbility(int id) => abilities.GetValueOrDefault(id);
}
```

### 5.3 Combat System (Simplified)

```csharp
// CombatManager.cs
public class CombatManager : MonoBehaviour {
    public List<Unit> playerUnits;
    public List<Unit> enemyUnits;
    
    private CombatPhase currentPhase;
    
    enum CombatPhase {
        FirstStrike,
        UltraAbilities,
        PreCombat,
        PlayerAttack,
        EnemyAttack,
        Reactive,
        EndTurn
    }
    
    public void StartCombat() {
        // Sort units by initiative
        playerUnits = playerUnits.OrderByDescending(u => u.stats.speed).ToList();
        enemyUnits = enemyUnits.OrderByDescending(u => u.stats.speed).ToList();
        
        StartCoroutine(CombatLoop());
    }
    
    IEnumerator CombatLoop() {
        while (!IsCombatOver()) {
            // First Strike Phase
            currentPhase = CombatPhase.FirstStrike;
            yield return ProcessFirstStrike();
            
            // Ultra Abilities Phase
            currentPhase = CombatPhase.UltraAbilities;
            yield return ProcessUltraAbilities();
            
            // Pre-Combat Abilities
            currentPhase = CombatPhase.PreCombat;
            yield return ProcessPreCombatAbilities();
            
            // Player Attack
            currentPhase = CombatPhase.PlayerAttack;
            yield return ProcessAttacks(playerUnits, enemyUnits);
            
            // Enemy Attack
            currentPhase = CombatPhase.EnemyAttack;
            yield return ProcessAttacks(enemyUnits, playerUnits);
            
            // Reactive Abilities
            currentPhase = CombatPhase.Reactive;
            yield return ProcessReactiveAbilities();
            
            // End Turn
            currentPhase = CombatPhase.EndTurn;
            yield return new WaitForSeconds(1f);
        }
        
        EndCombat();
    }
    
    IEnumerator ProcessAttacks(List<Unit> attackers, List<Unit> defenders) {
        foreach (Unit attacker in attackers) {
            if (attacker.health <= 0) continue;
            
            Unit target = SelectTarget(defenders);
            if (target != null) {
                int damage = CalculateDamage(attacker, target);
                target.health -= damage;
                
                // Visual feedback
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    int CalculateDamage(Unit attacker, Unit defender) {
        // Simplified damage calculation
        int baseDamage = attacker.stats.attack;
        int defense = defender.stats.defense;
        return Mathf.Max(1, baseDamage - defense / 2);
    }
    
    Unit SelectTarget(List<Unit> targets) {
        // Simple: target first alive unit
        return targets.FirstOrDefault(u => u.health > 0);
    }
    
    bool IsCombatOver() {
        return playerUnits.All(u => u.health <= 0) || 
               enemyUnits.All(u => u.health <= 0);
    }
}
```

## Step 6: Create Minimal UI

### Battle UI (Unity UI Toolkit or uGUI)
```csharp
// BattleUI.cs
public class BattleUI : MonoBehaviour {
    public Transform playerUnitContainer;
    public Transform enemyUnitContainer;
    public GameObject unitCardPrefab;
    
    public void DisplayUnits(List<Unit> playerUnits, List<Unit> enemyUnits) {
        // Clear existing
        foreach (Transform child in playerUnitContainer) {
            Destroy(child.gameObject);
        }
        
        // Spawn player units
        foreach (Unit unit in playerUnits) {
            GameObject card = Instantiate(unitCardPrefab, playerUnitContainer);
            card.GetComponent<UnitCard>().Setup(unit);
        }
        
        // Spawn enemy units
        foreach (Unit unit in enemyUnits) {
            GameObject card = Instantiate(unitCardPrefab, enemyUnitContainer);
            card.GetComponent<UnitCard>().Setup(unit);
        }
    }
}

// UnitCard.cs
public class UnitCard : MonoBehaviour {
    public Text nameText;
    public Text healthText;
    public Image icon;
    
    private Unit unit;
    
    public void Setup(Unit unit) {
        this.unit = unit;
        nameText.text = unit.name;
        UpdateHealth();
    }
    
    public void UpdateHealth() {
        healthText.text = $"HP: {unit.health}";
    }
}
```

## Step 7: Test Your Prototype

```csharp
// TestBattle.cs
public class TestBattle : MonoBehaviour {
    void Start() {
        // Create test units
        Unit volt = GameDataLoader.Instance.GetUnit(11001); // VOLT
        Unit longshot = GameDataLoader.Instance.GetUnit(11002); // LONGSHOT
        
        // Set up battle
        CombatManager combat = GetComponent<CombatManager>();
        combat.playerUnits = new List<Unit> { volt };
        combat.enemyUnits = new List<Unit> { longshot };
        
        // Start
        combat.StartCombat();
    }
}
```

## Step 8: Iterate and Expand

### Phase 1: Core Gameplay (Current)
- [x] Load unit data
- [x] Basic combat system
- [x] Simple UI
- [ ] Implement all ability types
- [ ] Add visual effects
- [ ] Polish combat flow

### Phase 2: Content
- [ ] Import all 370 units
- [ ] Implement 34 abilities
- [ ] Create unit sprites/models
- [ ] Add sound effects
- [ ] Campaign/mission system

### Phase 3: Progression
- [ ] Unit unlocking
- [ ] Research system
- [ ] Player progression
- [ ] Economy system

### Phase 4: Multiplayer
- [ ] PvP matchmaking
- [ ] Leaderboards
- [ ] Events

## Resources

### Documentation
- [REVERSE_ENGINEERING_SUMMARY.md](./REVERSE_ENGINEERING_SUMMARY.md) - Complete analysis (French)
- [DATABASE_SCHEMA.md](./DATABASE_SCHEMA.md) - Database reference

### External Resources
- [Unity Learn](https://learn.unity.com/) - Unity tutorials
- [Godot Docs](https://docs.godotengine.org/) - Godot documentation
- [SQLite Tutorial](https://www.sqlitetutorial.net/) - SQLite guide

### Community
- Create a Discord server for contributors
- Document your progress on GitHub
- Share screenshots and demos

## Tips

1. **Start Small**: Begin with 5-10 units, not all 370
2. **Iterate Fast**: Get something playable quickly, then improve
3. **Version Control**: Commit often, use branches
4. **Test Frequently**: Test every feature as you build it
5. **Ask for Help**: Join game dev communities if stuck
6. **Legal**: Create original art, don't use extracted assets
7. **Document**: Write docs as you code

## Common Pitfalls

❌ **Don't**: Try to implement everything at once  
✅ **Do**: Build incrementally, test each system

❌ **Don't**: Use extracted game assets in production  
✅ **Do**: Create original artwork or use open source assets

❌ **Don't**: Perfectly replicate the original  
✅ **Do**: Take inspiration and make it your own

❌ **Don't**: Work alone for too long  
✅ **Do**: Share progress, get feedback early

## Next Steps

1. Choose your engine (Unity or Godot)
2. Set up the project
3. Export database to JSON
4. Implement basic data structures
5. Create a simple combat prototype with 2-3 units
6. Test and iterate
7. Share your progress!

## License Note

Remember: This is an **educational** project for **game preservation**. The original game is discontinued. For any public release:

- Create **original assets** (art, music, sound)
- Give credit where due
- Use an open source license (MIT, GPL-3.0)
- Consider renaming to avoid trademark issues
- Respect the original creators' work

---

**Good luck rebuilding Super Battle Tactics! 🎮**

For questions or contributions, open an issue on GitHub.
