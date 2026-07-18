using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguishUnitAnimationHandler : AbilityAnimationHandler
{
	private List<ExtinguishEffect> extinguishEffectList = new List<ExtinguishEffect>();

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.EXTINGUISH_UNIT, 4);
	}

	public override IEnumerator PreInitiativeAnimation()
	{
		if (abilityState.target == null || abilityState.target.CurrentRollType != DieFaceType.Special)
		{
			yield break;
		}
		UnitView unit = abilityState.target.unitView;
		int teamUnitsCount = battleController.GetUnitsByTeam(abilityState.team).Count;
		for (int i = 0; i < teamUnitsCount; i++)
		{
			EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.EXTINGUISH_UNIT, unit.TankSpritesTransform.position, unit.transform).AutoDestroy();
			effectInstance.GetComponent<ExtinguishEffect>().PlayDeploy();
			yield return new WaitForSeconds(0.25f);
		}
		yield return new WaitForSeconds((float)teamUnitsCount * 0.25f);
		foreach (UnitView unitView in battleController.GetUnitsByTeam(abilityState.team))
		{
			StartCoroutine(ExtinguishLandSequence(unitView, 0.25f));
		}
		yield return new WaitForSeconds(1f);
	}

	public override IEnumerator EndOfRoundWithAbilities(TeamState team)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_extinguishused".Localize("Extinguish Activated!"));
		}
		foreach (ExtinguishEffect effect in extinguishEffectList)
		{
			StartCoroutine(ExtinguishExplosionSequence(effect, 0.25f));
		}
		yield return new WaitForSeconds(1.25f);
		foreach (UnitView unit in battleController.GetUnitsByTeam(abilityState.team))
		{
			if (unit.LocalDamagePerRound <= abilityState.BoostValue)
			{
				unit.LocalDamagePerRound = 0;
			}
			else
			{
				unit.LocalDamagePerRound -= abilityState.BoostValue;
			}
		}
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState(string.Empty);
		}
		yield return new WaitForSeconds(0.6f);
	}

	public override IEnumerator DeactivationAnimation()
	{
		foreach (ExtinguishEffect effect in extinguishEffectList)
		{
			effect.GetComponent<EffectInstance>().Destroy();
		}
		extinguishEffectList.Clear();
		yield break;
	}

	private IEnumerator ExtinguishLandSequence(UnitView unit, float maxDelay)
	{
		yield return new WaitForSeconds(Random.Range(0f, maxDelay));
		EffectInstance extinguishEffect = GlobalEffectsManager.Create(EffectType.EXTINGUISH_UNIT, unit.TankSpritesTransform.position, unit.transform);
		extinguishEffect.gameObject.SetLayerRecursively(unit.gameObject.layer);
		ExtinguishEffect extinguishEffectComponent = extinguishEffect.GetComponent<ExtinguishEffect>();
		ExtinguishEffect.ExtinguishIndicatorType randomEffectType = ((!(Random.Range(0f, 1f) > 0.5f)) ? ExtinguishEffect.ExtinguishIndicatorType.RIGHT : ExtinguishEffect.ExtinguishIndicatorType.LEFT);
		if ((bool)extinguishEffectComponent)
		{
			extinguishEffectComponent.ActivateEffect(randomEffectType, unit.TankSpritesTransform);
		}
		extinguishEffectList.Add(extinguishEffectComponent);
	}

	private IEnumerator ExtinguishExplosionSequence(ExtinguishEffect extinguishEffect, float maxDelay)
	{
		yield return new WaitForSeconds(Random.Range(0f, maxDelay));
		extinguishEffect.PlayExplosionEffect(null);
	}
}
