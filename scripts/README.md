# Open-Battle-Tactics — Scripts d'export

Ces scripts s'utilisent depuis la branche `reverse-engineering`, là où se trouvent les données du jeu original.

```bash
git checkout reverse-engineering
cd scripts/
```

---

## 1. `export_game_data.py` — Export SQLite → JSON/CSV

Exporte toutes les données du jeu depuis `dataModel.db` en fichiers JSON (et optionnellement CSV). Aucune dépendance externe — stdlib Python uniquement.

```bash
# Export minimal (EN, JSON)
python scripts/export_game_data.py

# Avec options
python scripts/export_game_data.py --db assets/dataModel.db --out game_data/ --lang fr
python scripts/export_game_data.py --db assets/dataModel.db --out game_data/ --lang en --format json+csv
```

### Options

| Option | Défaut | Description |
|--------|--------|-------------|
| `--db` | `assets/dataModel.db` | Chemin vers la base SQLite |
| `--out` | `game_data/` | Dossier de sortie |
| `--lang` | `en` | Langue : `en`, `fr`, `de`, `es`, `it`, `pt`, `zh` |
| `--format` | `json` | Format : `json`, `csv`, `json+csv` |

### Données exportées

```
game_data/
├── units.json                    # 370 unités avec noms traduits
├── unit_types.json               # 14 types d'unités
├── unit_rarity.json              # 5 niveaux de rareté
├── unit_progression/             # level-up, cooldowns, scrap values...
│   ├── unit_level_progression.json
│   ├── unit_cooldown.json
│   └── ...
├── abilities.json                # 34 capacités (ULTRA/PRE-COMBAT/REACTIVE/PASSIVE)
├── events.json                   # Événements en jeu
├── items.json
├── gacha/                        # Toutes les tables gacha
│   ├── gacha_pools.json
│   └── ...
├── progression/                  # Divisions, tiers, promotion series
├── leaderboards/
├── ai/                           # Armées IA et comportements
│   ├── ai_army.json
│   ├── ai_army_parts.json
│   └── ai_handler.json
├── config.json                   # Configuration générale du jeu
├── localization_full.json        # Toutes les traductions (14 langues)
└── tables_index.json             # Index des 67 tables avec nb de lignes
```

---

## 2. `extract_unity_assets.py` — Extraction assets Unity

Extrait les textures, sons et fichiers texte depuis `assets/bin/Data/` (format Unity).

```bash
# Installation des dépendances
pip install UnityPy Pillow

# Extraire tout
python scripts/extract_unity_assets.py --src assets/bin/Data/ --out extracted/ --verbose

# Extraire uniquement les textures et l'audio
python scripts/extract_unity_assets.py --src assets/bin/Data/ --out extracted/ --types Texture2D,AudioClip --verbose
```

### Options

| Option | Défaut | Description |
|--------|--------|-------------|
| `--src` | `assets/bin/Data/` | Dossier source Unity |
| `--out` | `extracted/` | Dossier de sortie |
| `--types` | (tous) | Types filtrés séparés par virgule |
| `--verbose` | false | Affichage fichier par fichier |

### Types supportés

| Type Unity | Sortie | Description |
|---|---|---|
| `Texture2D` | `extracted/Texture2D/*.png` | Textures des unités, UI |
| `Sprite` | `extracted/Sprite/*.png` | Sprites découpés |
| `AudioClip` | `extracted/AudioClip/*.wav` | Sons du jeu |
| `TextAsset` | `extracted/TextAsset/*.json` | Configs et données |
| `Shader` | `extracted/Shader/*.shader` | Shaders GLSL/HLSL |

---

## Workflow complet

```bash
# 1. Se placer sur la bonne branche
git checkout reverse-engineering

# 2. Exporter les données JSON (pour les dev)
python scripts/export_game_data.py --db assets/dataModel.db --out game_data/ --lang en

# 3. Extraire les assets Unity (pour les artistes)
pip install UnityPy Pillow
python scripts/extract_unity_assets.py --src assets/bin/Data/ --out extracted/ --types Texture2D,Sprite,AudioClip --verbose

# 4. Copier les JSONs vers la branche main pour le dev
git stash
git checkout main
mkdir -p src/config
cp game_data/units.json src/config/
cp game_data/abilities.json src/config/
```
