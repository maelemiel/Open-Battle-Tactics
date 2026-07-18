using System;
using System.Collections;

public class IncreaseBaseAttackAnimationHandler : AbilityAnimationHandler
{
	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.DIEFACE_BUFFED_EFFECT, 1);
	}

	public override IEnumerator OnRollFinished()
	{
		if (!abilityState.isActive)
		{
			yield break;
		}
		UnitView unitView = null;
		ServerUnitState[] aliveUnits = abilityState.team.aliveUnits;
		for (int i = 0; i < aliveUnits.Length; i++)
		{
			UnitState unit = (UnitState)aliveUnits[i];
			unitView = unit.unitView;
			int buffValue = (int)Math.Floor((float)abilityState.BoostValue * abilityState.boostMultiplier);
			unitView.BuffEffects.PushBuffValue(DieFaceType.DirectDamage, buffValue, BuffType.DirectDamage);
			if (Constants.AddDamageBoostToArmourPiercing)
			{
				unitView.BuffEffects.PushBuffValue(DieFaceType.ArmourPiercing, buffValue, BuffType.DirectDamage);
			}
		}
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
			unitView.BuffEffects.PopBuffValue(DieFaceType.DirectDamage, buffValue, BuffType.DirectDamage);
			if (Constants.AddDamageBoostToArmourPiercing)
			{
				unitView.BuffEffects.PopBuffValue(DieFaceType.ArmourPiercing, buffValue, BuffType.DirectDamage);
			}
		}
		yield break;
	}
}
