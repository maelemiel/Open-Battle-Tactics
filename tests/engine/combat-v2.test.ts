/**
 * Combat engine v2 — rules verified against the decompiled original
 * (docs/COMBAT_SPEC.md). Covers the three-stream MicrosoftRNG, the face-type
 * map, initiative coin toss, SHORT CIRCUIT, the reroll lock and determinism.
 */
import { describe, expect, it } from 'vitest';

import { createBattle } from '../../src/engine/combat/engine';
import type { Battle, BattleConfig } from '../../src/engine/combat/engine';
import { deriveSeeds, MicrosoftRNG } from '../../src/engine/combat/rng';
import type { BattleEvent, SpecialExecutedEvent, UnitRolledEvent } from '../../src/engine/combat/events';
import { loadUnits, getUnitById, loadUnitSpecialHandlers } from '../../src/engine/data/loader';
import { DIE_FACE_TYPE, FACE_TYPE_ID_MAP } from '../../src/engine/types';

// ---------------------------------------------------------------------------
// MicrosoftRNG — MSVC rand() LCG, byte-exact vectors
// ---------------------------------------------------------------------------

describe('MicrosoftRNG (MicrosoftRNG.cs: seed*214013+2531011, (seed/65536)%max)', () => {
  it('matches the canonical MSVC rand() sequence for srand(1) at max=32768', () => {
    // x % 32768 == x & 0x7fff, so Next(32768) reproduces rand() exactly.
    const rng = new MicrosoftRNG(1);
    const expected = [41, 18467, 6334, 26500, 19169, 15724, 11478, 29358, 26962, 24464];
    expect(Array.from({ length: 10 }, () => rng.next(32768))).toEqual(expected);
  });

  it('applies % max directly (no 0x7fff mask) — computed reference vectors', () => {
    const rng5 = new MicrosoftRNG(1);
    expect(Array.from({ length: 8 }, () => rng5.next(5))).toEqual([1, 1, 2, 4, 2, 3, 1, 2]);
    const rng2 = new MicrosoftRNG(1);
    expect(Array.from({ length: 8 }, () => rng2.next(2))).toEqual([1, 1, 0, 0, 1, 0, 0, 0]);
  });

  it('stays within [0, max) and covers the full range', () => {
    const rng = new MicrosoftRNG(987654321);
    const seen = new Set<number>();
    for (let i = 0; i < 500; i++) {
      const v = rng.next(5);
      expect(v).toBeGreaterThanOrEqual(0);
      expect(v).toBeLessThan(5);
      seen.add(v);
    }
    expect(seen.size).toBe(5);
  });

  it('advances the stream even when max === 1 (single-target draws)', () => {
    const a = new MicrosoftRNG(7);
    expect(a.next(1)).toBe(0);
    expect(a.generatedNumbers).toBe(1);
    const c = new MicrosoftRNG(7);
    c.next(5); // first draw of a fresh stream
    const cSecond = c.next(5);
    expect(a.next(5)).toBe(cSecond); // a is one draw ahead thanks to the max=1 call
  });

  it('nextRange mirrors GetNextBattleRandom/GetNextTeamRandom (from==to does not advance)', () => {
    const rng = new MicrosoftRNG(3);
    expect(rng.nextRange(5, 5)).toBe(5);
    expect(rng.generatedNumbers).toBe(0); // early return, no draw
    const first = rng.nextRange(2, 6); // 2 + Next(4)
    expect(first).toBeGreaterThanOrEqual(2);
    expect(first).toBeLessThan(6);
    expect(rng.generatedNumbers).toBe(1);
  });

  it('same seed -> same sequence; different seed -> different sequence', () => {
    const a = Array.from({ length: 20 }, () => new MicrosoftRNG(1234).next(100));
    const b = Array.from({ length: 20 }, () => new MicrosoftRNG(1234).next(100));
    const c = Array.from({ length: 20 }, () => new MicrosoftRNG(1235).next(100));
    expect(a).toEqual(b);
    expect(a).not.toEqual(c);
  });

  it('rejects max <= 0', () => {
    expect(() => new MicrosoftRNG(1).next(0)).toThrow();
  });
});

describe('three-stream seed derivation (rng.ts deriveSeeds)', () => {
  it('derives documented deterministic seeds: battle=s, player=2s+1, enemy=2s+2', () => {
    expect(deriveSeeds(1000)).toEqual({ battle: 1000, player: 2001, enemy: 2002 });
    expect(deriveSeeds(0)).toEqual({ battle: 0, player: 1, enemy: 2 });
    // wraps mod 2^32
    expect(deriveSeeds(0xffffffff)).toEqual({
      battle: 0xffffffff,
      player: 0xffffffff,
      enemy: 0,
    });
  });

  it('player and enemy streams ALWAYS differ (odd vs even)', () => {
    for (const s of [0, 1, 42, 999, 2 ** 31, 2 ** 32 - 1]) {
      const seeds = deriveSeeds(s);
      expect(seeds.player).not.toBe(seeds.enemy);
    }
  });

  it('explicit seeds override derivation and replay identically', () => {
    const base: Omit<BattleConfig, 'seed' | 'seeds'> = {
      playerUnitId: 11001,
      playerLevel: 1,
      enemyUnitId: 11002,
      enemyLevel: 1,
    };
    const run = (): BattleEvent[] => {
      const battle: Battle = createBattle({
        ...base,
        seeds: { battle: 5, player: 6, enemy: 7 },
      });
      const events: BattleEvent[] = [];
      while (!battle.isOver()) {
        events.push(...battle.rollPhase(), ...battle.resolveRound());
      }
      return events;
    };
    expect(run()).toEqual(run());
  });
});

// ---------------------------------------------------------------------------
// DieFaceType mapping (UnitLevelProgressionDataModel.cs:70-104 faceTypeMap)
// ---------------------------------------------------------------------------

describe('die face type mapping', () => {
  it('maps every raw id exactly like the original faceTypeMap', () => {
    expect(FACE_TYPE_ID_MAP[10000]).toBe(DIE_FACE_TYPE.NONE);
    expect(FACE_TYPE_ID_MAP[20000]).toBe(DIE_FACE_TYPE.DAMAGE);
    expect(FACE_TYPE_ID_MAP[30000]).toBe(DIE_FACE_TYPE.SPECIAL);
    expect(FACE_TYPE_ID_MAP[40000]).toBe(DIE_FACE_TYPE.INITIATIVE);
    expect(FACE_TYPE_ID_MAP[50000]).toBe(DIE_FACE_TYPE.SPECIAL);
    expect(FACE_TYPE_ID_MAP[60000]).toBe(DIE_FACE_TYPE.SPECIAL);
    expect(FACE_TYPE_ID_MAP[70000]).toBe(DIE_FACE_TYPE.ARMOUR_PIERCING);
    expect(FACE_TYPE_ID_MAP[80000]).toBe(DIE_FACE_TYPE.ACID_STRIKE);
    // 30000, 50000, 60000 are ALL Special.
    expect(new Set([30000, 50000, 60000].map((id) => FACE_TYPE_ID_MAP[id]))).toEqual(
      new Set([DIE_FACE_TYPE.SPECIAL]),
    );
  });

  it('loader (real DB) only emits canonical face types, with the exact known distribution', () => {
    const canonical = new Set<number>(Object.values(DIE_FACE_TYPE));
    const counts = new Map<number, number>();
    for (const unit of loadUnits()) {
      for (const level of unit.progression) {
        for (const face of level.faces) {
          expect(canonical.has(face.type)).toBe(true); // normalized, no 30000/50000 leaks
          counts.set(face.type, (counts.get(face.type) ?? 0) + 1);
        }
      }
    }
    // Verified against game_data/unit_level_progression.json directly.
    expect(Object.fromEntries(counts)).toEqual({
      [DIE_FACE_TYPE.DAMAGE]: 10161,
      [DIE_FACE_TYPE.INITIATIVE]: 3981,
      [DIE_FACE_TYPE.SPECIAL]: 2178,
      [DIE_FACE_TYPE.ARMOUR_PIERCING]: 135,
    });
  });

  it('loads special boost values (VOLT L6 SHORT CIRCUIT boostA=11)', () => {
    const volt6 = getUnitById(11001).progression.find((s) => s.level === 6)!;
    expect(volt6.specialId).toBe(60800);
    expect(volt6.specialBoostA).toBe(11);
    expect(volt6.specialBoostB).toBe(0);
    // L1 unit carrying SHORT CIRCUIT with boostA=3 (test workhorse).
    const unit = getUnitById(29703).stats;
    expect(unit.specialId).toBe(60800);
    expect(unit.specialBoostA).toBe(3);
  });

  it('loads the 19 handler strings incl. shortcircuit (608)', () => {
    const handlers = loadUnitSpecialHandlers();
    expect(handlers.find((h) => h.id === 608)?.handler).toBe('shortcircuit');
    expect(handlers.find((h) => h.id === 606)?.handler).toBe('hotshot');
    expect(handlers).toHaveLength(19);
  });
});

// ---------------------------------------------------------------------------
// SHORT CIRCUIT (ShortCircuitAbility.cs) + reroll lock semantics
// ---------------------------------------------------------------------------

/**
 * VOLT 11001 L6 (hp 25, faces [D3, D6, D8, D11, SPECIAL], boostA=11) vs
 * 22002 L1 (hp 30, faces [D5, D6, D7, I7, D8], no special): the enemy can
 * never one-shot VOLT (max 8 damage) and survives several SHORT CIRCUITs.
 */
const SC_CONFIG: Omit<BattleConfig, 'seed'> = {
  playerUnitId: 11001,
  playerLevel: 6,
  enemyUnitId: 22002,
  enemyLevel: 1,
};

function rollsOf(battle: Battle): readonly UnitRolledEvent[] {
  return battle.rollPhase().filter((e): e is UnitRolledEvent => e.type === 'unit-rolled');
}

function isSpecial(e: UnitRolledEvent): boolean {
  return e.face.type === DIE_FACE_TYPE.SPECIAL;
}

function findSeed(
  config: Omit<BattleConfig, 'seed'>,
  predicate: (rolls: readonly UnitRolledEvent[]) => boolean,
  max = 5000,
): number {
  for (let seed = 1; seed <= max; seed++) {
    const battle = createBattle({ ...config, seed });
    if (predicate(rollsOf(battle))) return seed;
  }
  throw new Error('findSeed: no matching seed');
}

describe('SHORT CIRCUIT 60800 (VOLT L6 special, handler 608)', () => {
  // Round 1: VOLT rolls its SPECIAL face (enemy roll irrelevant: max 8 dmg).
  const seed = findSeed(SC_CONFIG, (r) => isSpecial(r[0]!));
  const battle = createBattle({ ...SC_CONFIG, seed });
  battle.rollPhase();
  const events = battle.resolveRound();

  it('replaces the attack: floor(multiplier * boostA) = 11 to the random enemy', () => {
    const executed = events.filter(
      (e): e is SpecialExecutedEvent => e.type === 'special-executed',
    );
    expect(executed).toHaveLength(1);
    expect(executed[0]!.side).toBe('player');
    expect(executed[0]!.handler).toBe('shortcircuit');
    expect(executed[0]!.implemented).toBe(true);
    expect(executed[0]!.damage).toBe(11); // floor(1 * special_boost_value_a=11)

    const dealt = events.filter(
      (e): e is Extract<BattleEvent, { type: 'damage-dealt' }> => e.type === 'damage-dealt',
    );
    const scHit = dealt.find((d) => d.source === 'player');
    expect(scHit).toBeDefined();
    expect(scHit!.amount).toBe(11);
    expect(scHit!.targetUnitId).toBe(22002);
    expect(scHit!.targetHpAfter).toBe(30 - 11);
    expect(executed[0]!.targetUnitId).toBe(22002);
  });

  it('locks the victim reroll: roundsUntilRerollEnabled=1, rollLocked=true', () => {
    const victim = battle.state.combatants.find((c) => c.unitId === 22002)!;
    expect(victim.roundsUntilRerollEnabled).toBe(1);
    expect(victim.rollLocked).toBe(true);
  });

  it('lock does NOT freeze the die: BeginRound still rolls (C# isInitialRoll=true), then the lock lapses', () => {
    // Scenario seed: R1 VOLT SPECIAL (lock applied); R2 VOLT non-SPECIAL (no
    // fresh lock); R3 the victim rolls with the lock expired.
    let scenarioSeed = -1;
    for (let s = 1; s <= 5000; s++) {
      const b = createBattle({ ...SC_CONFIG, seed: s });
      const r1 = rollsOf(b);
      if (!isSpecial(r1[0]!)) continue;
      b.resolveRound();
      if (b.isOver()) continue;
      const r2 = rollsOf(b);
      if (isSpecial(r2[0]!)) continue; // fresh SHORT CIRCUIT would re-lock
      b.resolveRound();
      if (b.isOver()) continue;
      scenarioSeed = s;
      break;
    }
    expect(scenarioSeed).toBeGreaterThan(0);

    const b = createBattle({ ...SC_CONFIG, seed: scenarioSeed });
    b.rollPhase();
    b.resolveRound(); // R1: SHORT CIRCUIT, lock applied

    const r2 = rollsOf(b); // R2: locked victim must STILL roll
    expect(r2.find((e) => e.side === 'enemy')).toBeDefined();
    let victim = b.state.combatants.find((c) => c.unitId === 22002)!;
    expect(victim.roundsUntilRerollEnabled).toBe(0); // decremented by BeginRound
    expect(victim.rollLocked).toBe(true); // latched for this round (blocks reroll abilities)
    b.resolveRound();

    rollsOf(b); // R3
    victim = b.state.combatants.find((c) => c.unitId === 22002)!;
    expect(victim.alive).toBe(true);
    expect(victim.rollLocked).toBe(false); // lock expired without a fresh SHORT CIRCUIT
    expect(victim.roundsUntilRerollEnabled).toBe(0);
  });
});

// ---------------------------------------------------------------------------
// Coin toss determinism (findSeed over the battle stream)
// ---------------------------------------------------------------------------

describe('initiative coin toss determinism (BattleLogic.cs:281)', () => {
  const VOLT_VS_LONGSHOT: Omit<BattleConfig, 'seed'> = {
    playerUnitId: 11001,
    playerLevel: 1,
    enemyUnitId: 11002,
    enemyLevel: 1,
  };

  it('0-0 ties resolve 50/50 via the battle stream; findSeed finds both outcomes', () => {
    const firstToss = (seed: number): 0 | 1 => {
      const battle = createBattle({ ...VOLT_VS_LONGSHOT, seed });
      battle.rollPhase();
      const toss = battle.resolveRound().find((e) => e.type === 'initiative-cointoss');
      if (!toss) throw new Error('expected a coin toss (both teams all-DAMAGE faces)');
      return (toss as { toss: 0 | 1 }).toss;
    };
    let playerSeed = -1;
    let enemySeed = -1;
    for (let seed = 1; seed <= 100; seed++) {
      const toss = firstToss(seed);
      if (toss === 0 && playerSeed === -1) playerSeed = seed;
      if (toss === 1 && enemySeed === -1) enemySeed = seed;
      // The toss must equal the battle stream's first draw (deriveSeeds: battle=seed).
      expect(toss).toBe(new MicrosoftRNG(seed).nextRange(0, 2) as 0 | 1);
      if (playerSeed !== -1 && enemySeed !== -1) break;
    }
    expect(playerSeed).toBeGreaterThan(0);
    expect(enemySeed).toBeGreaterThan(0);
  });
});

// ---------------------------------------------------------------------------
// Team battles (rosters) + always-a-winner + determinism
// ---------------------------------------------------------------------------

describe('team battles (2v2 rosters)', () => {
  const config = (seed: number): BattleConfig => ({
    playerUnitId: 11001, // sugar, overridden by playerUnits
    playerLevel: 1,
    enemyUnitId: 11002,
    enemyLevel: 1,
    playerUnits: [
      { unitId: 11001, level: 1 },
      { unitId: 13001, level: 1 },
    ],
    enemyUnits: [
      { unitId: 11002, level: 1 },
      { unitId: 13001, level: 1 },
    ],
    seed,
  });

  it('plays a full 2v2 to a single winner with per-unit rolls and team energy', () => {
    const battle = createBattle(config(77));
    const events: BattleEvent[] = [];
    let guard = 0;
    while (!battle.isOver()) {
      events.push(...battle.rollPhase(), ...battle.resolveRound());
      if (++guard > 1000) throw new Error('2v2 battle did not terminate');
    }
    expect(battle.state.winner).not.toBeNull();
    expect(events[events.length - 1]!.type).toBe('battle-won');
    expect(battle.state.combatants).toHaveLength(4);
    expect(events.filter((e) => e.type === 'unit-rolled' && e.round === 1)).toHaveLength(4);
    // Initiative order always lists the winning side's units first.
    for (const order of events.filter(
      (e): e is Extract<BattleEvent, { type: 'initiative-determined' }> =>
        e.type === 'initiative-determined',
    )) {
      expect(new Set(order.order.slice(0, 2).map((o) => o.side)).size).toBe(1);
    }
  });

  it('determinism holds for 2v2 too', () => {
    const run = (): readonly BattleEvent[] => {
      const battle = createBattle(config(9));
      const events: BattleEvent[] = [];
      while (!battle.isOver()) events.push(...battle.rollPhase(), ...battle.resolveRound());
      return events;
    };
    expect(run()).toEqual(run());
  });
});
