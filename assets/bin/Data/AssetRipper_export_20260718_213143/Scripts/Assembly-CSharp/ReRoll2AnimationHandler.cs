using System.Collections;

public class ReRoll2AnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator PreActivationAnimation(UnitState target)
	{
		bool isPlayerTeam = abilityState.team == battleController.playerTeam;
		ReRoll2Ability abilityHandler = (ReRoll2Ability)abilityState.handler;
		yield return StartCoroutine(target.unitView.RollDice(abilityHandler.rollIndex, isPlayerTeam));
	}
}
