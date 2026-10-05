# Data Mapping

Source: `assets/dataModel.db` (SQLite, 67 tables, data version 3886).
Consumed via JSON exports in `game_data/` (see `scripts/export_game_data.py`).

## Combat stats location

- `unit` — static identity only. No combat numbers.
  - Used: `id`, `key_name`, `type`, `rarity`, `weapon_anim`, `unlock_tier`, `research_time`.
- `unit_level_progression` — ALL combat stats. One row per (unit, level), 3291 rows.
  - `hp` — health pool at that level.
  - `face_1_type`..`face_5_type` + `face_1_value`..`face_5_value` — the unit's 5-face die.
  - `special_id` — `unit_special.id` fired by SPECIAL faces (0 = none).
  - `passive_id` — passive reference (only 60900 in data).
  - `special_boost_value_a/b`, `passive_boost_value_a/b` — boost amounts for the above.
  - Ignored (economy/metadata): `killed_*`, `survived_*`, `price_id`, `asset_bundle_id`,
    `evolution_stage` (always 1), `is_skin`, `on_level_gift_id`, `level_up_requirement_id`,
    `alternative_weapon` (always 0).
- No separate attack/defense/initiative columns exist anywhere in the DB.
  Everything rolls through the die faces.

## Die face types (`unit_action.id` domain)

| id | meaning | evidence |
|---|---|---|
| 20000 | DAMAGE | prototype `wheelValues` match L1 faces (VOLT [2,4,6,9,10]); ATTACK BUFF `related_action_id` |
| 40000 | INITIATIVE | on Operative/Helicopter units, absent from Assault; localization key order matches ascending ids |
| 60000 | SPECIAL | 162/162 units holding it have `special_id != 0`; face value 0 |
| 70000 | ARMOUR_PIERCING | elimination; 11 units only (BUTTERKNIFE, SERVICE X, MACADEMI, ...) |

- `unit_action` also contains 10000, 30000 (MULTI-STRIKE via special 60200), 50000, 80000 — never used as a face type.
- Localization: `metadata_dice_type_damage|initiative|special|armour_piercing`.

## Abilities

- `ability` — 34 rows. Used: `id`, `key_name`, `key_description`, `ability_type` (1 ULTRA, 2 PRE-COMBAT, 3 REACTIVE, 4 PASSIVE), `action_point`, `handler_id`, `action_type`, `action_boost_value_a/b`, `target_group`, `limit_one_per_battle`, `limit_one_per_round`.
- `ability_handler` — 20 handler names (reroll, target, drawfire, barrage, jammer, ...).
- `ability.action_type` is an internal 100..106 enum, distinct from `unit_action` ids.
- `target_group`: 0 own team, 1 single enemy, 2 single unit either side (per handler usage).
- Ignored: `is_announcer`, `is_active`, `execution_order`, `unlock_*`, `research_time`, `num_kill_unit`, `icon_linkage_id`, `selection_text`.

## Unit specials

- `unit_special` — 20 rows (ATTACK BUFF 60100, MULTI-STRIKE 60200, RAIL GUN 60300, MINIGUN 60301, ARMOR BUFF 60400, FIRST STRIKE BUFF 60500, HOTSHOT 60600, NAPALM 60700, SHORT CIRCUIT 60800, EVADE 60900, ...).
- `unit_special_handler` — handler names (increase_base_attack, aoe_damage, rail_gun, ...).

## Names

- `localization_full.json` key `metadata_unit_name_<unit_id>` -> column `en`.
- `game_data/en/units.json` ships the same resolved (`key_name_text`) if needed.

## Mapping to types (`src/engine/types.ts`)

- `UnitDef` <- `unit` row + localized name + level-1 `unit_level_progression` row (`stats`) + full `progression`.
- `CombatStats` <- one `unit_level_progression` row: `hp`, 5x `DieFace` (`type` from face_N_type, `value` from face_N_value), `specialId`, `passiveId`.
- `AbilityDef` <- `ability` row + localized name/description.
- `UnitSpecialDef` <- `unit_special` row + localized name.

## Data quirks

- Boss units 49930-49933 have 2 progression rows for levels 2-5 (stage scaling). Loader keeps both, sorted by level.
- 17 `is_skin=1` rows extend a few units to cosmetic levels 90-91. Kept.
- `unit_level_progression.rarity` (0/1) is NOT `unit.rarity` (1-5). Ignored.
- VOLT level 6 replaces face 5 with a SPECIAL face (SHORT CIRCUIT 60800), value 0.
- Unit 49924 has SPECIAL faces with non-zero value (43) + `special_boost_value_a` — boost-charged specials.
- `metadata_unit_name_*` has 474 entries vs 370 units (extra keys for unreferenced ids).

## Unknown / TODO

- Face type evidence for 40000/70000 is inferential (C# source not in repo; AssetRipper export gitignored). Confidence high but unverified against game code.
- `unit_action` 10000, 50000, 80000 semantics unknown.
- `boost_ability_multiplier`, `boost`, `event_unit_boost` not mapped yet (boost stacking rules).
- `unit_cooldown`, `unit_partial_level` (per-part face upgrades) not mapped yet.
- Exact damage formula (armour subtraction, overkill) is engine work, not in data.
