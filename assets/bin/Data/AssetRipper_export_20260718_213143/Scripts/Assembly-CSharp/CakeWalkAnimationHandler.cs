using System.Collections;
using UnityEngine;

public class CakeWalkAnimationHandler : AbilityAnimationHandler
{
	private static int NUM_EXPLOSIONS = 6;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.PLANE_FLYOVER, 3);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.CAKE_WALK, 1);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.REWARD, NUM_EXPLOSIONS);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_cakewalkactivated".Localize("Cake Walk deployed!"));
			AudioTrigger.PlaneBy.Play();
			AnimationHandlerUtil.CreateDefaultPlaneFlyover(abilityState.team);
			yield return new WaitForSeconds(1f);
			battleController.CubeBar.GoToMainState();
		}
	}

	public override IEnumerator TeamBeginAttackAnimation(TeamState team)
	{
		TeamState targetTeam = team.otherTeam;
		BattleController bc = battleController;
		battleController.CubeBar.UpdateTextState("ui_battle_cakewalkused".Localize("CAKE WALK AWAY!"));
		AnimationHandlerUtil.CreateDefaultPlaneFlyover(targetTeam);
		AudioTrigger.BarrageActivated.Play();
		if (targetTeam.IsEnemy)
		{
			AudioTrigger.CrowdCheering.Play();
		}
		else
		{
			AudioTrigger.CrowdDisappointed.Play();
		}
		yield return new WaitForSeconds(1f);
		float dirScaler = ((!targetTeam.IsEnemy) ? (-1f) : 1f);
		Vector3 randPos = targetTeam.battleField.unityCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.9f, 0f));
		GameObject parentBattlefield = targetTeam.battleField.gameObject;
		EffectInstance bombDropAnim = null;
		yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.CAKE_WALK, randPos, parentBattlefield, delegate(EffectInstance result)
		{
			bombDropAnim = result;
		}));
		bombDropAnim.AutoDestroy();
		bombDropAnim.transform.localScale = new Vector3(dirScaler, 1f, 1f);
		float bombAnimDelay = bombDropAnim.SpineAnimation.state.Animation.Duration;
		for (int i = 0; i < NUM_EXPLOSIONS; i++)
		{
			randPos = targetTeam.battleField.unityCamera.ViewportToWorldPoint(new Vector3(Random.value, Random.value * 0.7f + 0.1f, 0f));
			yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.REWARD, randPos, parentBattlefield, delegate(EffectInstance result)
			{
				result.AutoDestroy();
				result.Delay(0.5f);
			}));
			yield return new WaitForSeconds(bombAnimDelay / ((float)NUM_EXPLOSIONS * 0.9f));
			targetTeam.battleField.shaker.Shake(10f, 0.1f);
		}
		yield return new WaitForSeconds(1f);
		bc.CubeBar.UpdateTextState(string.Empty);
	}
}
