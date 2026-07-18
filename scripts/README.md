# Open-Battle-Tactics — Export Scripts

Use on `reverse-engineering` branch. Original game data here.

```bash
git checkout reverse-engineering
cd scripts/
```

## 1. `export_game_data.py`

Export SQLite to JSON/CSV. No external dependencies. Uses Python stdlib.

```bash
# Minimal export (EN, JSON)
python scripts/export_game_data.py

# With options
python scripts/export_game_data.py --db assets/dataModel.db --out game_data/ --lang fr
python scripts/export_game_data.py --db assets/dataModel.db --out game_data/ --lang en --format json+csv
```

### Options
* `--db`: SQLite DB path. Default: `assets/dataModel.db`
* `--out`: Output dir. Default: `game_data/`
* `--lang`: Language (`en`, `fr`, `de`, `es`, `it`, `pt`, `zh`). Default: `en`
* `--format`: Export format (`json`, `csv`, `json+csv`). Default: `json`

### Exported Data
```text
game_data/
├── units.json                    # 370 units, translated names
├── unit_types.json               # 14 unit types
├── unit_rarity.json              # 5 rarity levels
├── unit_progression/             # level-up, cooldowns, scrap values
│   ├── unit_level_progression.json
│   ├── unit_cooldown.json
│   └── ...
├── abilities.json                # 34 abilities (ULTRA/PRE-COMBAT/REACTIVE/PASSIVE)
├── events.json                   # In-game events
├── items.json
├── gacha/                        # Gacha tables
│   ├── gacha_pools.json
│   └── ...
├── progression/                  # Divisions, tiers, promotion series
├── leaderboards/
├── ai/                           # AI armies and behaviors
│   ├── ai_army.json
│   ├── ai_army_parts.json
│   └── ai_handler.json
├── config.json                   # General game config
├── localization_full.json        # Translations (14 languages)
└── tables_index.json             # Index of 67 tables, row counts
```

## 2. `extract_unity_assets.py`

Extract Unity assets (textures, sounds, text) from `assets/bin/Data/`.

```bash
# Install dependencies
pip install UnityPy Pillow

# Extract all
python scripts/extract_unity_assets.py --src assets/bin/Data/ --out extracted/ --verbose

# Extract specific types
python scripts/extract_unity_assets.py --src assets/bin/Data/ --out extracted/ --types Texture2D,AudioClip --verbose
```

### Options
* `--src`: Unity source dir. Default: `assets/bin/Data/`
* `--out`: Output dir. Default: `extracted/`
* `--types`: Comma-separated filter. Default: (all)
* `--verbose`: Show per-file output. Default: false

### Supported Types
* `Texture2D`: `extracted/Texture2D/*.png` — Unit/UI textures
* `Sprite`: `extracted/Sprite/*.png` — Sliced sprites
* `AudioClip`: `extracted/AudioClip/*.wav` — Game sounds
* `TextAsset`: `extracted/TextAsset/*.json` — Configs/data
* `Shader`: `extracted/Shader/*.shader` — GLSL/HLSL shaders

## Workflow

```bash
# 1. Switch branch
git checkout reverse-engineering

# 2. Export JSON
python scripts/export_game_data.py --db assets/dataModel.db --out game_data/ --lang en

# 3. Extract Unity assets
pip install UnityPy Pillow
python scripts/extract_unity_assets.py --src assets/bin/Data/ --out extracted/ --types Texture2D,Sprite,AudioClip --verbose

# 4. Copy JSONs to main branch
git stash
git checkout main
mkdir -p src/config
cp game_data/units.json src/config/
cp game_data/abilities.json src/config/
```
