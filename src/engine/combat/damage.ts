import type { DamageType, DieFace } from '../types';
import { DAMAGE_TYPE, DIE_FACE_TYPE } from '../types';

/**
 * Damage a rolled face inflicts when its owner attacks this round.
 * Verified from decompiled C# source (`BattleLogic.CanUnitAttack`):
 * - DirectDamage (20000), Initiative (40000), ArmourPiercing (70000), and AcidStrike (80000)
 *   all execute an attack dealing their face roll value.
 * - Special (60000) and None (10000) do not perform standard direct damage.
 */
export function faceDamage(face: DieFace): number {
  switch (face.type) {
    case DIE_FACE_TYPE.DAMAGE:
    case DIE_FACE_TYPE.INITIATIVE:
    case DIE_FACE_TYPE.ARMOUR_PIERCING:
    case DIE_FACE_TYPE.ACID_STRIKE:
      return face.value;
    case DIE_FACE_TYPE.SPECIAL:
    case DIE_FACE_TYPE.NONE:
      return 0;
  }
}

export interface DamageResult {
  readonly finalHp: number;
  readonly finalArmor: number;
  readonly absorbed: number;
}

/**
 * Applies damage to a unit taking armor and damageType into account.
 * Verified from decompiled C# source (`BattleLogic.ApplyDamage`):
 * - If damageType is ArmourPiercing (3): armor is breached (set to 0), damage goes directly to HP.
 * - Otherwise: armor absorbs up to its value (absorbed = min(armor, damage)),
 *   and remaining damage reduces HP.
 */
export function applyDamage(
  hp: number,
  amount: number,
  armor = 0,
  damageType: DamageType = DAMAGE_TYPE.STANDARD,
): DamageResult {
  if (damageType === DAMAGE_TYPE.ARMOUR_PIERCING) {
    return {
      finalHp: Math.max(0, hp - amount),
      finalArmor: 0,
      absorbed: 0,
    };
  }

  const absorbed = Math.min(armor, amount);
  const finalArmor = armor - absorbed;
  const hpDamage = amount - absorbed;
  const finalHp = Math.max(0, hp - hpDamage);

  return {
    finalHp,
    finalArmor,
    absorbed,
  };
}
