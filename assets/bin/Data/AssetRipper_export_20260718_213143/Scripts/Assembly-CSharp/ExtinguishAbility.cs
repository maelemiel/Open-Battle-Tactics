public class ExtinguishAbility : ServerAbilityHandler
{
	public override bool PreInitiativeEvent()
	{
		if (abilityState.isActive && abilityState.target != null)
		{
			return !abilityState.target.IsDead;
		}
		return false;
	}

	public override bool TeamBeginAttackEvent(ServerTeamState team)
	{
		bool flag = false;
		if (abilityState.target != null && abilityState.target.IsDead)
		{
			flag = true;
		}
		if (team == abilityState.team && abilityState.isActive && !flag)
		{
			ServerUnitState serverUnitState = abilityState.target;
			if (serverUnitState.damagePerRound <= abilityState.BoostValue)
			{
				serverUnitState.damagePerRound = 0;
			}
			else
			{
				serverUnitState.damagePerRound -= abilityState.BoostValue;
			}
			return true;
		}
		return false;
	}
}
