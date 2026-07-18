# Data Extraction Guide

This guide explains how to extract and work with data from the Super Battle Tactics APK.

## Overview

The repository already contains:
- ✅ Extracted SQLite databases (`assets/dataModel.db`, `assets/keyValue.db`)
- ✅ Some extracted Unity assets (`extracted_assets/`)
- ✅ Original APK file (`super-battle-tactics.zip`)
- ⚠️ Some assets are incomplete (audio files are 0 bytes, shaders are empty)

## Tools Needed

### For APK Analysis
- **APK Tool** - Decompile APK
  ```bash
  # Install on Linux/Mac
  brew install apktool
  # Or download from: https://apktool.org/
  ```

### For Unity Asset Extraction
- **AssetStudio** - GUI tool for Unity assets
  - Download: https://github.com/Perfare/AssetStudio/releases
  - Best for: Visual browsing and extracting textures, audio, models

- **UnityPy** - Python library for Unity assets
  ```bash
  pip install UnityPy
  ```
  - Best for: Automated extraction, scripting

- **UABE (Unity Asset Bundle Extractor)**
  - Download: https://github.com/SeriousCache/UABE/releases
  - Best for: Complex asset bundles

### For SQLite Database
- **SQLite Browser** - GUI for SQLite
  - Download: https://sqlitebrowser.org/
  - Best for: Visual exploration

- **sqlite3** - Command line tool
  ```bash
  # Usually pre-installed on Linux/Mac
  sqlite3 --version
  ```

## Extraction Methods

### Method 1: Extract Unity Assets with AssetStudio (Recommended)

1. **Download and Install AssetStudio**
   - Get latest release from GitHub
   - No installation needed on Windows (portable)

2. **Load the Assets**
   ```
   File > Load folder
   Navigate to: Open-Battle-Tactics/assets/bin/Data/
   ```

3. **Browse Assets**
   - Asset List tab shows all assets
   - Scene Hierarchy shows Unity objects
   - Filter by type (Texture2D, AudioClip, Mesh, etc.)

4. **Export Assets**
   ```
   Select assets > Export > Export selected assets
   Choose output folder
   Select format (PNG for textures, WAV for audio, etc.)
   ```

5. **Export All**
   ```
   Export > All assets
   Choose output directory
   Wait for extraction (may take several minutes)
   ```

### Method 2: Extract with UnityPy (Python Script)

```python
# extract_unity_assets.py
import UnityPy
import os
from pathlib import Path

def extract_assets(source_folder, dest_folder):
    """Extract all Unity assets from a folder"""
    
    # Load environment
    env = UnityPy.load(source_folder)
    
    # Create output directory
    Path(dest_folder).mkdir(parents=True, exist_ok=True)
    
    # Statistics
    stats = {
        'Texture2D': 0,
        'Sprite': 0,
        'AudioClip': 0,
        'TextAsset': 0,
        'Shader': 0,
        'Mesh': 0,
        'Other': 0
    }
    
    # Iterate all assets
    for obj in env.objects:
        try:
            # Get object data
            if obj.type.name == "Texture2D":
                data = obj.read()
                # Save as PNG
                dest = os.path.join(dest_folder, "Texture2D", f"{data.name}.png")
                os.makedirs(os.path.dirname(dest), exist_ok=True)
                data.image.save(dest)
                stats['Texture2D'] += 1
                
            elif obj.type.name == "AudioClip":
                data = obj.read()
                # Save audio
                for name, audio_data in data.samples.items():
                    dest = os.path.join(dest_folder, "AudioClip", f"{data.name}.wav")
                    os.makedirs(os.path.dirname(dest), exist_ok=True)
                    with open(dest, 'wb') as f:
                        f.write(audio_data)
                    stats['AudioClip'] += 1
                    
            elif obj.type.name == "TextAsset":
                data = obj.read()
                # Save text
                dest = os.path.join(dest_folder, "TextAsset", f"{data.name}.txt")
                os.makedirs(os.path.dirname(dest), exist_ok=True)
                with open(dest, 'wb') as f:
                    f.write(bytes(data.script))
                stats['TextAsset'] += 1
                
            elif obj.type.name == "Sprite":
                data = obj.read()
                dest = os.path.join(dest_folder, "Sprite", f"{data.name}.png")
                os.makedirs(os.path.dirname(dest), exist_ok=True)
                data.image.save(dest)
                stats['Sprite'] += 1
                
            elif obj.type.name == "Shader":
                data = obj.read()
                dest = os.path.join(dest_folder, "Shader", f"{data.name}.shader")
                os.makedirs(os.path.dirname(dest), exist_ok=True)
                with open(dest, 'w') as f:
                    f.write(str(data.export()))
                stats['Shader'] += 1
                
            elif obj.type.name == "Mesh":
                data = obj.read()
                dest = os.path.join(dest_folder, "Mesh", f"{data.name}.obj")
                os.makedirs(os.path.dirname(dest), exist_ok=True)
                with open(dest, 'w') as f:
                    f.write(data.export())
                stats['Mesh'] += 1
                
            else:
                stats['Other'] += 1
                
        except Exception as e:
            print(f"Error processing {obj.type.name}: {e}")
            continue
    
    # Print statistics
    print("\nExtraction complete!")
    print("=" * 40)
    for asset_type, count in stats.items():
        if count > 0:
            print(f"{asset_type}: {count} files")
    print("=" * 40)

# Usage
if __name__ == "__main__":
    source = "assets/bin/Data"
    destination = "extracted_assets_new"
    
    print(f"Extracting assets from: {source}")
    print(f"Saving to: {destination}")
    print()
    
    extract_assets(source, destination)
```

**Run the script:**
```bash
python extract_unity_assets.py
```

### Method 3: Export SQLite Data to JSON

#### Using sqlite-utils (Recommended)
```bash
# Install
pip install sqlite-utils

# Export entire database structure
sqlite-utils dump assets/dataModel.db > database_dump.sql

# Export specific table to JSON
sqlite-utils assets/dataModel.db "SELECT * FROM unit" --json > units.json

# Export with pretty formatting
sqlite-utils assets/dataModel.db "SELECT * FROM unit" --json-cols --fmt json > units_pretty.json

# Export all tables
for table in $(sqlite-utils tables assets/dataModel.db --json | jq -r '.[].name'); do
  echo "Exporting $table..."
  sqlite-utils assets/dataModel.db "SELECT * FROM $table" --json > "exports/${table}.json"
done
```

#### Using Python Script
```python
# export_db_to_json.py
import sqlite3
import json
import os

def export_database_to_json(db_path, output_dir):
    """Export all tables from SQLite to JSON files"""
    
    # Create output directory
    os.makedirs(output_dir, exist_ok=True)
    
    # Connect to database
    conn = sqlite3.connect(db_path)
    conn.row_factory = sqlite3.Row  # Return rows as dictionaries
    cursor = conn.cursor()
    
    # Get all table names
    cursor.execute("SELECT name FROM sqlite_master WHERE type='table';")
    tables = cursor.fetchall()
    
    stats = {}
    
    for table_row in tables:
        table_name = table_row[0]
        print(f"Exporting {table_name}...")
        
        # Get all rows from table
        cursor.execute(f"SELECT * FROM {table_name}")
        rows = cursor.fetchall()
        
        # Convert to list of dicts
        data = [dict(row) for row in rows]
        
        # Save to JSON
        output_file = os.path.join(output_dir, f"{table_name}.json")
        with open(output_file, 'w', encoding='utf-8') as f:
            json.dump(data, f, indent=2, ensure_ascii=False)
        
        stats[table_name] = len(data)
    
    conn.close()
    
    # Print statistics
    print("\n" + "=" * 50)
    print("Export complete!")
    print("=" * 50)
    for table, count in sorted(stats.items()):
        print(f"{table}: {count} rows")

# Usage
export_database_to_json('assets/dataModel.db', 'game_data')
```

#### Export with Joins (Units with Names)
```python
# export_units_full.py
import sqlite3
import json

def export_units_with_localization(db_path, output_file):
    """Export units with their localized names"""
    
    conn = sqlite3.connect(db_path)
    conn.row_factory = sqlite3.Row
    cursor = conn.cursor()
    
    # Complex query joining unit with localization
    query = """
    SELECT 
        u.id,
        u.key_name,
        l.en_us as name_en,
        l.fr_fr as name_fr,
        l.zh_cn as name_zh,
        u.type,
        ut.id as type_id,
        utl.en_us as type_name,
        u.rarity,
        u.unlock_tier,
        u.research_time,
        u.can_buy_direct,
        u.found_in_gacha
    FROM unit u
    LEFT JOIN localization l ON u.key_name = l.key
    LEFT JOIN unit_type ut ON u.type = ut.id
    LEFT JOIN localization utl ON ut.key_name = utl.key
    ORDER BY u.unit_index
    """
    
    cursor.execute(query)
    rows = cursor.fetchall()
    
    data = [dict(row) for row in rows]
    
    with open(output_file, 'w', encoding='utf-8') as f:
        json.dump(data, f, indent=2, ensure_ascii=False)
    
    conn.close()
    print(f"Exported {len(data)} units to {output_file}")

# Usage
export_units_with_localization('assets/dataModel.db', 'units_complete.json')
```

## Advanced Extraction

### Extract from Asset Bundles
```python
# extract_asset_bundles.py
import UnityPy

def extract_asset_bundle(bundle_path, output_dir):
    """Extract a specific Unity asset bundle"""
    env = UnityPy.load(bundle_path)
    
    for obj in env.objects:
        if obj.type.name in ["Texture2D", "Sprite", "AudioClip"]:
            data = obj.read()
            # Export logic here
            pass

# Check if asset bundles table exists
import sqlite3
conn = sqlite3.connect('assets/dataModel.db')
cursor = conn.cursor()
cursor.execute("SELECT * FROM asset_bundles")
bundles = cursor.fetchall()
print(f"Found {len(bundles)} asset bundles")
```

### Decompile APK
```bash
# Decompile the APK
apktool d super-battle-tactics.zip -o decompiled/

# This creates:
# decompiled/
# ├── AndroidManifest.xml (readable)
# ├── smali/ (Dalvik bytecode)
# ├── res/ (resources)
# └── assets/
```

### Extract DEX to Java
```bash
# Install dex2jar
# Download from: https://github.com/pxb1988/dex2jar

# Convert DEX to JAR
d2j-dex2jar classes.dex

# Output: classes-dex2jar.jar

# Decompile JAR with JD-GUI
# Download from: http://java-decompiler.github.io/
# Open classes-dex2jar.jar to view Java source
```

## Organizing Extracted Data

### Recommended Directory Structure
```
game_data/
├── units/
│   ├── units.json              # All units
│   ├── unit_types.json
│   └── unit_stats.json
├── abilities/
│   ├── abilities.json          # All abilities
│   └── ability_types.json
├── localization/
│   ├── en_us.json
│   ├── fr_fr.json
│   └── ...
├── events/
│   └── events.json
├── items/
│   └── items.json
└── config/
    └── config.json
```

### Validation Script
```python
# validate_extracted_data.py
import json
import os

def validate_json_files(directory):
    """Validate all JSON files in directory"""
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith('.json'):
                filepath = os.path.join(root, file)
                try:
                    with open(filepath, 'r') as f:
                        data = json.load(f)
                    print(f"✓ {filepath}: Valid ({len(data)} items)")
                except json.JSONDecodeError as e:
                    print(f"✗ {filepath}: Invalid - {e}")

validate_json_files('game_data')
```

## Common Issues

### Issue: Audio files are 0 bytes
**Solution**: Unity sometimes stores audio in compressed format. Use AssetStudio or UnityPy to properly extract.

### Issue: Textures have wrong colors
**Solution**: Some textures use different color spaces. Try converting:
```python
from PIL import Image
img = Image.open('texture.png')
img = img.convert('RGBA')
img.save('texture_fixed.png')
```

### Issue: Can't read asset bundle
**Solution**: Asset bundle format varies by Unity version. Try different tools or Unity versions.

## Tips

1. **Backup First**: Always keep original files
2. **Use Version Control**: Track extracted files with Git LFS for large files
3. **Document**: Note which tool/version you used for extraction
4. **Validate**: Check extracted files aren't corrupted
5. **Organize**: Use consistent naming conventions

## Next Steps

After extraction:
1. Convert data to game-ready format (JSON, CSV, etc.)
2. Import into your game engine
3. Validate data integrity
4. Create data classes/structures
5. Build loading systems

See [QUICKSTART.md](./QUICKSTART.md) for using extracted data in your game.
