import type { CombatStats, DieFace, UnitDef } from '../types';

import type { BattleEvent, Side } from './events';

export interface BattleConfig {
  readonly playerUnitId: number;
  readonly playerLevel: number;
  readonly enemyUnitId: number;
  readonly enemyLevel: number;
  readonly seed: number;
}

/**
 * Phases of the original game's round, in execution order. Only ATTACK (fed
 * by the initiative ordering) is implemented; the others are placeholders.
 * TODO: implement FIRST_STRIKE / ULTRA / PRE_COMBAT / REACTIVE effects.
 */
export const GAME_PHASE = {
  FIRST_STRIKE: 'first-strike',
  ULTRA: 'ultra',
  PRE_COMBAT: 'pre-combat',
  ATTACK: 'attack',
  REACTIVE: 'reactive',
} as const;

export type GamePhase = (typeof GAME_PHASE)[keyof typeof GAME_PHASE];

/** Round pipeline; skipped phases are the documented no-ops above. */
export const ROUND_PHASE_ORDER: readonly GamePhase[] = [
  GAME_PHASE.FIRST_STRIKE,
  GAME_PHASE.ULTRA,
  GAME_PHASE.PRE_COMBAT,
  GAME_PHASE.ATTACK,
  GAME_PHASE.REACTIVE,
];

/** Where the battle stands in the MVP loop: roll -> resolve -> repeat. */
export type BattleStep = 'awaiting-roll' | 'awaiting-resolve' | 'over';

export interface Combatant {
  readonly side: Side;
  readonly unitId: number;
  readonly name: string;
  readonly maxHp: number;
  readonly hp: number;
  readonly alive: boolean;
  /** Face rolled this round; null before the first rollPhase. */
  readonly currentFace: DieFace | null;
  readonly faceIndex: number;
}

export interface BattleState {
  /** 0 until the first rollPhase. */
  readonly round: number;
  readonly step: BattleStep;
  /** Game phase being resolved; ATTACK for the whole MVP. */
  readonly phase: GamePhase;
  readonly combatants: readonly Combatant[];
  readonly winner: Side | null;
}

export interface Battle {
  /** Immutable snapshot; a new reference is published on every mutation. */
  readonly state: BattleState;
  /** Every living unit rolls its die. */
  rollPhase(): BattleEvent[];
  /** Initiative order + attacks + deaths + victory, as events. */
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
  alive: boolean;
  currentFace: DieFace | null;
  faceIndex: number;
  stats: CombatStats;
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
    alive: true,
    currentFace: null,
    faceIndex: -1,
    stats,
  };
}
