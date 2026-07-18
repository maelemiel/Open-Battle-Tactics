public class DrawFireAbility : ServerAbilityHandler
{
	public override bool JammedEvent(ServerAbilityState jammerAbility, JammerPhase jammerPhase)
	{
		if (abilityState.isActive && jammerAbility.metadata.Type == "jammer2" && jammerPhase == JammerPhase.JAM_DEFENSIVE_ABILITES)
		{
			abilityState.isActive = false;
			return true;
		}
		return false;
	}

	public override void Activate(ServerUnitState target)
	{
		target.armor += abilityState.metadata.BoostValue;
	}

	public override ServerUnitState GetTargetForTeam(ServerTeamState forTeam, ServerUnitState defaultTarget)
	{
		if (abilityState.team != forTeam && abilityState.isActive && !abilityState.target.IsDead)
		{
			return abilityState.target;
		}
		return defaultTarget;
	}
}
