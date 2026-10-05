import type { DieFace } from '../types';
import { DIE_FACE_TYPE } from '../types';

/**
 * Damage a rolled face inflicts when its owner attacks this round.
 * TODO: ARMOUR_PIERCING presumably ignores armour in the real game; the MVP
 * model has no armour stat, so it is treated as plain damage for now.
 * INITIATIVE and SPECIAL faces do not attack.
 */
export function faceDamage(face: DieFace): number {
  switch (face.type) {
    case DIE_FACE_TYPE.DAMAGE:
    case DIE_FACE_TYPE.ARMOUR_PIERCING:
      return face.value;
    case DIE_FACE_TYPE.INITIATIVE:
    case DIE_FACE_TYPE.SPECIAL:
      return 0;
  }
}

/** Applies damage, clamped at 0 hp (no overkill bleed in the MVP). */
export function applyDamage(hp: number, amount: number): number {
  return Math.max(0, hp - amount);
}
