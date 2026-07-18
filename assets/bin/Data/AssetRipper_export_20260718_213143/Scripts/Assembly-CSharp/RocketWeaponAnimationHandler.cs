using System.Collections;
using UnityEngine;

public class RocketWeaponAnimationHandler : MissileWeaponAnimationHandler
{
	private EffectType hitTrailEffect;

	public RocketWeaponAnimationHandler(EffectType fireEffect, EffectType hitTrailEffect, EffectType hitEffect, UnitWeaponType weaponType)
		: base(fireEffect, EffectType.NONE, hitEffect, weaponType)
	{
		this.hitTrailEffect = hitTrailEffect;
		kickbackAmount = 8f;
		randomFireRotation = 3f;
		randomMargin = 20f;
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(fireEffect, 10);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(hitTrailEffect, 10);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(hitEffect, 10);
	}

	public int NumRocketsToFire(UnitView unit)
	{
		int num = 3;
		int num2 = 5;
		return num + Mathf.CeilToInt(unit.state.CurrentRollValue / num2);
	}

	public override IEnumerator FiringAnimation(UnitView unit)
	{
		int numRockets = NumRocketsToFire(unit);
		for (int i = 0; i < numRockets; i++)
		{
			yield return battleController.StartCoroutine(base.FiringAnimation(unit));
		}
	}

	public override IEnumerator HitAnimation(UnitView source, UnitView target)
	{
		int numRockets = NumRocketsToFire(source);
		for (int i = 0; i < numRockets; i++)
		{
			battleController.StartCoroutine(HitAnimationSingle(source, target));
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.1f);
	}

	protected IEnumerator HitAnimationSingle(UnitView source, UnitView target)
	{
		EffectInstance hitTrail = GlobalEffectsManager.Create(hitTrailEffect, target.transform.position, target.TankSpritesTransform).AutoDestroy();
		hitTrail.transform.Translate(Random.Range(0f - randomMargin, randomMargin), Random.Range(0f - randomMargin, randomMargin), -20f);
		hitTrail.transform.Rotate(Vector3.forward, Random.Range(-3f, 3f));
		yield return new WaitForSeconds(0.5f);
		PlayAnimationHitSound(source, target);
		target.ShakeMyField(1f, 0.1f);
		EffectInstance hitEffect = GlobalEffectsManager.Create(hitAnimationEffect, target.transform.position, target.TankSpritesTransform).AutoDestroy();
		hitEffect.transform.localPosition = hitTrail.transform.localPosition;
		hitEffect.transform.MultiplyScale(1f, 1f, -1f);
	}
}
