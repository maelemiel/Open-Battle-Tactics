public class SelfDestructAbility : ServerAbilityHandler
{
	public override bool TeamBeginAttackEvent(ServerTeamState team)
	{
		if (abilityState.isActive && team == abilityState.team && !abilityState.target.IsDead)
		{
			BattleLogic.ApplyDamage(abilityState.target, 999, abilityState.team, DamageType.Standard);
			ServerUnitState[] aliveUnits = abilityState.team.otherTeam.aliveUnits;
			foreach (ServerUnitState serverUnitState in aliveUnits)
			{
				ApplyDamage(serverUnitState, abilityState.BoostValue, abilityState.team, DamageType.AOE);
			}
			return true;
		}
		return false;
	}
}
