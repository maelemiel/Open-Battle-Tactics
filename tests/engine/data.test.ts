import { describe, expect, it } from 'vitest';

import {
  getUnitById,
  loadAbilities,
  loadUnitSpecials,
  loadUnits,
} from '../../src/engine/data/loader';
import { DIE_FACE_TYPE } from '../../src/engine/types';

describe('unit loader', () => {
  it('loads all 370 units', () => {
    const units = loadUnits();
    expect(units).toHaveLength(370);
    expect(new Set(units.map((u) => u.id)).size).toBe(370);
  });

  it('resolves VOLT (11001) with real combat stats', () => {
    const volt = getUnitById(11001);
    expect(volt.name).toBe('VOLT');
    expect(volt.typeId).toBe(1);
    expect(volt.rarityId).toBe(1);
    // Verified against DB: hp 20, faces [2,4,6,9,10] all DAMAGE (prototype wheelValues).
    expect(volt.stats.hp).toBe(20);
    expect(volt.stats.faces.map((f) => f.value)).toEqual([2, 4, 6, 9, 10]);
    expect(volt.stats.faces.every((f) => f.type === DIE_FACE_TYPE.DAMAGE)).toBe(true);
    expect(volt.stats.specialId).toBe(0);
    expect(volt.progression.length).toBeGreaterThanOrEqual(6);
    // Level 6 swaps face 5 to SPECIAL (SHORT CIRCUIT 60800).
    const level6 = volt.progression.find((s) => s.level === 6);
    expect(level6?.faces[4].type).toBe(DIE_FACE_TYPE.SPECIAL);
    expect(level6?.specialId).toBe(60800);
  });

  it('resolves LONGSHOT (11002) with real combat stats', () => {
    const longshot = getUnitById(11002);
    expect(longshot.name).toBe('LONGSHOT');
    expect(longshot.stats.hp).toBe(19);
    expect(longshot.stats.faces.map((f) => f.value)).toEqual([2, 4, 6, 9, 10]);
    expect(longshot.stats.faces.every((f) => f.value > 0 && f.value < 100)).toBe(true);
  });

  it('gives every unit non-zero hp and 5 faces', () => {
    for (const unit of loadUnits()) {
      expect(unit.stats.hp).toBeGreaterThan(0);
      expect(unit.stats.faces).toHaveLength(5);
      expect(unit.name.length).toBeGreaterThan(0);
    }
  });

  it('throws on unknown unit id', () => {
    expect(() => getUnitById(999999)).toThrow();
  });
});

describe('ability loader', () => {
  it('loads all 34 abilities with localized names', () => {
    const abilities = loadAbilities();
    expect(abilities).toHaveLength(34);
    const respin = abilities.find((a) => a.id === 101000);
    expect(respin?.name).toBe('RE-SPIN');
    expect(respin?.actionPoint).toBe(1);
    expect(respin?.abilityType).toBe(2);
  });
});

describe('unit special loader', () => {
  it('loads all 20 specials', () => {
    expect(loadUnitSpecials()).toHaveLength(20);
  });
});
