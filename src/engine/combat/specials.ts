import { getSpecialHandlerName, loadUnitSpecials } from '../data/loader';
import type { UnitSpecialDef } from '../types';

import type { MutableCombatant } from './state';

/**
 * Unit SPECIAL effects, resolved at the unit's slot in the attack sequence
 * (`AbilityHooks.UnitFiringEvent`): a special that returns true REPLACES the
 * normal attack. Activation happens at ROLL time (`BattleLogic.cs:
 * SetUnitCurrentRoll` 661-691): rolling a Special face auto-activates the
 * unit's special (cost 0, target = the rolling unit).
 *
 * MVP: SHORT CIRCUIT (unit_special 60800, handler 608 'shortcircuit') is
 * implemented for real (`ShortCircuitAbility.cs`). Every other handler is
 * resolved as a documented stub (event emitted, no effect).
 */

let specialsById: Map<number, UnitSpecialDef> | undefined;

/** `unit_special` row for a specialId (0 -> null). */
export function getSpecialDef(specialId: number): UnitSpecialDef | null {
  if (specialId === 0) return null;
  if (!specialsById) {
    specialsById = new Map(loadUnitSpecials().map((s) => [s.id, s]));
  }
  const def = specialsById.get(specialId);
  if (!def) throw new Error(`unknown unit_special id ${specialId}`);
  return def;
}

export interface SpecialResolution {
  /** Handler string (`unit_special_handler.handler`). */
  readonly handler: string;
  /** True = real effect below; False = documented stub, no effect. */
  readonly implemented: boolean;
  /**
   * SHORT CIRCUIT: `floor(boostMultiplier * BoostValue)` damage
   * (`ShortCircuitAbility.cs:UnitFiringEvent`). `boostMultiplier` defaults to
   * 1 (`ServerAbilityState.boostMultiplier = 1f`); the raid-boss boost table
   * (`BattleLogic.cs:InitAbilityUnitBoosts`, Multiplier2) is the extension
   * point — no boosts in MVP.
   */
  readonly damage: number;
  /** SHORT CIRCUIT: damage target's reroll gets locked (`roundsUntilRerollEnabled++`). */
  readonly locksTargetReroll: boolean;
}

/**
 * Resolves what a unit's active special does this round. Returns null when
 * the unit has no active special (did not roll a SPECIAL face, has no
 * `special_id`, or its handler is the no-op 60908 `unit_event_boost`).
 */
export function resolveSpecial(unit: MutableCombatant): SpecialResolution | null {
  if (!unit.specialActive || unit.alive === false) return null;
  const def = getSpecialDef(unit.stats.specialId);
  if (!def) return null;

  const handler = getSpecialHandlerName(def.handlerId);
  if (handler === 'shortcircuit') {
    return {
      handler,
      implemented: true,
      damage: Math.floor(unit.stats.specialBoostA),
      locksTargetReroll: true,
    };
  }

  // Documented stubs (COMBAT_SPEC.md §5 behaviors, none implemented in MVP):
  //  aoe_damage        AOE floor(mult*A) to enemy team
  //  rail_gun          alternating deploy/fire, floor(mult*A) to random enemy
  //  hotshot           floor(mult*A) to random enemy + damagePerRound += B
  //  napalm            AOE floor(mult*A) + damagePerRound += B to enemy team
  //  minigun           floor(mult*A) + escalating extra shots of B
  //  increase_base_attack / first_strike_buff / boost_dice* / armour_buff /
  //  passive_evade / quick_draw_passive / extinguish_unit / *_passive:
  //  roll/defense modifiers — no UnitFiring replacement effect to model yet.
  return { handler, implemented: false, damage: 0, locksTargetReroll: false };
}
