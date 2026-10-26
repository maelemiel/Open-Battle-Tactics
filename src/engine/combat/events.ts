import type { DieFace } from '../types';

/** Which side of the battlefield an actor or event belongs to. */
export type Side = 'player' | 'enemy';

export interface UnitRolledEvent {
  readonly type: 'unit-rolled';
  readonly round: number;
  readonly side: Side;
  readonly unitId: number;
  /** Index (0-4) into the unit's 5 faces, for UI highlight. */
  readonly faceIndex: number;
  readonly face: DieFace;
}

export interface InitiativeDeterminedEvent {
  readonly type: 'initiative-determined';
  readonly round: number;
  /** Strike order for this round, first attacker first. */
  readonly order: readonly { side: Side; unitId: number }[];
}

/**
 * Initiative tie (including 0-0) was broken by the battle RNG coin toss
 * (`BattleLogic.cs:281`, `GetNextBattleRandom(battle, 0, 2)`).
 * toss 0 = host wins (player), 1 = guest wins (enemy).
 */
export interface InitiativeCointossEvent {
  readonly type: 'initiative-cointoss';
  readonly round: number;
  readonly toss: 0 | 1;
  readonly winner: Side;
}

export interface DamageDealtEvent {
  readonly type: 'damage-dealt';
  readonly round: number;
  readonly source: Side;
  readonly sourceUnitId: number;
  readonly target: Side;
  readonly targetUnitId: number;
  /** Pre-absorption damage value (matches the C# `ApplyDamage` return). */
  readonly amount: number;
  /** HP after the hit. NOT clamped at 0: overkill goes negative by design. */
  readonly targetHpAfter: number;
}

/** Armor absorbed part of an attack (`BattleLogic.cs:727-729`). */
export interface ArmorAbsorbedEvent {
  readonly type: 'armor-absorbed';
  readonly round: number;
  readonly side: Side;
  readonly unitId: number;
  readonly absorbed: number;
  readonly armorRemaining: number;
}

/** End-of-round permanent acid DoT tick (`BattleLogic.cs:556-559`, stacks never decay). */
export interface AcidTickedEvent {
  readonly type: 'acid-ticked';
  readonly round: number;
  readonly side: Side;
  readonly unitId: number;
  readonly amount: number;
  readonly acidPerRound: number;
  readonly targetHpAfter: number;
}

/** End-of-round `damagePerRound` tick (firebomb/napalm residue; MVP: never fires). */
export interface DotTickedEvent {
  readonly type: 'dot-ticked';
  readonly round: number;
  readonly side: Side;
  readonly unitId: number;
  readonly amount: number;
  readonly damagePerRound: number;
  readonly targetHpAfter: number;
}

/**
 * A SPECIAL face fired at this unit's place in the attack sequence.
 * Kept from v1 for UI compatibility; every SPECIAL face emits it.
 */
export interface SpecialTriggeredEvent {
  readonly type: 'special-triggered';
  readonly round: number;
  readonly side: Side;
  readonly unitId: number;
  readonly specialId: number;
}

/**
 * A unit special EXECUTED its real effect, replacing the normal attack
 * (`AbilityHooks.UnitFiringEvent` returning true). MVP: only SHORT CIRCUIT
 * (60800). Fields beyond specialId/handler are effect-specific.
 */
export interface SpecialExecutedEvent {
  readonly type: 'special-executed';
  readonly round: number;
  readonly side: Side;
  readonly unitId: number;
  readonly specialId: number;
  /** Handler string from `unit_special_handler` (e.g. 'shortcircuit'). */
  readonly handler: string;
  /** True when the effect is implemented; false = documented stub. */
  readonly implemented: boolean;
  /** SHORT CIRCUIT: damage dealt to the random enemy. */
  readonly damage?: number;
  /** SHORT CIRCUIT: enemy unit that took the hit + reroll lock. */
  readonly targetUnitId?: number;
}

export interface UnitDefeatedEvent {
  readonly type: 'unit-defeated';
  readonly round: number;
  readonly side: Side;
  readonly unitId: number;
}

export interface BattleWonEvent {
  readonly type: 'battle-won';
  readonly round: number;
  readonly winner: Side;
}

/** Discriminated union the UI animates from. Never side effects. */
export type BattleEvent =
  | UnitRolledEvent
  | InitiativeDeterminedEvent
  | InitiativeCointossEvent
  | DamageDealtEvent
  | ArmorAbsorbedEvent
  | AcidTickedEvent
  | DotTickedEvent
  | SpecialTriggeredEvent
  | SpecialExecutedEvent
  | UnitDefeatedEvent
  | BattleWonEvent;
