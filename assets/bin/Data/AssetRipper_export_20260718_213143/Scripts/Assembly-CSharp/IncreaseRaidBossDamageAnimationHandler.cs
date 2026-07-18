using System.Collections;
using UnityEngine;

public class IncreaseRaidBossDamageAnimationHandler : AbilityAnimationHandler
{
	private bool activated;

	public override IEnumerator OntoFieldAnimation()
	{
		if (abilityState.team.otherTeam.type == TeamType.RaidBoss)
		{
			abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
			UnitView unit = abilityState.target.unitView;
			GlobalEffectsManager.Create(EffectType.PUNISHER, unit.TankSpritesTransform.position, unit.transform).AutoDestroy();
			unit.BuffEffects.PushBuffValue(DieFaceType.DirectDamage, abilityState.BoostValue, BuffType.BossDamage);
			unit.BuffEffects.PushBuffValue(DieFaceType.Initiative, abilityState.BoostValue, BuffType.BossDamage);
			unit.BuffEffects.PushBuffValue(DieFaceType.ArmourPiercing, abilityState.BoostValue, BuffType.BossDamage);
			unit.BuffEffects.PushBuffValue(DieFaceType.Special, abilityState.BoostValue, BuffType.BossDamage);
		}
		yield return new WaitForSeconds(0.1f);
	}

	public override IEnumerator DestroyAnimation()
	{
		abilityState.target.unitView.BuffEffects.PopBuffValue(DieFaceType.DirectDamage, abilityState.BoostValue, BuffType.BossDamage);
		abilityState.target.unitView.BuffEffects.PopBuffValue(DieFaceType.Initiative, abilityState.BoostValue, BuffType.BossDamage);
		abilityState.target.unitView.BuffEffects.PopBuffValue(DieFaceType.ArmourPiercing, abilityState.BoostValue, BuffType.BossDamage);
		abilityState.target.unitView.BuffEffects.PopBuffValue(DieFaceType.Special, abilityState.BoostValue, BuffType.BossDamage);
		abilityState.target.unitView.BuffEffects.ResetBuffEffects();
		yield break;
	}

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.DIEFACE_BUFFED_EFFECT, 1);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.DIEFACE_BUFFING_EFFECT, 1);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.PUNISHER, 1);
	}

	public override IEnumerator OnRollFinished()
	{
		if (abilityState.team.otherTeam.type == TeamType.RaidBoss && !activated)
		{
			activated = true;
		}
		yield break;
	}

	public override IEnumerator PreFiringAnimation(ServerUnitState unit, ServerUnitState target)
	{
		if (abilityState.team.otherTeam.type == TeamType.RaidBoss)
		{
			abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
			UnitView unitview = abilityState.target.unitView;
			GlobalEffectsManager.Create(EffectType.PUNISHER, unitview.TankSpritesTransform.position, unitview.transform).AutoDestroy();
			yield return new WaitForSeconds(1.1f);
		}
	}
}
