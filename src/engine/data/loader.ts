/**
 * Loads game data from the JSON exports in game_data/.
 * All rows are validated at load time: an unexpected shape throws instead of
 * silently producing a broken UnitDef/AbilityDef.
 */

import type {
  AbilityDef,
  AbilityTypeId,
  CombatStats,
  DieFace,
  DieFaceType,
  RarityId,
  RawAbilityRow,
  RawLocalizationEntry,
  RawProgressionRow,
  RawSpecialRow,
  RawUnitRow,
  UnitDef,
  UnitSpecialDef,
  UnitTypeId,
} from '../types';
import { DIE_FACE_TYPE } from '../types';

import rawUnits from '../../../game_data/unit.json';
import rawProgression from '../../../game_data/unit_level_progression.json';
import rawAbilities from '../../../game_data/ability.json';
import rawSpecials from '../../../game_data/unit_special.json';
import rawLocalization from '../../../game_data/localization_full.json';

// ---------------------------------------------------------------------------
// Validation helpers (zero `any`: cast JSON through `unknown`)
// ---------------------------------------------------------------------------

const UNIT_TYPE_IDS: readonly UnitTypeId[] = [
  1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 20,
];
const RARITY_IDS: readonly RarityId[] = [1, 2, 3, 4, 5];
const ABILITY_TYPE_IDS: readonly AbilityTypeId[] = [1, 2, 3, 4];
const FACE_TYPE_IDS: readonly DieFaceType[] = [
  DIE_FACE_TYPE.DAMAGE,
  DIE_FACE_TYPE.INITIATIVE,
  DIE_FACE_TYPE.SPECIAL,
  DIE_FACE_TYPE.ARMOUR_PIERCING,
];

function rows(value: unknown): readonly unknown[] {
  if (!Array.isArray(value)) throw new Error('expected JSON array');
  return value;
}

function num(row: unknown, key: string): number {
  const v = (row as Record<string, unknown>)[key];
  if (typeof v !== 'number' || !Number.isFinite(v)) {
    throw new Error(`field ${key} is not a number: ${String(v)}`);
  }
  return v;
}

function str(row: unknown, key: string): string {
  const v = (row as Record<string, unknown>)[key];
  if (typeof v !== 'string') throw new Error(`field ${key} is not a string`);
  return v;
}

function pick<T extends number>(row: unknown, key: string, allowed: readonly T[]): T {
  const v = num(row, key);
  if (!(allowed as readonly number[]).includes(v)) {
    throw new Error(`field ${key} has unexpected value ${v}`);
  }
  return v as T;
}

// ---------------------------------------------------------------------------
// Localization
// ---------------------------------------------------------------------------

const localization = new Map<string, string>(
  rows(rawLocalization).map((row) => {
    const entry = row as RawLocalizationEntry;
    return [str(entry, 'key'), str(entry, 'en')];
  }),
);

/** English text for a localization key; falls back to the key itself. */
export function localize(key: string): string {
  return localization.get(key) ?? key;
}

// ---------------------------------------------------------------------------
// Units
// ---------------------------------------------------------------------------

function toStats(row: RawProgressionRow): CombatStats {
  const faces: DieFace[] = [];
  for (let i = 1; i <= 5; i++) {
    faces.push({
      type: pick(row, `face_${i}_type`, FACE_TYPE_IDS),
      value: num(row, `face_${i}_value`),
    });
  }
  return {
    level: num(row, 'level'),
    hp: num(row, 'hp'),
    faces,
    specialId: num(row, 'special_id'),
    passiveId: num(row, 'passive_id'),
  };
}

function buildUnit(unitRow: RawUnitRow, progression: readonly RawProgressionRow[]): UnitDef {
  const sorted = [...progression].sort((a, b) => num(a, 'level') - num(b, 'level'));
  const levelOne = sorted.find((row) => num(row, 'level') === 1);
  if (!levelOne) {
    throw new Error(`unit ${num(unitRow, 'id')} has no level 1 progression row`);
  }
  return {
    id: num(unitRow, 'id'),
    name: localize(str(unitRow, 'key_name')),
    typeId: pick(unitRow, 'type', UNIT_TYPE_IDS),
    rarityId: pick(unitRow, 'rarity', RARITY_IDS),
    weaponAnim: num(unitRow, 'weapon_anim'),
    unlockTier: num(unitRow, 'unlock_tier'),
    researchTime: num(unitRow, 'research_time'),
    stats: toStats(levelOne),
    progression: sorted.map(toStats),
  };
}

let unitsCache: readonly UnitDef[] | undefined;
let unitsByIdCache: Map<number, UnitDef> | undefined;

function ensureUnitCaches(): void {
  if (unitsCache) return;
  const progressionByUnit = new Map<number, RawProgressionRow[]>();
  for (const row of rows(rawProgression)) {
    const unitId = num(row, 'unit_id');
    const list = progressionByUnit.get(unitId);
    if (list) list.push(row as RawProgressionRow);
    else progressionByUnit.set(unitId, [row as RawProgressionRow]);
  }
  unitsCache = rows(rawUnits).map((row) =>
    buildUnit(row as RawUnitRow, progressionByUnit.get(num(row, 'id')) ?? []),
  );
  unitsByIdCache = new Map(unitsCache.map((unit) => [unit.id, unit]));
}

/** All 370 unit definitions, ordered as in game_data/unit.json. */
export function loadUnits(): readonly UnitDef[] {
  ensureUnitCaches();
  return unitsCache!;
}

export function getUnitById(id: number): UnitDef {
  ensureUnitCaches();
  const unit = unitsByIdCache!.get(id);
  if (!unit) throw new Error(`unknown unit id ${id}`);
  return unit;
}

// ---------------------------------------------------------------------------
// Abilities
// ---------------------------------------------------------------------------

let abilitiesCache: readonly AbilityDef[] | undefined;

/** All 34 ability definitions, ordered as in game_data/ability.json. */
export function loadAbilities(): readonly AbilityDef[] {
  if (abilitiesCache) return abilitiesCache;
  abilitiesCache = rows(rawAbilities).map((row) => {
    const ability = row as RawAbilityRow;
    return {
      id: num(ability, 'id'),
      name: localize(str(ability, 'key_name')),
      description: localize(str(ability, 'key_description')),
      abilityType: pick(ability, 'ability_type', ABILITY_TYPE_IDS),
      actionPoint: num(ability, 'action_point'),
      handlerId: num(ability, 'handler_id'),
      actionType: num(ability, 'action_type'),
      boostValueA: num(ability, 'action_boost_value_a'),
      boostValueB: num(ability, 'action_boost_value_b'),
      targetGroup: num(ability, 'target_group'),
      limitOnePerBattle: num(ability, 'limit_one_per_battle') !== 0,
      limitOnePerRound: num(ability, 'limit_one_per_round') !== 0,
    };
  });
  return abilitiesCache;
}

// ---------------------------------------------------------------------------
// Unit specials
// ---------------------------------------------------------------------------

let specialsCache: readonly UnitSpecialDef[] | undefined;

/** All 20 unit special definitions (special die face effects). */
export function loadUnitSpecials(): readonly UnitSpecialDef[] {
  if (specialsCache) return specialsCache;
  specialsCache = rows(rawSpecials).map((row) => {
    const special = row as RawSpecialRow;
    return {
      id: num(special, 'id'),
      name: localize(str(special, 'key_name')),
      relatedActionId: num(special, 'related_action_id'),
      handlerId: num(special, 'handler_id'),
      executionOrder: num(special, 'execution_order'),
    };
  });
  return specialsCache;
}
