using System.Collections;
using UnityEngine;

public class PartsIncreaseAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator OntoFieldAnimation()
	{
		abilityState.target.unitView.ShowPassiveEffect("Reward Passive", abilityState.Name);
		yield return new WaitForSeconds(0.1f);
	}

	public override IEnumerator UnitDiedAnimation(UnitState unit)
	{
		if (abilityState.target.team == battleController.playerTeam)
		{
			unit.unitView.DeathAnimationList.Add(ShowPartsReward());
		}
		yield break;
	}

	private IEnumerator ShowPartsReward()
	{
		abilityState.target.unitView.ShowPassiveEffect("Reward Passive", abilityState.Name);
		yield return new WaitForSeconds(0.25f);
	}
}
