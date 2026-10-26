import type { DamageType, DieFace } from '../types';
import { DAMAGE_TYPE, DIE_FACE_TYPE } from '../types';

/**
 * Which faces perform a standard attack (`BattleLogic.cs:CanUnitAttack` 231-250):
 * DirectDamage, Initiative, ArmourPiercing and AcidStrike attack; Special and
 * None do not (Special fires the unit's ability instead).
 */
export function canUnitAttack(face: DieFace): boolean {
  switch (face.type) {
    case DIE_FACE_TYPE.DAMAGE:
    case DIE_FACE_TYPE.INITIATIVE:
    case DIE_FACE_TYPE.ARMOUR_PIERCING:
    case DIE_FACE_TYPE.ACID_STRIKE:
      return true;
    case DIE_FACE_TYPE.SPECIAL:
    case DIE_FACE_TYPE.NONE:
      return false;
  }
}

/**
 * Damage a rolled face inflicts when its owner attacks this round
 * (`BattleLogic.cs:GetRollValueForUnit` base value, hooks add on top —
 * none implemented in MVP).
 */
export function faceDamage(face: DieFace): number {
  return canUnitAttack(face) ? face.value : 0;
}

/**
 * DamageType per face (`BattleLogic.cs:GetDamageTypeForUnit` 939-950):
 * ArmourPiercing face -> ArmourPiercing, AcidStrike face -> Acid,
 * everything else -> Standard.
 */
export function damageTypeForFace(face: DieFace): DamageType {
  if (face.type === DIE_FACE_TYPE.ARMOUR_PIERCING) return DAMAGE_TYPE.ARMOUR_PIERCING;
  if (face.type === DIE_FACE_TYPE.ACID_STRIKE) return DAMAGE_TYPE.ACID;
  return DAMAGE_TYPE.STANDARD;
}

export interface DamageTargetState {
  readonly hp: number;
  readonly armor: number;
  /** Target-side amplification debuff (`targeting_advance` ability; 0 in MVP). */
  readonly extraDamage: number;
  readonly acidPerRound: number;
}

export interface DamageResult extends DamageTargetState {
  /** Damage actually removed from HP (post-absorption). */
  readonly hpDamage: number;
  /** Damage absorbed by armor. */
  readonly absorbed: number;
  /** Pre-absorption total (`damage + extraDamage`) — the C# return value. */
  readonly total: number;
}

/**
 * Pure port of `BattleLogic.cs:ApplyDamage` (706-744). Exact order:
 * 1. dead target -> no-op (caller checks; kept out of the pure function).
 * 2. damage += target.extraDamage (hook point; see DamageTargetState).
 * 3. evadeNextAOE would zero AOE damage here (no AOE source in MVP).
 * 4. ArmourPiercing: armor is ZEROED (this target only) and does NOT absorb;
 *    otherwise armor absorbs min(armor, damage) and is decremented.
 * 5. Acid: acidPerRound++ (permanent stacking DoT).
 * 6. hp -= remaining damage; hp is NOT clamped at 0 — overkill goes negative
 *    (`hp <= 0` is dead; `hp <= -7` is the overkill coin stat).
 */
export function applyDamage(
  target: DamageTargetState,
  amount: number,
  damageType: DamageType = DAMAGE_TYPE.STANDARD,
): DamageResult {
  const total = amount + target.extraDamage;
  // Acid increment is independent of the AP/absorb branch (`BattleLogic.cs:731`),
  // but a single hit is never both AP and Acid (one DamageType per face).
  const acidPerRound =
    damageType === DAMAGE_TYPE.ACID ? target.acidPerRound + 1 : target.acidPerRound;

  if (damageType === DAMAGE_TYPE.ARMOUR_PIERCING) {
    return {
      hp: target.hp - total,
      armor: 0,
      extraDamage: target.extraDamage,
      acidPerRound,
      hpDamage: total,
      absorbed: 0,
      total,
    };
  }

  const absorbed = Math.min(target.armor, total);
  const hpDamage = total - absorbed;

  return {
    hp: target.hp - hpDamage,
    armor: target.armor - absorbed,
    extraDamage: target.extraDamage,
    acidPerRound,
    hpDamage,
    absorbed,
    total,
  };
}
