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
 * Verified from decompiled C# source (`UnitLevelProgressionDataModel.faceTypeMap`):
 * raw DB ids map to enum ordinals (DieFaceType.cs):
 * None=0, DirectDamage=1, Initiative=2, Special=3, ArmourPiercing=4, AcidStrike=5.
 * - 10000 -> None, 20000 -> DirectDamage, 30000 -> Special, 40000 -> Initiative,
 *   50000 -> Special, 60000 -> Special, 70000 -> ArmourPiercing, 80000 -> AcidStrike.
 * 30000, 50000 and 60000 are ALL Special (COMBAT_SPEC.md §1).
 */
export const DIE_FACE_TYPE = {
  NONE: 10000,
  DAMAGE: 20000,
  INITIATIVE: 40000,
  SPECIAL: 60000,
  ARMOUR_PIERCING: 70000,
  ACID_STRIKE: 80000,
} as const;

export type DieFaceType = (typeof DIE_FACE_TYPE)[keyof typeof DIE_FACE_TYPE];

/**
 * Full DB id -> canonical `DIE_FACE_TYPE` mapping (`faceTypeMap`,
 * `UnitLevelProgressionDataModel.cs:70-104`). The loader normalizes raw ids
 * through this map so engine code only ever sees canonical values (30000 and
 * 50000 both collapse to SPECIAL = 60000).
 */
export const FACE_TYPE_ID_MAP: Readonly<Record<number, DieFaceType>> = {
  10000: DIE_FACE_TYPE.NONE,
  20000: DIE_FACE_TYPE.DAMAGE,
  30000: DIE_FACE_TYPE.SPECIAL,
  40000: DIE_FACE_TYPE.INITIATIVE,
  50000: DIE_FACE_TYPE.SPECIAL,
  60000: DIE_FACE_TYPE.SPECIAL,
  70000: DIE_FACE_TYPE.ARMOUR_PIERCING,
  80000: DIE_FACE_TYPE.ACID_STRIKE,
};

/** Enum ordinals used by `unit_partial_level.faceNType` casts (0-5, 0 = keep base). */
export const DIE_FACE_ORDINAL_TO_TYPE: readonly DieFaceType[] = [
  DIE_FACE_TYPE.NONE,
  DIE_FACE_TYPE.DAMAGE,
  DIE_FACE_TYPE.INITIATIVE,
  DIE_FACE_TYPE.SPECIAL,
  DIE_FACE_TYPE.ARMOUR_PIERCING,
  DIE_FACE_TYPE.ACID_STRIKE,
];

/**
 * `DamageType` enum from C# decompilation (`BattleLogic.ApplyDamage`).
 */
export const DAMAGE_TYPE = {
  STANDARD: 0,
  AOE: 1,
  PER_ROUND: 2,
  ARMOUR_PIERCING: 3,
  ACID: 4,
} as const;

export type DamageType = (typeof DAMAGE_TYPE)[keyof typeof DAMAGE_TYPE];

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
  /** `special_boost_value_a/b` — BoostValue/SecondaryBoostValue of the special. */
  readonly specialBoostA: number;
  readonly specialBoostB: number;
  /** `passive_boost_value_a/b` — boost values of the passive slot. */
  readonly passiveBoostA: number;
  readonly passiveBoostB: number;
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
  /** `related_action_id`, a `unit_action.id` (0 = none; NOT read by battle code). */
  readonly relatedActionId: number;
  /** `handler_id` -> `unit_special_handler.id`. */
  readonly handlerId: number;
  readonly executionOrder: number;
}

/** `unit_special_handler` row: handler id -> behavior string (`AbilityHandlerFactory.cs:Init`). */
export interface UnitSpecialHandlerDef {
  readonly id: number;
  readonly handler: string;
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
  readonly special_boost_value_a: number;
  readonly special_boost_value_b: number;
  readonly passive_boost_value_a: number;
  readonly passive_boost_value_b: number;
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

export interface RawSpecialHandlerRow {
  readonly id: number;
  readonly handler: string;
}

/** One entry of localization_full.json (only `key` + `en` consumed). */
export interface RawLocalizationEntry {
  readonly key: string;
  readonly en: string | null;
}
