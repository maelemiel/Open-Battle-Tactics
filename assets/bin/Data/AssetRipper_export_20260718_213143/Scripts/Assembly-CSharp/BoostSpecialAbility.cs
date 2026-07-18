public class BoostSpecialAbility : ServerAbilityHandler
{
	public override void MatchBegin()
	{
		ServerUnitState[] units = abilityState.target.team.units;
		foreach (ServerUnitState serverUnitState in units)
		{
			foreach (ServerAbilityState ability in serverUnitState.abilities)
			{
				if (ability.metadata.ID == abilityState.SecondaryBoostValue.ToString())
				{
					ability.addedBoostA += abilityState.BoostValue;
					ability.addedBoostB += abilityState.BoostValue;
				}
			}
		}
	}

	public override bool UnitDiedEvent(ServerUnitState deadUnit, ServerTeamState damageSource)
	{
		if (deadUnit == abilityState.target)
		{
			ServerUnitState[] units = abilityState.target.team.units;
			foreach (ServerUnitState serverUnitState in units)
			{
				foreach (ServerAbilityState ability in serverUnitState.abilities)
				{
					if (ability.metadata.ID == abilityState.SecondaryBoostValue.ToString())
					{
						ability.addedBoostA -= abilityState.BoostValue;
						ability.addedBoostB -= abilityState.BoostValue;
					}
				}
			}
		}
		return false;
	}
}
