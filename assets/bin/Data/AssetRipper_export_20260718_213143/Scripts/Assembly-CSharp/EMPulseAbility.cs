public class EMPulseAbility : ServerAbilityHandler
{
	public override bool TeamBeginAttackEvent(ServerTeamState team)
	{
		if (abilityState.isActive && team == abilityState.team)
		{
			abilityState.target.roundsUntilRerollEnabled++;
			abilityState.target.preventReroll = true;
			ApplyDamage(abilityState.target, abilityState.BoostValue, abilityState.team, DamageType.Standard);
			return true;
		}
		return false;
	}

	public override bool JammedEvent(ServerAbilityState jammerAbility, JammerPhase jammerPhase)
	{
		if (abilityState.isActive && jammerAbility.metadata.Type == "jammer2" && jammerPhase == JammerPhase.JAM_OFFENSIVE_ABILITES)
		{
			abilityState.target = BattleLogic.GetRandomUnitOnTeam(abilityState.team.otherTeam, abilityState.team, "EMPulse - JammedEvent");
			return true;
		}
		return false;
	}
}
