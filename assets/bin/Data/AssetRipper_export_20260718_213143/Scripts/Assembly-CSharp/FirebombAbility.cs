public class FirebombAbility : ServerAbilityHandler
{
	public override bool TeamBeginAttackEvent(ServerTeamState team)
	{
		if (abilityState.isActive && team == abilityState.team)
		{
			ApplyAOEDamageAndDPRToTeam(team.otherTeam, abilityState.BoostValue, abilityState.SecondaryBoostValue, abilityState.team);
			return true;
		}
		return false;
	}
}
