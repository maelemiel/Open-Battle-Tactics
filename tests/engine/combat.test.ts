import { describe, expect, it } from 'vitest';

import { createBattle } from '../../src/engine/combat/engine';
import type { Battle, BattleConfig } from '../../src/engine/combat/engine';
import type {
  BattleEvent,
  DamageDealtEvent,
  InitiativeDeterminedEvent,
  SpecialTriggeredEvent,
  UnitRolledEvent,
} from '../../src/engine/combat/events';
import { mulberry32 } from '../../src/engine/combat/rng';
import { computeInitiativeOrder } from '../../src/engine/combat/initiative';
import { applyDamage } from '../../src/engine/combat/damage';
import { DAMAGE_TYPE, DIE_FACE_TYPE } from '../../src/engine/types';

// VOLT 11001 L1: hp 20, all DAMAGE faces [2,4,6,9,10].
// LONGSHOT 11002 L1: hp 19, all DAMAGE faces [2,4,6,9,10].
// SPEEDSTER 13001 L1: hp 14, faces [D1, D2, D5, I5, I9] (has INITIATIVE).
// POLKA-DoT 21029 L1: hp 25, faces [SPECIAL 0, D5, D8, D10, D13] (HOTSHOT 60600).

const VOLT_VS_LONGSHOT: Omit<BattleConfig, 'seed'> = {
  playerUnitId: 11001,
  playerLevel: 1,
  enemyUnitId: 11002,
  enemyLevel: 1,
};

/** Plays roll/resolve until the battle ends (fails after 1000 rounds). */
function playAll(battle: Battle): BattleEvent[] {
  const events: BattleEvent[] = [];
  let guard = 0;
  while (!battle.isOver()) {
    events.push(...battle.rollPhase());
    events.push(...battle.resolveRound());
    if (++guard > 1000) throw new Error('battle did not terminate');
  }
  return events;
}

function rollEvents(events: readonly BattleEvent[]): readonly UnitRolledEvent[] {
  return events.filter((e): e is UnitRolledEvent => e.type === 'unit-rolled');
}

function damageEvents(events: readonly BattleEvent[]): readonly DamageDealtEvent[] {
  return events.filter((e): e is DamageDealtEvent => e.type === 'damage-dealt');
}

/** Searches seeds whose FIRST rollPhase satisfies the predicate. */
function findSeed(
  config: Omit<BattleConfig, 'seed'>,
  predicate: (rolls: readonly UnitRolledEvent[]) => boolean,
  max = 5000,
): number {
  for (let seed = 1; seed <= max; seed++) {
    const battle = createBattle({ ...config, seed });
    if (predicate(rollEvents(battle.rollPhase()))) return seed;
  }
  throw new Error('findSeed: no seed matched the predicate');
}

describe('seeded rng', () => {
  it('replays the exact same sequence for the same seed', () => {
    const a = mulberry32(42);
    const b = mulberry32(42);
    const seqA = Array.from({ length: 50 }, () => a.next());
    const seqB = Array.from({ length: 50 }, () => b.next());
    expect(seqA).toEqual(seqB);
    expect(new Set(seqA).size).toBeGreaterThan(1);
  });

  it('differs between seeds', () => {
    const a = Array.from({ length: 50 }, () => mulberry32(1).next());
    const b = Array.from({ length: 50 }, () => mulberry32(2).next());
    expect(a).not.toEqual(b);
  });
});

describe('battle determinism', () => {
  it('same seed replays an identical full battle (events + state)', () => {
    const battleA = createBattle({ ...VOLT_VS_LONGSHOT, seed: 123 });
    const battleB = createBattle({ ...VOLT_VS_LONGSHOT, seed: 123 });
    const eventsA = playAll(battleA);
    const eventsB = playAll(battleB);
    expect(eventsA).toEqual(eventsB);
    expect(battleA.state).toEqual(battleB.state);
    expect(battleA.state).not.toBe(battleB.state);
  });

  it('state gets a new reference on each mutation and stays fresh', () => {
    const battle = createBattle({ ...VOLT_VS_LONGSHOT, seed: 7 });
    const s0 = battle.state;
    expect(s0.round).toBe(0);
    expect(s0.combatants.every((c) => c.currentFace === null)).toBe(true);
    battle.rollPhase();
    const s1 = battle.state;
    expect(s1).not.toBe(s0);
    expect(s1.round).toBe(1);
    expect(s1.combatants.every((c) => c.currentFace !== null)).toBe(true);
    battle.resolveRound();
    expect(battle.state).not.toBe(s1);
    expect(() => {
      // Frozen snapshot: silent no-op in sloppy mode, throw in ESM strict.
      (battle.state as { round: number }).round = 99;
    }).toThrow();
    expect(battle.state.round).not.toBe(99);
  });
});

describe('VOLT vs LONGSHOT full battle', () => {
  const battle = createBattle({ ...VOLT_VS_LONGSHOT, seed: 2026 });
  const events = playAll(battle);
  const state = battle.state;

  it('ends with a winner and sane hp', () => {
    expect(state.winner).not.toBeNull();
    expect(battle.isOver()).toBe(true);
    const winnerUnit = state.combatants.find((c) => c.side === state.winner)!;
    const loserUnit = state.combatants.find((c) => c.side !== state.winner)!;
    expect(winnerUnit.alive).toBe(true);
    expect(winnerUnit.hp).toBeGreaterThan(0);
    expect(loserUnit.alive).toBe(false);
    expect(loserUnit.hp).toBe(0); // exact kill, never negative
    for (const c of state.combatants) {
      expect(Number.isFinite(c.hp)).toBe(true);
      expect(c.hp).toBeGreaterThanOrEqual(0);
      expect(c.hp).toBeLessThanOrEqual(c.maxHp);
    }
  });

  it('has no event after battle-won and ends on it', () => {
    const last = events[events.length - 1]!;
    expect(last.type).toBe('battle-won');
    const wonIndex = events.findIndex((e) => e.type === 'battle-won');
    expect(events.filter((e) => e.type === 'battle-won')).toHaveLength(1);
    expect(wonIndex).toBe(events.length - 1);
  });

  it('throws when the battle continues after victory', () => {
    expect(() => battle.rollPhase()).toThrow(/battle is over/);
    expect(() => battle.resolveRound()).toThrow(/battle is over/);
  });

  it('deals exactly face.value damage with a consistent hp ledger', () => {
    const hp: Record<string, number> = { 'player:11001': 20, 'enemy:11002': 19 };
    const rolled = new Map<string, UnitRolledEvent>();
    for (const e of events) {
      if (e.type === 'unit-rolled') rolled.set(`${e.side}:${e.unitId}#${e.round}`, e);
      if (e.type !== 'damage-dealt') continue;
      expect(Number.isFinite(e.amount)).toBe(true);
      expect(e.amount).toBeGreaterThan(0);
      const roll = rolled.get(`${e.source}:${e.sourceUnitId}#${e.round}`);
      expect(roll, 'attacker rolled this round before damaging').toBeDefined();
      expect(roll!.face.type).toBe(DIE_FACE_TYPE.DAMAGE);
      expect(e.amount).toBe(roll!.face.value); // exact face damage
      const key = `${e.target}:${e.targetUnitId}`;
      expect(e.targetHpAfter).toBe(Math.max(0, hp[key]! - e.amount));
      hp[key] = e.targetHpAfter;
    }
    for (const e of events) {
      if (e.type === 'damage-dealt') expect(e.targetHpAfter).toBeGreaterThanOrEqual(0);
    }
  });
});

describe('initiative rules (BattleLogic.CalculateInitiativeWinner)', () => {
  const voltFace = (value: number) => ({ type: DIE_FACE_TYPE.DAMAGE, value });
  const initFace = (value: number) => ({ type: DIE_FACE_TYPE.INITIATIVE, value });

  it('INITIATIVE face beats a DAMAGE face regardless of damage value', () => {
    const order = computeInitiativeOrder([
      { side: 'enemy', unitId: 2, face: voltFace(10) },
      { side: 'player', unitId: 1, face: initFace(5) },
    ]);
    expect(order.map((c) => c.unitId)).toEqual([1, 2]);
  });

  it('both INITIATIVE: higher value first', () => {
    expect(
      computeInitiativeOrder([
        { side: 'player', unitId: 1, face: initFace(5) },
        { side: 'enemy', unitId: 2, face: initFace(9) },
      ]).map((c) => c.unitId),
    ).toEqual([2, 1]);
  });

  it('neither rolls INITIATIVE: 50/50 tie-break determines order', () => {
    const contenders = [
      { side: 'enemy' as const, unitId: 2, face: voltFace(10) },
      { side: 'player' as const, unitId: 1, face: voltFace(2) },
    ];
    // tieBreaker = 0 -> player first
    expect(computeInitiativeOrder(contenders, () => 0).map((c) => c.unitId)).toEqual([1, 2]);
    // tieBreaker = 1 -> enemy first
    expect(computeInitiativeOrder(contenders, () => 1).map((c) => c.unitId)).toEqual([2, 1]);
  });

  it('INITIATIVE face strikes first and ALSO attacks with its face value', () => {
    const config = { playerUnitId: 13001, playerLevel: 1, enemyUnitId: 11001, enemyLevel: 1 };
    const seed = findSeed(config, (rolls) => rolls[0]!.face.type === DIE_FACE_TYPE.INITIATIVE);
    const battle = createBattle({ ...config, seed });
    battle.rollPhase();
    const events = battle.resolveRound();
    const order = events.filter(
      (e): e is InitiativeDeterminedEvent => e.type === 'initiative-determined',
    );
    expect(order[0]?.order[0]?.side).toBe('player');
    const damages = damageEvents(events);
    // Player with INITIATIVE face attacks first for its face value
    expect(damages.length).toBeGreaterThanOrEqual(1);
    expect(damages[0]!.source).toBe('player');
  });
});

describe('damage and armor formulas (BattleLogic.ApplyDamage)', () => {
  it('absorbs damage using armor before reducing hp', () => {
    // 6 damage against 20 HP with 4 armor -> 4 absorbed, 2 to HP -> 18 HP, 0 armor
    const result = applyDamage(20, 6, 4, DAMAGE_TYPE.STANDARD);
    expect(result.finalHp).toBe(18);
    expect(result.finalArmor).toBe(0);
    expect(result.absorbed).toBe(4);
  });

  it('partially absorbs damage when armor exceeds damage', () => {
    // 3 damage against 20 HP with 10 armor -> 3 absorbed, 0 to HP -> 20 HP, 7 armor
    const result = applyDamage(20, 3, 10, DAMAGE_TYPE.STANDARD);
    expect(result.finalHp).toBe(20);
    expect(result.finalArmor).toBe(7);
    expect(result.absorbed).toBe(3);
  });

  it('armour piercing ignores armor completely', () => {
    // 6 AP damage against 20 HP with 10 armor -> ignores armor -> 14 HP, 0 armor
    const result = applyDamage(20, 6, 10, DAMAGE_TYPE.ARMOUR_PIERCING);
    expect(result.finalHp).toBe(14);
    expect(result.finalArmor).toBe(0);
    expect(result.absorbed).toBe(0);
  });
});

describe('special faces (MVP stub)', () => {
  it('SPECIAL face emits special-triggered with the unit specialId', () => {
    const config = { playerUnitId: 21029, playerLevel: 1, enemyUnitId: 11001, enemyLevel: 1 };
    const seed = findSeed(config, (rolls) => rolls[0]!.face.type === DIE_FACE_TYPE.SPECIAL);
    const battle = createBattle({ ...config, seed });
    battle.rollPhase();
    const events = battle.resolveRound();
    const specials = events.filter(
      (e): e is SpecialTriggeredEvent => e.type === 'special-triggered',
    );
    expect(specials).toHaveLength(1);
    expect(specials[0]?.specialId).toBe(60600);
    const damages = damageEvents(events);
    expect(damages.every((d) => d.source === 'enemy')).toBe(true);
  });
});

describe('step guards', () => {
  it('rejects resolveRound before rollPhase', () => {
    const battle = createBattle({ ...VOLT_VS_LONGSHOT, seed: 1 });
    expect(() => battle.resolveRound()).toThrow(/invalid step/);
    expect(() => battle.rollPhase()).not.toThrow();
    expect(() => battle.rollPhase()).toThrow(/invalid step/); // roll twice in a row
  });
});
