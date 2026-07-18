using System;

public class FirstStrikeBuffAbility : ServerAbilityHandler
{
	public override int GetRollValueForUnit(ServerUnitState unit, int defaultValue)
	{
		if (abilityState.isActive && unit.CurrentRollType == DieFaceType.Initiative && unit.team == abilityState.team)
		{
			defaultValue += (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
		}
		return defaultValue;
	}
}
