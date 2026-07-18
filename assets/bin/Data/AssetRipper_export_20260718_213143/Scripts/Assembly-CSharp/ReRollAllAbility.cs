public class ReRollAllAbility : ServerAbilityHandler
{
	public override void Activate(ServerUnitState target)
	{
		BattleLogic.RerollTeam(abilityState.team, true);
	}
}
