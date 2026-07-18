using System.Collections;
using UnityEngine;

public class IntelAnimationHandler : AbilityAnimationHandler
{
	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.INTEL_DEPLOY, 2);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.INTEL_SCAN, 1);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.INTEL_PARACHUTE, 1);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		TeamState targetTeam = abilityState.team.otherTeam;
		TeamState team = abilityState.team;
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_intelstep1".Localize("Scanning enemies!"));
			GameObject parentBattlefieldTargetTeam = targetTeam.battleField.gameObject;
			GameObject parentBattlefieldTeam = team.battleField.gameObject;
			Vector3 position = default(Vector3);
			position = new Vector3(0.7f, 0.8f, 0f);
			position = team.battleField.unityCamera.ViewportToWorldPoint(position);
			yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.INTEL_DEPLOY, position, parentBattlefieldTeam, delegate(EffectInstance e)
			{
				e.AutoDestroy();
			}));
			yield return new WaitForSeconds(1f);
			AudioTrigger.IntelScan.Play();
			position = new Vector3(0.6f, 0.8f, 0f);
			position = targetTeam.battleField.unityCamera.ViewportToWorldPoint(position);
			yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.INTEL_DEPLOY, position, parentBattlefieldTargetTeam, delegate(EffectInstance e)
			{
				e.AutoDestroy();
			}));
			yield return new WaitForSeconds(1.5f);
			position = new Vector3(-0.15f, 0.7f, 0f);
			position = targetTeam.battleField.unityCamera.ViewportToWorldPoint(position);
			yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.INTEL_SCAN, position, parentBattlefieldTargetTeam, delegate(EffectInstance e)
			{
				e.AutoDestroy();
			}));
			yield return new WaitForSeconds(1f);
			battleController.CubeBar.GoToMainState();
		}
	}

	public override IEnumerator PreInitiativeResultsAbility(int intelBoostTeam, int intelBoostOtherTeam)
	{
		bool isOpponent = abilityState.team != battleController.playerTeam;
		TeamState team = abilityState.team;
		GameObject parentBattlefieldTeam = team.battleField.gameObject;
		battleController.CubeBar.UpdateTextState("ui_battle_intelstep2".Localize("Here is the help!"));
		Vector3 position = new Vector3(0.6f, 0.7f, 0f);
		position = team.battleField.unityCamera.ViewportToWorldPoint(position);
		yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.INTEL_PARACHUTE, position, parentBattlefieldTeam, delegate(EffectInstance result)
		{
			result.AutoDestroy();
			IntelParachuteController component = result.GetComponent<IntelParachuteController>();
			int boostValue;
			component.SetBoostValue(boostValue);
			if (team == battleController.battleState.initiativeWinner)
			{
				component.ExpandBoostDice();
			}
			if (isOpponent)
			{
				component.transform.SetLocalXScale(0f - component.transform.localScale.x);
				Vector3 scale = component.BoosValueTextMesh.scale;
				scale.x = 0f - Mathf.Abs(scale.x);
				component.BoosValueTextMesh.scale = scale;
			}
		}));
		yield return new WaitForSeconds(3f);
		battleController.CubeBar.UpdateTextState(string.Empty);
	}
}
