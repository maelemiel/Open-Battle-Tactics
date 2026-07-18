using System.Collections;

public class ReRollAllAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator ActivationAnimation(UnitState target)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			int[] rollValues = battleController.roundStartPhase.GetTeamRollValues(false);
			yield return StartCoroutine(battleController.roundStartPhase.SpinAllDiceStaggered(rollValues, false));
		}
	}
}
