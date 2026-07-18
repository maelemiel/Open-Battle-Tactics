using System;

public class BoostSpecialPercentageAbility : ServerAbilityHandler
{
	public override void MatchBegin()
	{
		float num = (float)abilityState.BoostValue / 100f;
		ServerUnitState[] units = abilityState.target.team.units;
		foreach (ServerUnitState serverUnitState in units)
		{
			foreach (ServerAbilityState ability in serverUnitState.abilities)
			{
				if (ability.metadata.ID == abilityState.SecondaryBoostValue.ToString())
				{
					ability.addedBoostA += (int)Math.Ceiling((float)ability.BaseBoostValue * num);
					ability.addedBoostB += (int)Math.Ceiling((float)ability.BaseSecondaryBoostValue * num);
				}
			}
		}
	}

	public override bool UnitDiedEvent(ServerUnitState deadUnit, ServerTeamState damageSource)
	{
		if (deadUnit == abilityState.target)
		{
			float num = (float)abilityState.BoostValue / 100f;
			ServerUnitState[] units = abilityState.target.team.units;
			foreach (ServerUnitState serverUnitState in units)
			{
				foreach (ServerAbilityState ability in serverUnitState.abilities)
				{
					if (ability.metadata.ID == abilityState.SecondaryBoostValue.ToString())
					{
						ability.addedBoostA -= (int)Math.Ceiling((float)ability.BaseBoostValue * num);
						ability.addedBoostB -= (int)Math.Ceiling((float)ability.BaseSecondaryBoostValue * num);
					}
				}
			}
		}
		return false;
	}
}
