public class IonStrikeAbility : ServerAbilityHandler
{
	public override bool TeamBeginAttackEvent(ServerTeamState team)
	{
		if (abilityState.isActive && team == abilityState.team)
		{
			ApplyDamageInRange(abilityState.target, abilityState.BoostValue, abilityState.SecondaryBoostValue, abilityState.team, DamageType.Standard);
			return true;
		}
		return false;
	}

	public override bool JammedEvent(ServerAbilityState jammerAbility, JammerPhase jammerPhase)
	{
		if (abilityState.isActive && jammerAbility.metadata.Type == "jammer2" && jammerPhase == JammerPhase.JAM_OFFENSIVE_ABILITES)
		{
			abilityState.target = BattleLogic.GetRandomUnitOnTeam(abilityState.team.otherTeam, abilityState.team, "IonStrike - JammedEvent");
			return true;
		}
		return false;
	}
}
