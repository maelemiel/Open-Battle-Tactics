using System.Collections;
using UnityEngine;

public class EventPointBoostAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator OntoFieldAnimation()
	{
		abilityState.target.unitView.ShowPassiveEffect("Reward Passive", abilityState.Name);
		yield return new WaitForSeconds(0.1f);
	}

	public override IEnumerator UnitDiedAnimation(UnitState unit)
	{
		if (battleController.MatchHandler.IsEventPointsMatch)
		{
			bool shouldShowPoints = abilityState.target.team == battleController.playerTeam;
			unit.unitView.DeathAnimationList.Add(ShowBoost(unit, unit.extraDestroyEventPoints, shouldShowPoints));
		}
		yield break;
	}

	private IEnumerator ShowBoost(UnitState deadUnit, int bonus, bool showPoints)
	{
		abilityState.target.unitView.ShowPassiveEffect("Reward Passive", abilityState.Name);
		if (showPoints)
		{
			if (abilityState.team.otherTeam.type == TeamType.RaidBoss)
			{
				abilityState.target.unitView.ShowCashStackEffect(bonus, UserInventory.ItemType.RaidBossEventPoint);
			}
			else
			{
				abilityState.target.unitView.ShowCashStackEffect(bonus, UserInventory.ItemType.EventPoint);
			}
		}
		yield return new WaitForSeconds(0.5f);
	}
}
