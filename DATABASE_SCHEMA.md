# Database Schema Reference

## Overview

The `dataModel.db` SQLite database contains **67 tables** with complete game data including 370 units, 34 abilities, events, progression systems, and more.

**Database Version**: 3886

## Quick Stats

- **Units**: 370
- **Abilities**: 34
- **Unit Types**: 14
- **Rarity Levels**: 5
- **Tables**: 67
- **Languages**: 14

## Core Tables

### Units System

#### `unit` - Main unit definitions (370 rows)
```sql
CREATE TABLE unit (
    id INTEGER UNIQUE PRIMARY KEY,
    key_name VARCHAR,              -- Localization key
    type INTEGER,                  -- Foreign key to unit_type
    rarity INTEGER,                -- Foreign key to unit_rarity (1-5)
    weapon_anim INTEGER,           -- Animation reference
    blueprint_linkage_id INTEGER,  -- Asset linkage
    reward_type_id INTEGER,
    reward_amount INTEGER,
    research_time INTEGER,         -- Time to unlock (seconds)
    unlock_tier INTEGER,           -- Tier requirement
    can_buy_direct INTEGER,        -- Can purchase directly
    found_in_gacha INTEGER,        -- Available in gacha
    unit_index INTEGER             -- Display order
)
```

**Sample units**: VOLT, LONGSHOT, BOLTS, RAZE, RUSTY, BOOMBOX, PROSPECTOR, SCREECH, RENEGADE, SPARKY

#### `unit_type` - Unit classifications (14 types)
- **1**: Assault - Primary damage dealers
- **2**: Command - Quick Strike providers, buff allies
- **3**: Operative - Wildcards with game-changing abilities
- **4**: Helicopter - Air units
- **5-7**: Exclusive variants (Assault, Command, Operative)
- **10-13**: Event variants (Air, Assault, Command, Operative)
- **20**: MEGA BOSS! - Raid bosses

#### `unit_rarity` - Rarity levels (5 levels)
- 1 to 5 (Common to Legendary)

#### Related Unit Tables
- `unit_action` - Unit combat actions
- `unit_cooldown` - Ability cooldowns
- `unit_destroy_gem_drop` - Gem drop rates
- `unit_level_progression` - Leveling system
- `unit_level_up_requirement` - Level up costs
- `unit_part_types` - Unit component types
- `unit_partial_level` - Partial leveling
- `unit_parts` - Unit components
- `unit_scrap_value` - Recycling value
- `unit_special` - Special abilities
- `unit_special_handler` - Special ability handlers
- `unit_weapon_anim` - Weapon animations

### Abilities System

#### `ability` - Ability definitions (34 rows)
```sql
CREATE TABLE ability (
    id INTEGER UNIQUE PRIMARY KEY,
    key_name VARCHAR,              -- Localization key
    key_description VARCHAR,       -- Description key
    ability_type INTEGER,          -- Type (1-4)
    handler_id INTEGER,            -- Handler reference
    action_type INTEGER,
    action_boost_type INTEGER,
    action_boost_value_a INTEGER,
    action_boost_value_b INTEGER,
    action_point INTEGER,          -- Cost in action points
    target_group INTEGER,          -- Target selection
    is_active INTEGER,
    is_announcer INTEGER,
    execution_order INTEGER,       -- Order in combat phase
    unlock_tier INTEGER,
    unlock_order INTEGER,
    research_time INTEGER,
    limit_one_per_battle INTEGER,
    limit_one_per_round INTEGER,
    num_kill_unit INTEGER,
    icon_linkage_id INTEGER,
    selection_text VARCHAR
)
```

#### `ability_type` - Ability categories (4 types)
1. **ULTRA** - Fire after First-Strike phase, before combat
2. **PRE-COMBAT** - During attack phase, before units fire
3. **REACTIVE** - Counter specific abilities
4. **PASSIVE** - Used inside and outside battle, no assignment needed

#### Related Ability Tables
- `ability_handler` - Ability behavior handlers
- `boost_ability_multiplier` - Ability boost calculations

### Game Systems

#### Events System
- `event` - Event definitions with start/end timestamps
- `event_assets` - Event-specific assets
- `event_multi_team_effectiveness` - Team bonuses
- `event_multi_team_result_thresho` - Team results
- `event_parts` - Event components
- `event_points_buckets` - Point thresholds
- `event_raidboss_damage_drop_rate` - Raid boss loot
- `event_unit_boost` - Event unit bonuses
- `event_units` - Event-exclusive units

#### Gacha System (Loot Boxes)
- `gacha_ab_testing` - A/B testing configurations
- `gacha_ab_testing_group_enable` - Test group settings
- `gacha_info_details` - Gacha machine details
- `gacha_info_items` - Items in gacha pools
- `gacha_plinko_details` - Plinko minigame config
- `gacha_plinko_prize_price` - Prize costs
- `gacha_plinko_prizes` - Available prizes
- `gacha_plinko_slot_chances` - Drop rates per slot
- `gacha_pools` - Gacha pool definitions

#### Progression System
- `progression_division` - Division/rank system
- `progression_promotion_series` - Promotion requirements
- `progression_tier_buckets` - Tier groupings

#### Economy
- `item` - In-game items
- `item_gift` - Gift items
- `item_price` - Item pricing

#### Leaderboards
- `leaderboards` - Leaderboard definitions
- `leaderboard_rewards` - Rank rewards

#### AI System
- `ai_army` - AI opponent armies
- `ai_army_parts` - AI army compositions
- `ai_handler` - AI behavior handlers

#### Mission System
- `contracts` - Contract/mission definitions
- `contract_details` - Contract requirements

#### UI & Assets
- `asset_bundles` - Unity asset bundles
- `asset_linkage` - Asset ID mappings
- `dialog_screen` - Dialog configurations
- `audio_triggers` - Audio event triggers
- `effects` - Visual effects

#### Help System
- `help_registers` - Help topic registry
- `help_topic` - Help content

#### Content
- `announcer_dialog_sequences` - Announcer voice lines
- `lines_emoticons` - Emoticon definitions
- `lines_news` - News feed items
- `lines_pro_tips` - Tutorial tips

#### Other
- `boost` - Boost items/effects
- `config` - Game configuration
- `constant` - Game constants
- `localization` - Multilingual text (14 languages)
- `news` - News items
- `version` - Database version

## Localization

The `localization` table contains translations in 14 languages:
1. English
2. French
3. English (UK)
4. Chinese
5. Portuguese
6. English
7. German
8. English
9. Italian
10. English
11. English
12. Default
13. Spanish
14. Italian

## Configuration

### Key Config Values
```sql
SELECT * FROM config;
```

- `VERSION`: 1
- `ASSET_URL`: http://files.mobage.com/files/battledice/asset_3836/

## Sample Queries

### Get all units with their types
```sql
SELECT u.id, l.en_us as name, ut.id as type_id, ur.id as rarity
FROM unit u
JOIN localization l ON u.key_name = l.key
LEFT JOIN unit_type ut ON u.type = ut.id
LEFT JOIN unit_rarity ur ON u.rarity = ur.id
ORDER BY u.unit_index;
```

### Get all abilities with descriptions
```sql
SELECT a.id, 
       ln.en_us as name, 
       ld.en_us as description,
       at.id as type
FROM ability a
JOIN localization ln ON a.key_name = ln.key
JOIN localization ld ON a.key_description = ld.key
JOIN ability_type at ON a.ability_type = at.id;
```

### Get active events
```sql
SELECT * FROM event
ORDER BY start_timestamp DESC;
```

### Count units by rarity
```sql
SELECT rarity, COUNT(*) as count
FROM unit
GROUP BY rarity
ORDER BY rarity;
```

### Get unit types with names
```sql
SELECT ut.id, l.en_us as name, l2.en_us as description
FROM unit_type ut
JOIN localization l ON ut.key_name = l.key
JOIN localization l2 ON l2.key LIKE 'metadata_unit_type_description_' || ut.id;
```

## Notes

- All text content is in the `localization` table with keys referenced from other tables
- IDs typically start at high numbers (11001+ for units, 101000+ for abilities)
- Timestamps are in format: "MM/DD/YYYY HH:MM:SS-TZ:TZ"
- Research times are in seconds
- Most boolean fields use INTEGER (0/1)

## Extraction Tips

To export data to JSON:
```bash
# Install sqlite-utils
pip install sqlite-utils

# Export a table
sqlite-utils dataModel.db "SELECT * FROM unit" --json-cols > units.json

# Export with joins
sqlite-utils dataModel.db "SELECT u.*, l.en_us as name FROM unit u JOIN localization l ON u.key_name = l.key" > units_with_names.json
```

To inspect the database:
```bash
sqlite3 assets/dataModel.db
.tables          # List all tables
.schema unit     # Show table schema
.mode column     # Better formatting
.headers on      # Show column headers
```
