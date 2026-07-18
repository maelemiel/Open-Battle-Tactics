using System;
using System.Collections;
using UnityEngine;

public class MissileWeaponAnimationHandler : BasicWeaponAnimationHandler
{
	protected float randomMargin = 10f;

	protected float randomFireRotation;

	private EffectType burstAnimationEffect;

	public MissileWeaponAnimationHandler(EffectType fireEffect, EffectType burstAnimation, EffectType hitEffect, UnitWeaponType weaponType)
		: base(fireEffect, hitEffect, weaponType)
	{
		burstAnimationEffect = burstAnimation;
		kickbackAmount = 8f;
	}

	public override IEnumerator FiringAnimation(UnitView unit)
	{
		PlayAnimationAttackSound(unit);
		unit.ShakeMyField(screenShake, 0.1f);
		unit.PlayKickbackAnimation(kickbackAmount);
		EffectInstance fireEffect = null;
		EffectType effectType = firingAnimationEffect;
		if (unit.AlternativeWeapon != 0)
		{
			effectType = (EffectType)(int)Enum.Parse(typeof(EffectType), "BATTLE_EFFECT_FIRING_" + unit.AlternativeWeapon);
		}
		yield return battleController.StartCoroutine(GlobalEffectsManager.CreateCoroutine(effectType, unit.TankSpritesTransform.position, unit.gameObject, delegate(EffectInstance effectInstance)
		{
			fireEffect = effectInstance;
		}));
		fireEffect.AutoDestroy();
		fireEffect.transform.Translate(UnityEngine.Random.Range(0f - randomMargin, randomMargin), UnityEngine.Random.Range(0f - randomMargin, randomMargin), -20f);
		fireEffect.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(0f - randomFireRotation, randomFireRotation));
		EffectInstance burstEffect = null;
		if (burstAnimationEffect != EffectType.NONE)
		{
			yield return battleController.StartCoroutine(GlobalEffectsManager.CreateCoroutine(burstAnimationEffect, unit.TankSpritesTransform.position, unit.gameObject, delegate(EffectInstance effectInstance)
			{
				burstEffect = effectInstance;
			}));
			burstEffect.AutoDestroy();
		}
		fireEffect.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(0f - data.angle, data.angle));
		if (unit.isEnemy)
		{
			fireEffect.transform.MultiplyScale(-1f, 1f, 1f);
		}
		Vector3 anchorOffset = data.anchorPt;
		if (unit.isEnemy)
		{
			anchorOffset.x *= -1f;
		}
		fireEffect.transform.Translate(anchorOffset);
		if ((bool)burstEffect)
		{
			burstEffect.transform.Translate(anchorOffset);
		}
		yield return new WaitForSeconds(0.1f);
	}
}
