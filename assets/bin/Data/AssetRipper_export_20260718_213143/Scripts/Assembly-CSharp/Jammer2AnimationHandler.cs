using System.Collections;
using UnityEngine;

public class Jammer2AnimationHandler : AbilityAnimationHandler
{
	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.JAMMER_2, 2);
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
		Vector3 effectPosition = team.otherTeam.battleField.background.transform.position;
		EffectInstance jammer = GlobalEffectsManager.Create(EffectType.JAMMER_2, effectPosition, team.otherTeam.battleField.gameObject);
		jammer.SpineAnimation.Skeleton.SortOrder = 10;
		AudioTrigger.JammerStarted.Play();
		yield return new WaitForSeconds(1f);
		jammer.Destroy();
	}
}
