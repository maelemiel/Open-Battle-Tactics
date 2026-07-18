using System;
using System.Collections;
using UnityEngine;

public class BasicWeaponAnimationHandler : WeaponAnimationHandler
{
	protected EffectType firingAnimationEffect;

	protected EffectType hitAnimationEffect;

	protected UnitWeaponType weaponType;

	protected float screenShake = 1f;

	protected float kickbackAmount = 50f;

	public BasicWeaponAnimationHandler(EffectType fireEffect, EffectType hitEffect, UnitWeaponType weaponType)
	{
		firingAnimationEffect = fireEffect;
		hitAnimationEffect = hitEffect;
		this.weaponType = weaponType;
		kickbackAmount = 30f;
	}

	public override IEnumerator FiringAnimation(UnitView unit)
	{
		PlayAnimationAttackSound(unit);
		unit.ShakeMyField(screenShake, 0.1f);
		unit.PlayKickbackAnimation(kickbackAmount);
		EffectInstance fireEffect = null;
		if (unit.AlternativeWeapon == 0)
		{
			fireEffect = GlobalEffectsManager.Create(firingAnimationEffect, unit.TankSpritesTransform.position, unit.TankSpritesTransform).AutoDestroy();
		}
		else
		{
			EffectType effectType = (EffectType)(int)Enum.Parse(typeof(EffectType), "BATTLE_EFFECT_FIRING_" + unit.AlternativeWeapon);
			yield return battleController.StartCoroutine(GlobalEffectsManager.CreateCoroutine(effectType, unit.TankSpritesTransform.position, unit.gameObject, delegate(EffectInstance effectInstance)
			{
				fireEffect = effectInstance;
			}));
			fireEffect.AutoDestroy();
		}
		fireEffect.transform.Translate(0f, 0f, -20f);
		fireEffect.transform.Rotate(Vector3.forward, data.angle);
		fireEffect.transform.MultiplyScale(-1f, 1f, 1f);
		Vector3 anchorOffset = data.anchorPt;
		if (unit.isEnemy)
		{
			anchorOffset.x *= -1f;
		}
		fireEffect.transform.Translate(anchorOffset);
		fireEffect.SpineAnimation.AnimationName = fireEffect.SpineAnimation.GetAnimationNames()[0];
		yield return new WaitForSeconds(0.1f);
	}

	public override IEnumerator HitAnimation(UnitView source, UnitView target)
	{
		PlayAnimationHitSound(source, target);
		target.ShakeMyField(1f, 0.1f);
		EffectInstance hitEffect = null;
		if (source.CurrentRollAction == DieFaceType.ArmourPiercing)
		{
			hitEffect = GlobalEffectsManager.Create(EffectType.ARMOUR_PIERCING, target.transform.position, target.TankSpritesTransform).AutoDestroy();
			if (target.LocalArmor > 0)
			{
				hitEffect.GetComponent<tk2dSpineAnimation>().AnimationName = "Armor Piercing Special Hit Break Shield";
			}
			else
			{
				hitEffect.GetComponent<tk2dSpineAnimation>().AnimationName = "Armor Piercing Special Hit";
			}
		}
		else if (source.AlternativeWeapon == 0)
		{
			hitEffect = GlobalEffectsManager.Create(hitAnimationEffect, target.transform.position, target.TankSpritesTransform).AutoDestroy();
			hitEffect.SpineAnimation.AnimationName = hitEffect.SpineAnimation.GetAnimationNames()[0];
		}
		else
		{
			EffectType effectType = (EffectType)(int)Enum.Parse(typeof(EffectType), "BATTLE_EFFECT_HIT_" + source.AlternativeWeapon);
			yield return battleController.StartCoroutine(GlobalEffectsManager.CreateCoroutine(effectType, target.transform.position, target.gameObject, delegate(EffectInstance effectInstance)
			{
				hitEffect = effectInstance;
			}));
			hitEffect.AutoDestroy();
		}
		hitEffect.transform.Translate(0f, 0f, -20f);
		hitEffect.transform.MultiplyScale(1f, 1f, -1f);
		yield return new WaitForSeconds(0.1f);
	}

	protected void PlayAnimationAttackSound(UnitView unit)
	{
		if (unit.AlternativeWeapon != 0)
		{
			AudioTrigger audioName = (AudioTrigger)(int)Enum.Parse(typeof(AudioTrigger), "AudioABEffectFiring_" + unit.AlternativeWeapon);
			audioName.Play();
			return;
		}
		switch (weaponType)
		{
		case UnitWeaponType.MachineGunLow:
		case UnitWeaponType.MachineGunRare:
		case UnitWeaponType.MachineGunSR:
			AudioTrigger.MachineGunAttack.Play();
			break;
		case UnitWeaponType.Missile:
		case UnitWeaponType.Rockets:
			AudioTrigger.MissileFiringAttack.Play();
			break;
		case UnitWeaponType.ArmourPiercing:
			AudioTrigger.Mini_Gun_Deploy.Play();
			break;
		default:
			AudioTrigger.BattleCannonAttack.Play();
			break;
		}
	}

	protected void PlayAnimationHitSound(UnitView source, UnitView target)
	{
		if (source.AlternativeWeapon != 0)
		{
			AudioTrigger audioName = (AudioTrigger)(int)Enum.Parse(typeof(AudioTrigger), "AudioABEffectFiring_" + source.AlternativeWeapon);
			audioName.Play();
			return;
		}
		switch (weaponType)
		{
		case UnitWeaponType.MachineGunLow:
		case UnitWeaponType.MachineGunRare:
		case UnitWeaponType.MachineGunSR:
			AudioTrigger.MachineGunHit.Play();
			break;
		case UnitWeaponType.Missile:
		case UnitWeaponType.Rockets:
			AudioTrigger.MissileFiringHit.Play();
			break;
		case UnitWeaponType.ArmourPiercing:
			if (target.LocalArmor > 0)
			{
				AudioTrigger.Armour_Piercing_Hit_Shield.Play();
			}
			else
			{
				AudioTrigger.Armour_Piercing_Hit.Play();
			}
			break;
		default:
			AudioTrigger.BattleCannonHit.Play();
			break;
		}
	}
}
