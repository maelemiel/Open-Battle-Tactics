public class ReRollAbility : ServerAbilityHandler
{
	public int rollIndex;

	public override void Activate(ServerUnitState target)
	{
		BattleLogic.RerollUnit(target, false, false, true);
		rollIndex = target.currentRoll;
		BattleLogic.EvaluateRollRewards(target.team, false);
	}
}
