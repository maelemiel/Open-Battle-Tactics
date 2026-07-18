public class CakeWalkAbility : ServerAbilityHandler
{
	public override bool TeamBeginAttackEvent(ServerTeamState team)
	{
		if (abilityState.isActive && team == abilityState.team)
		{
			team.battle.cashPrizePool += abilityState.BoostValue;
			return true;
		}
		return false;
	}
}
