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

export interface DamageDealtEvent {
  readonly type: 'damage-dealt';
  readonly round: number;
  readonly source: Side;
  readonly sourceUnitId: number;
  readonly target: Side;
  readonly targetUnitId: number;
  readonly amount: number;
  readonly targetHpAfter: number;
}

/**
 * A SPECIAL face was rolled. Recognized and logged only for the MVP.
 * TODO: route specialId through unit_special handlers (real effects).
 */
export interface SpecialTriggeredEvent {
  readonly type: 'special-triggered';
  readonly round: number;
  readonly side: Side;
  readonly unitId: number;
  readonly specialId: number;
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
  | DamageDealtEvent
  | SpecialTriggeredEvent
  | UnitDefeatedEvent
  | BattleWonEvent;
