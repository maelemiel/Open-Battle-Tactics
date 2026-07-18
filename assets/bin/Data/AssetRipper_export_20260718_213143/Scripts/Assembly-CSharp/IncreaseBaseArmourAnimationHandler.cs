using System;
using System.Collections;
using UnityEngine;

public class IncreaseBaseArmourAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator PreviewActivationAnimation(UnitState target)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_increasearmouractivated".Localize("Shields powering up..."));
			AudioTrigger.PowerUp.Play();
			yield return new WaitForSeconds(1.25f);
			battleController.CubeBar.GoToMainState();
		}
	}

	public override IEnumerator PreviewDeactivationAnimation()
	{
		return base.PreviewDeactivationAnimation();
	}

	public override IEnumerator TeamBeginAttackAnimation(TeamState team)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_increasearmourused".Localize("Shields Activated!"));
		}
		foreach (UnitView item in battleController.GetUnitsByTeam(abilityState.team))
		{
			item.LocalArmor += (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
		}
		yield return new WaitForSeconds(1.25f);
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState(string.Empty);
		}
	}

	public override IEnumerator DeactivationAnimation()
	{
		foreach (UnitView unit in battleController.GetUnitsByTeam(abilityState.team))
		{
			int armor = unit.LocalArmor - (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			if (armor < 0)
			{
				armor = 0;
			}
			unit.SetLocalArmor(armor, false);
		}
		yield return new WaitForSeconds(1f);
	}
}
