using System.Collections;

public class BoostDiceAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator OntoFieldAnimation()
	{
		DieFaceType typeRequired = GetFaceType();
		if (typeRequired != DieFaceType.None)
		{
			abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
			for (int i = 0; i < abilityState.target.team.units.Length; i++)
			{
				abilityState.target.team.units[i].unitView.BuffEffects.PushBuffValue(typeRequired, abilityState.BoostValue, BuffType.None);
			}
		}
		yield break;
	}

	public override IEnumerator DestroyAnimation()
	{
		PopBuffEffects();
		yield break;
	}

	private DieFaceType GetFaceType()
	{
		string key = abilityState.SecondaryBoostValue.ToString();
		if (!UnitLevelProgressionDataModel.faceTypeMap.ContainsKey(key))
		{
			return DieFaceType.None;
		}
		return UnitLevelProgressionDataModel.faceTypeMap[key];
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
		string faceType = abilityState.SecondaryBoostValue.ToString();
		if (UnitLevelProgressionDataModel.faceTypeMap.ContainsKey(faceType))
		{
			DieFaceType typeRequired = UnitLevelProgressionDataModel.faceTypeMap[faceType];
			if (typeRequired == unit.CurrentRollType)
			{
				abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
			}
		}
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
		if (faceType != DieFaceType.None)
		{
			for (int i = 0; i < abilityState.target.team.units.Length; i++)
			{
				abilityState.target.team.units[i].unitView.BuffEffects.PopBuffValue(faceType, abilityState.BoostValue, BuffType.None);
				abilityState.target.team.units[i].unitView.BuffEffects.ResetBuffEffects();
			}
		}
	}
}
