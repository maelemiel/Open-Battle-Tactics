import type { DieFace } from '../types';
import { DIE_FACE_TYPE } from '../types';
import { getUnitById } from '../data/loader';

import type { BattleEvent, Side } from './events';
import { computeInitiativeOrder } from './initiative';
import type { InitiativeContender } from './initiative';
import { applyDamage, faceDamage } from './damage';
import { mulberry32, nextInt } from './rng';
import type {
  Battle,
  BattleConfig,
  BattleState,
  BattleStep,
  Combatant,
  MutableCombatant,
} from './state';
import { GAME_PHASE, ROUND_PHASE_ORDER, spawn } from './state';

// Public API surface: import everything from './engine'.
export type { Battle, BattleConfig, BattleState, BattleStep, Combatant, GamePhase } from './state';
export { GAME_PHASE, ROUND_PHASE_ORDER, statsForLevel } from './state';

export function createBattle(config: BattleConfig): Battle {
  // Player listed first = stable order for initiative ties.
  const combatants: MutableCombatant[] = [
    spawn(getUnitById(config.playerUnitId), config.playerLevel, 'player'),
    spawn(getUnitById(config.enemyUnitId), config.enemyLevel, 'enemy'),
  ];
  const rng = mulberry32(config.seed);
  let round = 0;
  let step: BattleStep = 'awaiting-roll';
  let winner: Side | null = null;
  let snapshot = publishState();

  function assertStep(expected: BattleStep, caller: string): void {
    if (winner !== null) throw new Error(`${caller}(): battle is over (winner=${winner})`);
    if (step !== expected) {
      throw new Error(`${caller}(): invalid step '${step}', expected '${expected}'`);
    }
  }

  function viewOf(c: MutableCombatant): Combatant {
    return Object.freeze({
      side: c.side,
      unitId: c.unitId,
      name: c.name,
      maxHp: c.maxHp,
      hp: c.hp,
      alive: c.alive,
      currentFace: c.currentFace,
      faceIndex: c.faceIndex,
    });
  }

  /** Builds a fresh frozen snapshot; a new reference per mutation. */
  function publishState(): BattleState {
    return Object.freeze({
      round,
      step,
      phase: GAME_PHASE.ATTACK,
      winner,
      combatants: Object.freeze(combatants.map(viewOf)),
    });
  }

  function rollPhase(): BattleEvent[] {
    assertStep('awaiting-roll', 'rollPhase');
    round += 1;
    const events: BattleEvent[] = [];
    for (const c of combatants) {
      if (!c.alive) continue;
      const index = nextInt(rng, c.stats.faces.length);
      c.faceIndex = index;
      c.currentFace = c.stats.faces[index]!;
      events.push({
        type: 'unit-rolled',
        round,
        side: c.side,
        unitId: c.unitId,
        faceIndex: index,
        face: c.currentFace,
      });
    }
    step = 'awaiting-resolve';
    snapshot = publishState();
    return events;
  }

  function resolveRound(): BattleEvent[] {
    assertStep('awaiting-resolve', 'resolveRound');
    const events: BattleEvent[] = [];
    const contenders: InitiativeContender[] = combatants
      .filter(
        (c): c is MutableCombatant & { currentFace: DieFace } =>
          c.alive && c.currentFace !== null,
      )
      .map((c) => ({ side: c.side, unitId: c.unitId, face: c.currentFace }));
    const order = computeInitiativeOrder(contenders);
    events.push({ type: 'initiative-determined', round, order });
    for (const phase of ROUND_PHASE_ORDER) {
      // Only ATTACK is implemented; other phases are TODO no-ops.
      if (phase !== GAME_PHASE.ATTACK) continue;
      resolveAttacks(order, events);
    }
    checkVictory(events);
    step = winner !== null ? 'over' : 'awaiting-roll';
    snapshot = publishState();
    return events;
  }

  function resolveAttacks(
    order: readonly InitiativeContender[],
    events: BattleEvent[],
  ): void {
    for (const ref of order) {
      if (winner !== null) return; // no event after battle-won
      const attacker = combatants.find(
        (c) => c.unitId === ref.unitId && c.side === ref.side,
      );
      if (!attacker || !attacker.alive || attacker.currentFace === null) continue;
      const face = attacker.currentFace;
      if (face.type === DIE_FACE_TYPE.SPECIAL) {
        // SPECIAL: recognized and logged, effect stubbed for the MVP.
        // TODO: route attacker.stats.specialId through unit_special handlers.
        events.push({
          type: 'special-triggered',
          round,
          side: attacker.side,
          unitId: attacker.unitId,
          specialId: attacker.stats.specialId,
        });
        continue;
      }
      const amount = faceDamage(face);
      if (amount <= 0) continue; // INITIATIVE faces do not attack this round
      const target = combatants.find((c) => c.side !== attacker.side && c.alive);
      if (!target) continue;
      target.hp = applyDamage(target.hp, amount);
      events.push({
        type: 'damage-dealt',
        round,
        source: attacker.side,
        sourceUnitId: attacker.unitId,
        target: target.side,
        targetUnitId: target.unitId,
        amount,
        targetHpAfter: target.hp,
      });
      if (target.hp <= 0) {
        target.alive = false;
        events.push({
          type: 'unit-defeated',
          round,
          side: target.side,
          unitId: target.unitId,
        });
        checkVictory(events);
      }
    }
  }

  function checkVictory(events: BattleEvent[]): void {
    if (winner !== null) return;
    const playerAlive = combatants.some((c) => c.side === 'player' && c.alive);
    const enemyAlive = combatants.some((c) => c.side === 'enemy' && c.alive);
    // Draw impossible with sequential attacks; revisit for team battles.
    if (playerAlive && !enemyAlive) winner = 'player';
    else if (!playerAlive && enemyAlive) winner = 'enemy';
    else return;
    events.push({ type: 'battle-won', round, winner });
  }

  return {
    get state(): BattleState {
      return snapshot;
    },
    rollPhase,
    resolveRound,
    isOver: (): boolean => winner !== null,
  };
}
