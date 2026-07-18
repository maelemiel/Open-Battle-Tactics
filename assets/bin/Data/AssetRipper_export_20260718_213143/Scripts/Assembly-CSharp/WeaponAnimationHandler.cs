using System;
using System.Collections;
using System.Collections.Generic;

public class WeaponAnimationHandler
{
	protected const string FIRING_ANIM_PREFIX = "BATTLE_EFFECT_FIRING_";

	protected const string HIT_ANIM_PREFIX = "BATTLE_EFFECT_HIT_";

	public UnitWeaponSystem.UnitWeaponData data;

	public BattleController battleController;

	private static Dictionary<UnitWeaponType, Func<WeaponAnimationHandler>> registeredHandlers = new Dictionary<UnitWeaponType, Func<WeaponAnimationHandler>>
	{
		{
			UnitWeaponType.MachineGunLow,
			() => new MachineGunAnimationHandler(EffectType.MACHINEGUN_LOW_FIRE, EffectType.MACHINEGUN_LOW_HIT, UnitWeaponType.MachineGunLow)
		},
		{
			UnitWeaponType.MachineGunRare,
			() => new MachineGunAnimationHandler(EffectType.MACHINEGUN_RARE_FIRE, EffectType.MACHINEGUN_HIGH_HIT, UnitWeaponType.MachineGunRare)
		},
		{
			UnitWeaponType.MachineGunSR,
			() => new MachineGunAnimationHandler(EffectType.MACHINEGUN_SR_FIRE, EffectType.MACHINEGUN_HIGH_HIT, UnitWeaponType.MachineGunSR)
		},
		{
			UnitWeaponType.CannonLow,
			() => new BasicWeaponAnimationHandler(EffectType.CANNON_LOW_FIRE, EffectType.CANNON_LOW_HIT, UnitWeaponType.CannonLow)
		},
		{
			UnitWeaponType.CannonRare,
			() => new BasicWeaponAnimationHandler(EffectType.CANNON_RARE_FIRE, EffectType.CANNON_HIGH_HIT, UnitWeaponType.CannonRare)
		},
		{
			UnitWeaponType.CannonSR,
			() => new BasicWeaponAnimationHandler(EffectType.CANNON_SR_FIRE, EffectType.CANNON_HIGH_HIT, UnitWeaponType.CannonSR)
		},
		{
			UnitWeaponType.Missile,
			() => new MissileWeaponAnimationHandler(EffectType.MISSILE_FIRE, EffectType.MISSILE_BURST, EffectType.MISSILE_EXPLOSION, UnitWeaponType.Missile)
		},
		{
			UnitWeaponType.Rockets,
			() => new RocketWeaponAnimationHandler(EffectType.ROCKET_FIRE, EffectType.ROCKET_HIT, EffectType.ROCKET_EXPLOSION, UnitWeaponType.Rockets)
		},
		{
			UnitWeaponType.ArmourPiercing,
			() => new ArmourPiercingAnimationHanlder(EffectType.ARMOUR_PIERCING, EffectType.ARMOUR_PIERCING, UnitWeaponType.ArmourPiercing)
		},
		{
			UnitWeaponType.AcidStrike,
			() => new AcidStrikeAnimationHandler(EffectType.ACID_STRIKE, EffectType.ACID_STRIKE, UnitWeaponType.AcidStrike)
		}
	};

	public static WeaponAnimationHandler CreateAnimationHandler(UnitWeaponSystem.UnitWeaponData weaponData, BattleController battleController)
	{
		if (!registeredHandlers.ContainsKey(weaponData.type))
		{
			return null;
		}
		WeaponAnimationHandler weaponAnimationHandler = registeredHandlers[weaponData.type]();
		weaponAnimationHandler.data = weaponData;
		weaponAnimationHandler.battleController = battleController;
		return weaponAnimationHandler;
	}

	public virtual IEnumerator FiringAnimation(UnitView unit)
	{
		yield break;
	}

	public virtual IEnumerator HitAnimation(UnitView source, UnitView target)
	{
		yield break;
	}
}
