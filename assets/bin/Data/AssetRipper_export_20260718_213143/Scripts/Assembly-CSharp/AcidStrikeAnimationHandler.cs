using System.Collections;
using UnityEngine;

public class AcidStrikeAnimationHandler : BasicWeaponAnimationHandler
{
	public AcidStrikeAnimationHandler(EffectType fireEffect, EffectType hitEffect, UnitWeaponType weaponType)
		: base(fireEffect, hitEffect, weaponType)
	{
	}

	public override IEnumerator FiringAnimation(UnitView unit)
	{
		PlayAnimationAttackSound(unit);
		unit.ShakeMyField(screenShake, 0.1f);
		unit.PlayKickbackAnimation(kickbackAmount);
		EffectInstance fireEffect = null;
		Vector3 anchorOffset = Vector3.zero;
		yield return battleController.StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.ACID_STRIKE, unit.TankSpritesTransform.position, unit.gameObject, delegate(EffectInstance effectInstance)
		{
			fireEffect = effectInstance;
		}));
		fireEffect.AutoDestroy();
		tk2dSpineAnimation t = fireEffect.GetComponent<tk2dSpineAnimation>();
		string[] names = t.GetAnimationNames();
		t.AnimationName = names[1];
		fireEffect.transform.Translate(0f, 0f, -20f);
		fireEffect.transform.Rotate(Vector3.forward, data.angle);
		fireEffect.transform.MultiplyScale(-1f, 1f, 1f);
		if (anchorOffset == Vector3.zero)
		{
			anchorOffset = data.anchorPt;
		}
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
		hitEffect = GlobalEffectsManager.Create(EffectType.ACID_STRIKE, target.transform.position, target.TankSpritesTransform).AutoDestroy();
		tk2dSpineAnimation t = hitEffect.GetComponent<tk2dSpineAnimation>();
		string[] names = t.GetAnimationNames();
		t.AnimationName = names[2];
		hitEffect.transform.Translate(0f, 0f, -20f);
		hitEffect.transform.MultiplyScale(1f, 1f, -1f);
		yield return new WaitForSeconds(0.1f);
	}
}
