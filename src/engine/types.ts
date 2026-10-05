/**
 * Frozen data contract for the TS rebuild.
 * Source of truth: game_data/*.json exports of assets/dataModel.db (data version 3886).
 * Column meanings verified against the old JS prototype (commit dcb4d96^).
 */

// ---------------------------------------------------------------------------
// Reference ids (DB small tables)
// ---------------------------------------------------------------------------

/** `unit_type.id`. 14 rows in DB. */
export type UnitTypeId =
  | 1 // Assault
  | 2 // Command
  | 3 // Operative
  | 4 // Helicopter
  | 5 // Exclusive Assault
  | 6 // Exclusive Command
  | 7 // Exclusive Operative
  | 8 // Exclusive Air
  | 9 // Boss
  | 10 // Event Air
  | 11 // Event Assault
  | 12 // Event Command
  | 13 // Event Operative
  | 20; // MEGA BOSS

/** `unit_rarity.id`. 5 rows in DB. */
export type RarityId =
  | 1 // COMMON
  | 2 // UNCOMMON
  | 3 // RARE
  | 4 // SUPER RARE
  | 5; // LEGENDARY

/** `ability_type.id`. 4 rows in DB. */
export type AbilityTypeId =
  | 1 // ULTRA: fires after First-Strike, before combat
  | 2 // PRE-COMBAT: during attack phase, before firing
  | 3 // REACTIVE: counter abilities
  | 4; // PASSIVE: always on, no assignment

// ---------------------------------------------------------------------------
// Die faces (combat core)
// ---------------------------------------------------------------------------

/**
 * `unit_level_progression.face_N_type`, a `unit_action.id`.
 * Decoded from data (no C# source available):
 * - 20000 DAMAGE: matches prototype `wheelValues` (VOLT L1 = [2,4,6,9,10]);
 *   also `unit_special.related_action_id` 20000 for ATTACK BUFF.
 * - 40000 INITIATIVE: carried by Operative/Helicopter units, absent from
 *   Assault; localization order (damage, initiative, special, armour_piercing)
 *   matches ascending id order.
 * - 60000 SPECIAL: all 162 units holding a 60000 face have special_id != 0.
 * - 70000 ARMOUR_PIERCING: by elimination; 11 units only (BUTTERKNIFE, ...).
 */
export const DIE_FACE_TYPE = {
  DAMAGE: 20000,
  INITIATIVE: 40000,
  SPECIAL: 60000,
  ARMOUR_PIERCING: 70000,
} as const;

export type DieFaceType = (typeof DIE_FACE_TYPE)[keyof typeof DIE_FACE_TYPE];

/** One face of a unit's 5-face die. Value 0 on SPECIAL faces without boost. */
export interface DieFace {
  readonly type: DieFaceType;
  readonly value: number;
}

/**
 * Combat stats of a unit at a given level (`unit_level_progression` row).
 * hp is the pool; the 5 faces are rolled each round (wheel/dice combat).
 */
export interface CombatStats {
  readonly level: number;
  readonly hp: number;
  readonly faces: readonly DieFace[];
  /** `special_id` -> `unit_special.id` (0 = none). Triggered by SPECIAL faces. */
  readonly specialId: number;
  /** `passive_id` -> passive ability id (0 = none). */
  readonly passiveId: number;
}

// ---------------------------------------------------------------------------
// Static definitions
// ---------------------------------------------------------------------------

/** `unit` row joined with its name (localization `en`) and level-1 stats. */
export interface UnitDef {
  readonly id: number;
  /** English name from localization (`metadata_unit_name_<id>`). */
  readonly name: string;
  readonly typeId: UnitTypeId;
  readonly rarityId: RarityId;
  /** `unit_weapon_anim.id` -> anim_name (1..7). */
  readonly weaponAnim: number;
  readonly unlockTier: number;
  /** Research time in seconds. */
  readonly researchTime: number;
  /** Level-1 combat stats (the MVP baseline). */
  readonly stats: CombatStats;
  /** Full level progression, ascending by level. Boss rows may repeat a level. */
  readonly progression: readonly CombatStats[];
}

/** `ability` row joined with localized name/description. */
export interface AbilityDef {
  readonly id: number;
  readonly name: string;
  readonly description: string;
  readonly abilityType: AbilityTypeId;
  /** Action points cost to activate. */
  readonly actionPoint: number;
  /** `handler_id` -> `ability_handler.id` (901 reroll, 902 target, ...). */
  readonly handlerId: number;
  /** Internal action kind (100 reroll, 101 damage, 102 drawfire, ...). */
  readonly actionType: number;
  /** Boost values; semantics depend on handler (e.g. damage range a..b). */
  readonly boostValueA: number;
  readonly boostValueB: number;
  /** 0 own team, 1 enemy single, 2 enemy/own unit, per ability_handler usage. */
  readonly targetGroup: number;
  readonly limitOnePerBattle: boolean;
  readonly limitOnePerRound: boolean;
}

/** `unit_special` row (special die face effect). */
export interface UnitSpecialDef {
  readonly id: number;
  readonly name: string;
  /** `related_action_id`, a `unit_action.id` (0 = none). */
  readonly relatedActionId: number;
  /** `handler_id` -> `unit_special_handler.id`. */
  readonly handlerId: number;
  readonly executionOrder: number;
}

// ---------------------------------------------------------------------------
// Raw JSON row shapes (narrow the untyped imports at the loader boundary)
// ---------------------------------------------------------------------------

export interface RawUnitRow {
  readonly id: number;
  readonly key_name: string;
  readonly type: number;
  readonly rarity: number;
  readonly weapon_anim: number;
  readonly unlock_tier: number;
  readonly research_time: number;
}

export interface RawProgressionRow {
  readonly unit_id: number;
  readonly level: number;
  readonly hp: number;
  readonly face_1_type: number;
  readonly face_1_value: number;
  readonly face_2_type: number;
  readonly face_2_value: number;
  readonly face_3_type: number;
  readonly face_3_value: number;
  readonly face_4_type: number;
  readonly face_4_value: number;
  readonly face_5_type: number;
  readonly face_5_value: number;
  readonly special_id: number;
  readonly passive_id: number;
}

export interface RawAbilityRow {
  readonly id: number;
  readonly key_name: string;
  readonly key_description: string;
  readonly ability_type: number;
  readonly action_point: number;
  readonly handler_id: number;
  readonly action_type: number;
  readonly action_boost_value_a: number;
  readonly action_boost_value_b: number;
  readonly target_group: number;
  readonly limit_one_per_battle: number;
  readonly limit_one_per_round: number;
}

export interface RawSpecialRow {
  readonly id: number;
  readonly key_name: string;
  readonly related_action_id: number;
  readonly handler_id: number;
  readonly execution_order: number;
}

/** One entry of localization_full.json (only `key` + `en` consumed). */
export interface RawLocalizationEntry {
  readonly key: string;
  readonly en: string | null;
}
