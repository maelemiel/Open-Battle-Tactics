using System;
using System.Collections;
using UnityEngine;

public class ExtinguishAnimationHandler : AbilityAnimationHandler
{
	private ExtinguishEffect extinguishEffectComponent;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.EXTINGUISH_UNIT, 1);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		if (target != null)
		{
			UnitView unit = target.unitView;
			StartCoroutine(ExtinguishLandSequence(unit));
			yield return new WaitForSeconds(1f);
		}
	}

	public override IEnumerator TeamBeginAttackAnimation(TeamState team)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_extinguishused".Localize("Extinguish Activated!"));
		}
		StartCoroutine(ExtinguishExplosionSequence(extinguishEffectComponent, 0.25f));
		yield return new WaitForSeconds(0.65f);
		UnitView unit = abilityState.target.unitView;
		int damage = (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
		if (unit.LocalDamagePerRound <= damage)
		{
			unit.LocalDamagePerRound = 0;
		}
		else
		{
			unit.LocalDamagePerRound -= damage;
		}
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState(string.Empty);
		}
		yield return new WaitForSeconds(0.6f);
	}

	public override IEnumerator DeactivationAnimation()
	{
		extinguishEffectComponent.GetComponent<EffectInstance>().Destroy();
		extinguishEffectComponent = null;
		yield break;
	}

	private IEnumerator ExtinguishLandSequence(UnitView unit)
	{
		EffectInstance extinguishEffect = GlobalEffectsManager.Create(EffectType.EXTINGUISH_UNIT, unit.TankSpritesTransform.position, unit.transform);
		extinguishEffect.gameObject.SetLayerRecursively(unit.gameObject.layer);
		extinguishEffectComponent = extinguishEffect.GetComponent<ExtinguishEffect>();
		if ((bool)extinguishEffectComponent)
		{
			extinguishEffectComponent.ActivateEffect(ExtinguishEffect.ExtinguishIndicatorType.TOP, unit.TankSpritesTransform);
		}
		yield break;
	}

	private IEnumerator ExtinguishExplosionSequence(ExtinguishEffect extinguishEffect, float maxDelay)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0f, maxDelay));
		extinguishEffect.PlayExplosionEffect(null);
	}
}
