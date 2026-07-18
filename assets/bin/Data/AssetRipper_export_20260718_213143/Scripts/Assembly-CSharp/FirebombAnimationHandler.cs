using System.Collections;
using UnityEngine;

public class FirebombAnimationHandler : AbilityAnimationHandler
{
	private static int NUM_EXPLOSIONS = 12;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.PLANE_FLYOVER, 3);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOMB_DROP, NUM_EXPLOSIONS);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.NAPALM_HIT, NUM_EXPLOSIONS);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BURN, 8);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_firebombactivated".Localize("(Fire)Bombers deployed!"));
			AudioTrigger.PlaneBy.Play();
			AnimationHandlerUtil.CreateDefaultPlaneFlyover(abilityState.team);
			yield return new WaitForSeconds(1f);
			battleController.CubeBar.GoToMainState();
		}
	}

	public override IEnumerator TeamBeginAttackAnimation(TeamState team)
	{
		TeamState targetTeam = team.otherTeam;
		battleController.CubeBar.UpdateTextState("ui_battle_firebombused".Localize("(FIRE)BOMBS AWAY!"));
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
			if ((bool)bombDropAnim)
			{
				bombDropAnim.AutoDestroy();
				bombDropAnim.transform.localScale = new Vector3(dirScaler, 1f, 1f);
				float bombAnimDelay = bombDropAnim.SpineAnimation.state.Animation.Duration;
				yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.NAPALM_HIT, randPos, parentBattlefield, delegate(EffectInstance result)
				{
					result.AutoDestroy();
					result.Delay(bombAnimDelay);
				}));
				yield return new WaitForSeconds(bombAnimDelay * 0.1f);
				targetTeam.battleField.shaker.Shake(10f, 0.2f);
			}
		}
		foreach (UnitView unit in battleController.GetUnitsByTeam(targetTeam))
		{
			int damage = GetNextDamage();
			unit.TakeDamage(damage);
			if (damage > 0)
			{
				unit.LocalDamagePerRound += abilityState.SecondaryBoostValue;
			}
			yield return new WaitForSeconds(0.15f);
		}
		yield return new WaitForSeconds(1f);
		battleController.CubeBar.UpdateTextState(string.Empty);
	}
}
