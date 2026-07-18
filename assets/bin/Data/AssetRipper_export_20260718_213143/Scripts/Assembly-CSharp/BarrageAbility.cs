public class BarrageAbility : ServerAbilityHandler
{
	public override bool TeamBeginAttackEvent(ServerTeamState team)
	{
		if (abilityState.isActive && team == abilityState.team)
		{
			ServerUnitState[] aliveUnits = team.otherTeam.aliveUnits;
			foreach (ServerUnitState serverUnitState in aliveUnits)
			{
				ApplyDamageInRange(serverUnitState, abilityState.BoostValue, abilityState.SecondaryBoostValue, abilityState.team, DamageType.AOE);
			}
			return true;
		}
		return false;
	}
}
