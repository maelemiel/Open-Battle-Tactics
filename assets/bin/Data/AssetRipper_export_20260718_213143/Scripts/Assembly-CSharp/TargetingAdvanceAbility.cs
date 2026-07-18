public class TargetingAdvanceAbility : ServerAbilityHandler
{
	public override bool JammedEvent(ServerAbilityState jammerAbility, JammerPhase jammerPhase)
	{
		if (abilityState.isActive && jammerAbility.metadata.Type == "jammer2" && jammerPhase == JammerPhase.JAM_OFFENSIVE_ABILITES)
		{
			Deactivate();
			abilityState.isActive = false;
			return true;
		}
		return false;
	}

	public override ServerUnitState GetTargetForTeam(ServerTeamState forTeam, ServerUnitState defaultTarget)
	{
		if (abilityState.team == forTeam && abilityState.isActive && !abilityState.target.IsDead)
		{
			return abilityState.target;
		}
		return defaultTarget;
	}

	public override void Activate(ServerUnitState target)
	{
		target.extraDamage += abilityState.metadata.BoostValue;
	}

	public override void Deactivate()
	{
		if (!abilityState.target.IsDead)
		{
			abilityState.target.extraDamage -= abilityState.metadata.BoostValue;
		}
	}
}
