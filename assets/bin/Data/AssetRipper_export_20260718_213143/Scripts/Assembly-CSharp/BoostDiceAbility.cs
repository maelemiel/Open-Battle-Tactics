public class BoostDiceAbility : ServerAbilityHandler
{
	public override int GetRollValueForUnit(ServerUnitState unit, int defaultValue)
	{
		if (unit.team == abilityState.team && !abilityState.target.IsDead)
		{
			if (abilityState.SecondaryBoostValue != 0)
			{
				string text = abilityState.SecondaryBoostValue.ToString();
				if (!UnitLevelProgressionDataModel.faceTypeMap.ContainsKey(text))
				{
					Log.Error("BoostDiceAbility: Could not find Dice face type: " + text);
					return defaultValue;
				}
				DieFaceType dieFaceType = UnitLevelProgressionDataModel.faceTypeMap[text];
				if (dieFaceType == unit.CurrentRollType)
				{
					defaultValue += abilityState.BoostValue;
				}
			}
			else if (unit.CurrentRollType == DieFaceType.DirectDamage || unit.CurrentRollType == DieFaceType.Initiative || unit.CurrentRollType == DieFaceType.ArmourPiercing || unit.CurrentRollType == DieFaceType.AcidStrike)
			{
				defaultValue += abilityState.BoostValue;
			}
		}
		return defaultValue;
	}
}
