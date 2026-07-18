public class TargetingAbility : ServerAbilityHandler
{
	public override bool JammedEvent(ServerAbilityState jammerAbility, JammerPhase jammerPhase)
	{
		if (abilityState.isActive && jammerPhase == JammerPhase.JAM_OFFENSIVE_ABILITES)
		{
			abilityState.isActive = false;
			return true;
		}
		return false;
	}

	public override ServerUnitState GetTargetForTeam(ServerTeamState forTeam, ServerUnitState defaultTarget)
	{
		if (abilityState.team == forTeam && abilityState.isActive && !ApplyCountermeasure(defaultTarget) && !abilityState.target.IsDead)
		{
			return abilityState.target;
		}
		return defaultTarget;
	}

	private bool ApplyCountermeasure(ServerUnitState defaultTarget)
	{
		bool result = false;
		ServerAbilityState[] abilities = defaultTarget.team.abilities;
		foreach (ServerAbilityState serverAbilityState in abilities)
		{
			if (serverAbilityState.metadata.Type == "jammer" && serverAbilityState.isActive)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
