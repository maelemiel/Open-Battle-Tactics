# Database Schema

- `dataModel.db` SQLite database.
- 67 tables.
- Database Version: 3886.

## Stats
- Units: 370
- Abilities: 34
- Unit Types: 14
- Rarity Levels: 5
- Tables: 67
- Languages: 14

## Core Tables

### Units
- `unit`: 370 rows.
  - `id` (INTEGER UNIQUE PRIMARY KEY)
  - `key_name` (VARCHAR, localization key)
  - `type` (INTEGER, FK `unit_type`)
  - `rarity` (INTEGER, FK `unit_rarity`, 1-5)
  - `weapon_anim` (INTEGER)
  - `blueprint_linkage_id` (INTEGER)
  - `reward_type_id` (INTEGER)
  - `reward_amount` (INTEGER)
  - `research_time` (INTEGER, seconds)
  - `unlock_tier` (INTEGER)
  - `can_buy_direct` (INTEGER)
  - `found_in_gacha` (INTEGER)
  - `unit_index` (INTEGER)
- Sample units: VOLT, LONGSHOT, BOLTS, RAZE, RUSTY, BOOMBOX, PROSPECTOR, SCREECH, RENEGADE, SPARKY.

- `unit_type`: 14 types.
  - 1: Assault.
  - 2: Command.
  - 3: Operative.
  - 4: Helicopter.
  - 5-7: Exclusive variants (Assault, Command, Operative).
  - 10-13: Event variants (Air, Assault, Command, Operative).
  - 20: MEGA BOSS.

- `unit_rarity`: 5 levels (1-5, Common-Legendary).

- Related unit tables:
  - `unit_action`: combat actions.
  - `unit_cooldown`: ability cooldowns.
  - `unit_destroy_gem_drop`: gem drop rates.
  - `unit_level_progression`: leveling system.
  - `unit_level_up_requirement`: level up costs.
  - `unit_part_types`: component types.
  - `unit_partial_level`: partial leveling.
  - `unit_parts`: components.
  - `unit_scrap_value`: recycling value.
  - `unit_special`: special abilities.
  - `unit_special_handler`: handlers.
  - `unit_weapon_anim`: weapon animations.

### Abilities
- `ability`: 34 rows.
  - `id` (INTEGER UNIQUE PRIMARY KEY)
  - `key_name` (VARCHAR)
  - `key_description` (VARCHAR)
  - `ability_type` (INTEGER, 1-4)
  - `handler_id` (INTEGER)
  - `action_type` (INTEGER)
  - `action_boost_type` (INTEGER)
  - `action_boost_value_a` (INTEGER)
  - `action_boost_value_b` (INTEGER)
  - `action_point` (INTEGER)
  - `target_group` (INTEGER)
  - `is_active` (INTEGER)
  - `is_announcer` (INTEGER)
  - `execution_order` (INTEGER)
  - `unlock_tier` (INTEGER)
  - `unlock_order` (INTEGER)
  - `research_time` (INTEGER)
  - `limit_one_per_battle` (INTEGER)
  - `limit_one_per_round` (INTEGER)
  - `num_kill_unit` (INTEGER)
  - `icon_linkage_id` (INTEGER)
  - `selection_text` (VARCHAR)

- `ability_type`: 4 types.
  - 1: ULTRA (fire after First-Strike, before combat).
  - 2: PRE-COMBAT (during attack phase, before fire).
  - 3: REACTIVE (counter abilities).
  - 4: PASSIVE (used everywhere, no assignment needed).

- Related ability tables:
  - `ability_handler`: behavior handlers.
  - `boost_ability_multiplier`: boost calculations.

### Systems
- Events:
  - `event`: start/end timestamps.
  - `event_assets`: assets.
  - `event_multi_team_effectiveness`: team bonuses.
  - `event_multi_team_result_thresho`: team results.
  - `event_parts`: components.
  - `event_points_buckets`: point thresholds.
  - `event_raidboss_damage_drop_rate`: raid boss loot.
  - `event_unit_boost`: unit bonuses.
  - `event_units`: exclusive units.

- Gacha:
  - `gacha_ab_testing`: A/B testing.
  - `gacha_ab_testing_group_enable`: test group settings.
  - `gacha_info_details`: machine details.
  - `gacha_info_items`: items in pools.
  - `gacha_plinko_details`: plinko config.
  - `gacha_plinko_prize_price`: prize costs.
  - `gacha_plinko_prizes`: available prizes.
  - `gacha_plinko_slot_chances`: drop rates.
  - `gacha_pools`: pool definitions.

- Progression:
  - `progression_division`: division/rank system.
  - `progression_promotion_series`: promotion requirements.
  - `progression_tier_buckets`: tier groupings.

- Economy:
  - `item`: in-game items.
  - `item_gift`: gifts.
  - `item_price`: pricing.

- Leaderboards:
  - `leaderboards`: definitions.
  - `leaderboard_rewards`: rank rewards.

- AI:
  - `ai_army`: opponents.
  - `ai_army_parts`: compositions.
  - `ai_handler`: behavior handlers.

- Mission:
  - `contracts`: definitions.
  - `contract_details`: requirements.

- UI/Assets:
  - `asset_bundles`: Unity bundles.
  - `asset_linkage`: ID mappings.
  - `dialog_screen`: configs.
  - `audio_triggers`: events.
  - `effects`: VFX.

- Help:
  - `help_registers`: topic registry.
  - `help_topic`: content.

- Content:
  - `announcer_dialog_sequences`: voice lines.
  - `lines_emoticons`: emoticons.
  - `lines_news`: news feed.
  - `lines_pro_tips`: tutorial tips.

- Other:
  - `boost`: items/effects.
  - `config`: configuration.
  - `constant`: constants.
  - `localization`: text (14 languages).
  - `news`: news items.
  - `version`: DB version.

## Localization
- `localization` table: 14 languages.
- 1: English, 2: French, 3: English (UK), 4: Chinese, 5: Portuguese, 6: English, 7: German, 8: English, 9: Italian, 10: English, 11: English, 12: Default, 13: Spanish, 14: Italian.

## Configuration
- `config` table key values:
  - `VERSION`: 1
  - `ASSET_URL`: http://files.mobage.com/files/battledice/asset_3836/

## Queries

- Get units with types:
  ```sql
  SELECT u.id, l.en_us as name, ut.id as type_id, ur.id as rarity FROM unit u JOIN localization l ON u.key_name = l.key LEFT JOIN unit_type ut ON u.type = ut.id LEFT JOIN unit_rarity ur ON u.rarity = ur.id ORDER BY u.unit_index;
  ```

- Get abilities:
  ```sql
  SELECT a.id, ln.en_us as name, ld.en_us as description, at.id as type FROM ability a JOIN localization ln ON a.key_name = ln.key JOIN localization ld ON a.key_description = ld.key JOIN ability_type at ON a.ability_type = at.id;
  ```

- Get active events:
  ```sql
  SELECT * FROM event ORDER BY start_timestamp DESC;
  ```

- Count units by rarity:
  ```sql
  SELECT rarity, COUNT(*) as count FROM unit GROUP BY rarity ORDER BY rarity;
  ```

- Get unit types with names:
  ```sql
  SELECT ut.id, l.en_us as name, l2.en_us as description FROM unit_type ut JOIN localization l ON ut.key_name = l.key JOIN localization l2 ON l2.key LIKE 'metadata_unit_type_description_' || ut.id;
  ```

## Notes
- Text in `localization` table. Keys referenced by other tables.
- IDs start high (11001+ units, 101000+ abilities).
- Timestamps: "MM/DD/YYYY HH:MM:SS-TZ:TZ".
- Research times: seconds.
- Booleans: INTEGER (0/1).

## Extraction
- JSON export via `sqlite-utils`:
  ```bash
  pip install sqlite-utils
  sqlite-utils dataModel.db "SELECT * FROM unit" --json-cols > units.json
  sqlite-utils dataModel.db "SELECT u.*, l.en_us as name FROM unit u JOIN localization l ON u.key_name = l.key" > units_with_names.json
  ```
- SQLite CLI inspection:
  ```bash
  sqlite3 assets/dataModel.db
  .tables
  .schema unit
  .mode column
  .headers on
  ```
