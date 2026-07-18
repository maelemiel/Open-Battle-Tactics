public class ReRoll2Ability : ServerAbilityHandler
{
	public int rollIndex;

	public override void Activate(ServerUnitState target)
	{
		BattleLogic.RerollUnit(target, false, false, false);
		rollIndex = target.currentRoll;
		BattleLogic.EvaluateRollRewards(target.team, false);
	}
}
