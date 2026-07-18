using System.Collections;

public class BoostDiceAndSpecialAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator OntoFieldAnimation()
	{
		DieFaceType typeRequired = GetFaceType();
		string abilityId = abilityState.SecondaryBoostValue.ToString().Remove(0, 1);
		if (typeRequired == DieFaceType.None)
		{
			yield break;
		}
		abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
		for (int i = 0; i < abilityState.target.team.units.Length; i++)
		{
			abilityState.target.team.units[i].unitView.BuffEffects.PushBuffValue(typeRequired, abilityState.BoostValue, BuffType.None);
			foreach (AbilityState ability in abilityState.target.team.units[i].abilities)
			{
				if (ability.metadata.ID == abilityId)
				{
					abilityState.target.team.units[i].unitView.BuffEffects.PushBuffValue(DieFaceType.Special, abilityState.BoostValue, BuffType.None);
					break;
				}
			}
		}
	}

	public override IEnumerator DestroyAnimation()
	{
		PopBuffEffects();
		yield break;
	}

	public override IEnumerator PassiveFiringAnimation(UnitState unit)
	{
		if (unit.team != abilityState.target.team)
		{
			yield break;
		}
		if (abilityState.SecondaryBoostValue == 0)
		{
			abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
			yield break;
		}
		string faceType = abilityState.SecondaryBoostValue.ToString().Remove(1) + "0000";
		if (!UnitLevelProgressionDataModel.faceTypeMap.ContainsKey(faceType))
		{
			yield break;
		}
		DieFaceType typeRequired = UnitLevelProgressionDataModel.faceTypeMap[faceType];
		if (unit.CurrentRollType == typeRequired)
		{
			abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
		}
		else
		{
			if (unit.CurrentRollType != DieFaceType.Special)
			{
				yield break;
			}
			string abilityId = abilityState.SecondaryBoostValue.ToString().Remove(0, 1);
			foreach (AbilityState ability in unit.abilities)
			{
				if (ability.metadata.ID == abilityId)
				{
					abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
					break;
				}
			}
		}
	}

	private DieFaceType GetFaceType()
	{
		string key = abilityState.SecondaryBoostValue.ToString().Remove(1) + "0000";
		if (!UnitLevelProgressionDataModel.faceTypeMap.ContainsKey(key))
		{
			return DieFaceType.None;
		}
		return UnitLevelProgressionDataModel.faceTypeMap[key];
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
		DieFaceType faceType = GetFaceType();
		string text = abilityState.SecondaryBoostValue.ToString().Remove(0, 1);
		if (faceType == DieFaceType.None)
		{
			return;
		}
		for (int i = 0; i < abilityState.target.team.units.Length; i++)
		{
			abilityState.target.team.units[i].unitView.BuffEffects.PopBuffValue(faceType, abilityState.BoostValue, BuffType.None);
			foreach (AbilityState ability in abilityState.target.team.units[i].abilities)
			{
				if (ability.metadata.ID == text)
				{
					abilityState.target.team.units[i].unitView.BuffEffects.PopBuffValue(DieFaceType.Special, abilityState.BoostValue, BuffType.None);
					break;
				}
			}
			abilityState.target.team.units[i].unitView.BuffEffects.ResetBuffEffects();
		}
	}
}
