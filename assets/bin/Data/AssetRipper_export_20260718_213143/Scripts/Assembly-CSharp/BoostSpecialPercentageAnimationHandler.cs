using System;
using System.Collections;

public class BoostSpecialPercentageAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator PassiveFiringAnimation(UnitState unit)
	{
		if (unit.team != abilityState.target.team || unit.CurrentRollType != DieFaceType.Special)
		{
			yield break;
		}
		string abilityId = abilityState.SecondaryBoostValue.ToString();
		for (int i = 0; i < abilityState.target.team.units.Length; i++)
		{
			foreach (AbilityState ability in abilityState.target.team.units[i].abilities)
			{
				if (ability.metadata.ID == abilityId)
				{
					abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
					break;
				}
			}
		}
	}

	public override IEnumerator OntoFieldAnimation()
	{
		abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
		string abilityId = abilityState.SecondaryBoostValue.ToString();
		float percentIncrease = (float)abilityState.BoostValue / 100f;
		int boostValue = 0;
		for (int i = 0; i < abilityState.target.team.units.Length; i++)
		{
			foreach (AbilityState ability in abilityState.target.team.units[i].abilities)
			{
				if (ability.metadata.ID == abilityId)
				{
					boostValue = (int)Math.Ceiling((float)(ability.BoostValue - ability.addedBoostA) * percentIncrease);
					abilityState.target.team.units[i].unitView.BuffEffects.PushBuffValue(DieFaceType.Special, boostValue, BuffType.None);
					break;
				}
			}
		}
		yield break;
	}

	public override IEnumerator DestroyAnimation()
	{
		PopBuffEffects();
		yield break;
	}

	public override IEnumerator UnitDiedAnimation(UnitState unit)
	{
		if (unit == abilityState.target)
		{
			PopBuffEffects();
		}
		yield break;
	}

	private void PopBuffEffects()
	{
		string text = abilityState.SecondaryBoostValue.ToString().Remove(0, 1);
		float num = (float)abilityState.BoostValue / 100f;
		int num2 = 0;
		for (int i = 0; i < abilityState.target.team.units.Length; i++)
		{
			foreach (AbilityState ability in abilityState.target.team.units[i].abilities)
			{
				if (ability.metadata.ID == text)
				{
					num2 = (int)Math.Ceiling((float)(ability.BoostValue - ability.addedBoostA) * num);
					abilityState.target.team.units[i].unitView.BuffEffects.PopBuffValue(DieFaceType.Special, num2, BuffType.None);
					break;
				}
			}
		}
	}
}
