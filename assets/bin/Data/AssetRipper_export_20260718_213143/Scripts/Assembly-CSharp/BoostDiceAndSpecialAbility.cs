public class BoostDiceAndSpecialAbility : ServerAbilityHandler
{
	public override int GetRollValueForUnit(ServerUnitState unit, int defaultValue)
	{
		if (unit.team == abilityState.team && !abilityState.target.IsDead)
		{
			DieFaceType faceType = GetFaceType();
			if (faceType == unit.CurrentRollType)
			{
				defaultValue += abilityState.BoostValue;
			}
		}
		return defaultValue;
	}

	public override void MatchBegin()
	{
		string text = abilityState.SecondaryBoostValue.ToString().Remove(0, 1);
		ServerUnitState[] units = abilityState.target.team.units;
		foreach (ServerUnitState serverUnitState in units)
		{
			foreach (ServerAbilityState ability in serverUnitState.abilities)
			{
				if (ability.metadata.ID == text)
				{
					ability.addedBoostA += abilityState.BoostValue;
					ability.addedBoostB += abilityState.BoostValue;
				}
			}
		}
	}

	public override bool UnitDiedEvent(ServerUnitState deadUnit, ServerTeamState damageSource)
	{
		string text = abilityState.SecondaryBoostValue.ToString().Remove(0, 1);
		if (deadUnit == abilityState.target)
		{
			ServerUnitState[] units = abilityState.target.team.units;
			foreach (ServerUnitState serverUnitState in units)
			{
				foreach (ServerAbilityState ability in serverUnitState.abilities)
				{
					if (ability.metadata.ID == text)
					{
						ability.addedBoostA -= abilityState.BoostValue;
						ability.addedBoostB -= abilityState.BoostValue;
					}
				}
			}
		}
		return false;
	}

	private DieFaceType GetFaceType()
	{
		string text = abilityState.SecondaryBoostValue.ToString().Remove(1) + "0000";
		if (!UnitLevelProgressionDataModel.faceTypeMap.ContainsKey(text))
		{
			Log.Error("BoostDiceAbility: Could not find Dice face type: " + text);
			return DieFaceType.None;
		}
		return UnitLevelProgressionDataModel.faceTypeMap[text];
	}
}
