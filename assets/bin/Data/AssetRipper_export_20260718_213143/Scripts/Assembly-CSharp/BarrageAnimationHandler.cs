using System.Collections;
using UnityEngine;

public class BarrageAnimationHandler : AbilityAnimationHandler
{
	private static int NUM_EXPLOSIONS = 12;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.PLANE_FLYOVER, 3);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOMB_DROP, NUM_EXPLOSIONS);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOMB_EXPLODE, NUM_EXPLOSIONS);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOMB_EXPLODE_2, NUM_EXPLOSIONS);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_barrageactivated".Localize("Bombers deployed!"));
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
		battleController.CubeBar.UpdateTextState("ui_battle_barrageused".Localize("BOMBS AWAY!"));
		AudioTrigger.BombAttack.Play();
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
		for (int i = 0; i < NUM_EXPLOSIONS; i++)
		{
			float dirScaler = ((!targetTeam.IsEnemy) ? (-1f) : 1f);
			Vector3 randPos = new Vector3(Random.value, Random.value * 0.7f + 0.1f, 0f);
			randPos = targetTeam.battleField.unityCamera.ViewportToWorldPoint(randPos);
			GameObject parentBattlefield = targetTeam.battleField.gameObject;
			EffectInstance bombDropAnim = null;
			yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.BOMB_DROP, randPos, parentBattlefield, delegate(EffectInstance result)
			{
				bombDropAnim = result;
			}));
			bombDropAnim.transform.localScale = new Vector3(dirScaler, 1f, 1f);
			float bombAnimDelay = bombDropAnim.SpineAnimation.state.Animation.Duration;
			EffectType effectType = ((Random.Range(0, 100) <= 50) ? EffectType.BOMB_EXPLODE_2 : EffectType.BOMB_EXPLODE);
			yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(effectType, randPos, parentBattlefield, delegate(EffectInstance result)
			{
				result.AutoDestroy();
				result.Delay(bombAnimDelay);
			}));
			yield return new WaitForSeconds(bombAnimDelay * 0.1f);
			targetTeam.battleField.shaker.Shake(10f, 0.1f);
		}
		foreach (UnitView unit in bc.GetUnitsByTeam(targetTeam))
		{
			unit.TakeDamage(GetNextDamage());
			yield return new WaitForSeconds(0.15f);
		}
		yield return new WaitForSeconds(1f);
		bc.CubeBar.UpdateTextState(string.Empty);
	}
}
