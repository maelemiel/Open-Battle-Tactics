# SBT Data Extraction

## Repo Status
- SQLite: `assets/dataModel.db`, `assets/keyValue.db`
- Unity assets: `extracted_assets/`
- Source APK: `super-battle-tactics.zip`
- Broken: Audio (0 bytes), Shaders (empty)

## Tools
- APK Tool: `brew install apktool` | https://apktool.org/
- AssetStudio (GUI): https://github.com/Perfare/AssetStudio/releases
- UnityPy (Python): `pip install UnityPy`
- UABE: https://github.com/SeriousCache/UABE/releases
- SQLite Browser: https://sqlitebrowser.org/
- sqlite3 CLI: `sqlite3 --version`

## Asset Extraction

### AssetStudio (GUI)
- Load: `File > Load folder` -> `Open-Battle-Tactics/assets/bin/Data/`
- Select objects in Asset List
- Export: `Export > Export selected assets` or `Export > All assets`

### UnityPy (Script)
- Read dir: `assets/bin/Data` -> `UnityPy.load(source_folder)`
- Export loop: `env.objects`
- Format map:
  - `Texture2D`, `Sprite` -> PNG
  - `AudioClip` -> WAV (`samples.items()`)
  - `TextAsset` -> TXT
  - `Shader` -> `.shader`
  - `Mesh` -> `.obj`

## DB Extraction

### sqlite-utils
- Install: `pip install sqlite-utils`
- Full dump: `sqlite-utils dump assets/dataModel.db > database_dump.sql`
- Table JSON: `sqlite-utils assets/dataModel.db "SELECT * FROM unit" --json > units.json`

### Python sqlite3
- `conn.row_factory = sqlite3.Row` for dict conversion
- Tables: `SELECT name FROM sqlite_master WHERE type='table';`
- Loop, `json.dump()`

### Complex Query Example (Units)
- Target: `assets/dataModel.db`
- Tables: `unit` JOIN `localization` (name), `unit_type` (type id)

## APK / Java Extraction
- Decompile APK: `apktool d super-battle-tactics.zip -o decompiled/`
- DEX to JAR: `d2j-dex2jar classes.dex` (https://github.com/pxb1988/dex2jar)
- View JAR: JD-GUI (http://java-decompiler.github.io/)

## Output Structure
- `game_data/`
  - `units/`
  - `abilities/`
  - `localization/`
  - `events/`
  - `items/`
  - `config/`
- Validate script: Python `os.walk` + `json.load()`

## Fixes
- 0-byte audio: Caused by compression. Use UnityPy/AssetStudio.
- Wrong texture colors: Python PIL `img.convert('RGBA')`.
- Read errors: Try different Unity version tools.

## Next
- See `QUICKSTART.md`
