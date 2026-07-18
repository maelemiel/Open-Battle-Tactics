using System;
using System.Collections;

public class FirstStrikeBuffAnimationHandler : AbilityAnimationHandler
{
	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.DIEFACE_BUFFED_EFFECT, 8);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.DIEFACE_BUFFING_EFFECT, 8);
	}

	public override IEnumerator OnRollFinished()
	{
		if (abilityState.isActive)
		{
			UnitView unitView = null;
			ServerUnitState[] aliveUnits = abilityState.team.aliveUnits;
			for (int i = 0; i < aliveUnits.Length; i++)
			{
				UnitState unit = (UnitState)aliveUnits[i];
				unitView = unit.unitView;
				int buffValue = (int)Math.Floor((float)abilityState.BoostValue * abilityState.boostMultiplier);
				unitView.BuffEffects.PushBuffValue(DieFaceType.Initiative, buffValue, BuffType.Initiative);
			}
		}
		yield break;
	}

	public override IEnumerator DeactivationAnimation()
	{
		UnitView unitView = null;
		ServerUnitState[] aliveUnits = abilityState.team.aliveUnits;
		for (int i = 0; i < aliveUnits.Length; i++)
		{
			UnitState unit = (UnitState)aliveUnits[i];
			unitView = unit.unitView;
			int buffValue = (int)Math.Floor((float)abilityState.BoostValue * abilityState.boostMultiplier);
			unitView.BuffEffects.PopBuffValue(DieFaceType.Initiative, buffValue, BuffType.Initiative);
		}
		yield break;
	}
}
