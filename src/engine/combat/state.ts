import type { CombatStats, DieFace, UnitDef } from '../types';

import type { BattleEvent, Side } from './events';
import type { BattleSeeds } from './rng';

/** One unit on a team roster. */
export interface UnitSetup {
  readonly unitId: number;
  readonly level: number;
}

export interface BattleConfig {
  /** Single-unit sugar for `playerUnits` (kept for UI compatibility). */
  readonly playerUnitId: number;
  readonly playerLevel: number;
  /** Single-unit sugar for `enemyUnits` (kept for UI compatibility). */
  readonly enemyUnitId: number;
  readonly enemyLevel: number;
  /** Full rosters (up to 4 units each, original rule). Override the sugar fields when present. */
  readonly playerUnits?: readonly UnitSetup[];
  readonly enemyUnits?: readonly UnitSetup[];
  /**
   * Single seed: deterministically derived into the 3 RNG streams
   * (see `rng.ts:deriveSeeds` for the documented derivation).
   */
  readonly seed?: number;
  /** Explicit 3-stream seeds, as the server sends them (`MatchParser.cs`). Overrides `seed`. */
  readonly seeds?: BattleSeeds;
  /**
   * Host side for initiative ties (coin toss 0 = host). The original reads a
   * `playerIsHost` flag from match data (`BattleController.cs:212`); we
   * default to the player hosting. TODO: wire when match setup exists.
   */
  readonly playerIsHost?: boolean;
}

/** Where the battle stands in the loop: roll (BeginRound) -> resolve (SimulateRound). */
export type BattleStep = 'awaiting-roll' | 'awaiting-resolve' | 'over';

/** Team-level combat state (`ServerTeamState`). */
export interface TeamState {
  readonly side: Side;
  /** Energy, reset to 3 every round (`BattleLogic.cs:139`). Unspent in MVP. */
  readonly energy: number;
  /** Initiative bonus from the ULTRA `intel` ability (`BattleLogic.cs:963`). Always 0 in MVP. */
  readonly intelBoost: number;
}

export interface Combatant {
  readonly side: Side;
  readonly unitId: number;
  readonly name: string;
  readonly maxHp: number;
  /** NOT clamped at 0 on death: overkill keeps hp negative (`BattleLogic.cs:735`). */
  readonly hp: number;
  readonly armor: number;
  readonly alive: boolean;
  /** Face rolled this round; null before the first rollPhase. */
  readonly currentFace: DieFace | null;
  readonly faceIndex: number;
  /** Permanent stacking acid DoT; +1 per acid hit, ticks every round end, never decays. */
  readonly acidPerRound: number;
  /** Cleanse-able DoT (napalm/firebomb). Always 0 in MVP (no source ability). */
  readonly damagePerRound: number;
  /** Reroll lock counter (`roundsUntilRerollEnabled`, `BattleLogic.cs:150-154`). */
  readonly roundsUntilRerollEnabled: number;
  /** True while reroll abilities may not target this unit (`preventReroll`). */
  readonly rollLocked: boolean;
}

export interface BattleState {
  /** 0 until the first rollPhase. */
  readonly round: number;
  readonly step: BattleStep;
  readonly combatants: readonly Combatant[];
  /** Per-team energy/intelBoost, keyed by side. */
  readonly teams: Readonly<Record<Side, TeamState>>;
  readonly winner: Side | null;
}

export interface Battle {
  /** Immutable snapshot; a new reference is published on every mutation. */
  readonly state: BattleState;
  /** BeginRound: rolls every living unit's die via its team's RNG stream. */
  rollPhase(): BattleEvent[];
  /** SimulateRound: initiative, attacks/specials, end-of-round damage, victory. */
  resolveRound(): BattleEvent[];
  isOver(): boolean;
}

/** Internal mutable combatant (never exposed; state snapshots are frozen). */
export interface MutableCombatant {
  side: Side;
  unitId: number;
  name: string;
  maxHp: number;
  hp: number;
  armor: number;
  alive: boolean;
  currentFace: DieFace | null;
  faceIndex: number;
  acidPerRound: number;
  damagePerRound: number;
  roundsUntilRerollEnabled: number;
  preventReroll: boolean;
  /** Consecutive identical roll indices (`rollRepeats`, roll rewards only). */
  rollRepeats: number;
  /** Special ability activated by rolling a SPECIAL face this round. */
  specialActive: boolean;
  stats: CombatStats;
}

/** Internal mutable team state. */
export interface MutableTeamState {
  side: Side;
  energy: number;
  intelBoost: number;
  units: MutableCombatant[];
}

export function statsForLevel(unit: UnitDef, level: number): CombatStats {
  const row = unit.progression.find((s) => s.level === level);
  if (!row) throw new Error(`unit ${unit.id} has no level ${level} progression`);
  return row;
}

export function spawn(unit: UnitDef, level: number, side: Side): MutableCombatant {
  const stats = statsForLevel(unit, level);
  return {
    side,
    unitId: unit.id,
    name: unit.name,
    maxHp: stats.hp,
    hp: stats.hp,
    armor: 0,
    alive: true,
    currentFace: null,
    faceIndex: -1,
    acidPerRound: 0,
    damagePerRound: 0,
    roundsUntilRerollEnabled: 0,
    preventReroll: false,
    rollRepeats: 0,
    specialActive: false,
    stats,
  };
}
