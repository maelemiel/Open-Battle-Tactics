using System.Collections;

public class ReRollAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator PreActivationAnimation(UnitState target)
	{
		bool isPlayerTeam = abilityState.team == battleController.playerTeam;
		ReRollAbility abilityHandler = (ReRollAbility)abilityState.handler;
		yield return StartCoroutine(target.unitView.RollDice(abilityHandler.rollIndex, isPlayerTeam));
	}
}
