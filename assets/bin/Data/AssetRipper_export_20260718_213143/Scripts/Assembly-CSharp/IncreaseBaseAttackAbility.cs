using System;

public class IncreaseBaseAttackAbility : ServerAbilityHandler
{
	public override int GetRollValueForUnit(ServerUnitState unit, int defaultValue)
	{
		if (abilityState.isActive)
		{
			if (unit.CurrentRollType == DieFaceType.DirectDamage && unit.team == abilityState.team)
			{
				defaultValue += (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			}
			bool flag = CacheManager.GetConstantInt("battle_add_damage_boost_to_ap", 1) == 1;
			if (unit.CurrentRollType == DieFaceType.ArmourPiercing && unit.team == abilityState.team && flag)
			{
				defaultValue += (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			}
		}
		return defaultValue;
	}
}
