using System.Collections;
using UnityEngine;

public class JammerAnimationHandler : AbilityAnimationHandler
{
	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.JAMMER, 2);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_jammeractivated".Localize("Sensors Jammed!"));
			AudioTrigger.JammerStarted.Play();
			yield return new WaitForSeconds(1f);
			battleController.CubeBar.GoToMainState();
		}
	}

	public override IEnumerator TeamBeginAttackAnimation(TeamState team)
	{
		battleController.CubeBar.GoToTextState("ui_battle_jammeractivated".Localize("Target Jammed!"));
		Vector3 effectPosition = team.otherTeam.battleField.background.transform.position;
		EffectInstance jammer = GlobalEffectsManager.Create(EffectType.JAMMER, effectPosition, team.otherTeam.battleField.gameObject);
		jammer.SpineAnimation.Skeleton.SortOrder = 10;
		AudioTrigger.JammerStarted.Play();
		yield return new WaitForSeconds(2f);
		jammer.Destroy();
		battleController.CubeBar.GoToTextState(string.Empty);
	}
}
