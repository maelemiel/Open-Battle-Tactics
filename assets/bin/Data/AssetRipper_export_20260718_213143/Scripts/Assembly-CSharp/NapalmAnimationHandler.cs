using System.Collections;
using UnityEngine;

public class NapalmAnimationHandler : AbilityAnimationHandler
{
	private static int NUM_EXPLOSIONS = 12;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.PLANE_FLYOVER, 3);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOMB_DROP, NUM_EXPLOSIONS);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.FIREBOMB_EXPLODE, NUM_EXPLOSIONS);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BURN, 8);
	}

	public override IEnumerator UnitFiringAnimation(UnitState unit, UnitState target)
	{
		TeamState targetTeam = unit.team.otherTeam;
		int numMuzzleFlashes = abilityState.BoostValue;
		for (int i = 0; i < numMuzzleFlashes; i++)
		{
			yield return battleController.StartCoroutine(abilityState.target.unitView.PlayWeaponFiringAnimation());
		}
		AudioTrigger.NapalmAttack.Play();
		for (int j = 0; j < NUM_EXPLOSIONS; j++)
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
			bombDropAnim.AutoDestroy();
			bombDropAnim.transform.localScale = new Vector3(dirScaler, 1f, 1f);
			float bombAnimDelay = bombDropAnim.SpineAnimation.state.Animation.Duration;
			yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.FIREBOMB_EXPLODE, randPos, parentBattlefield, delegate(EffectInstance result)
			{
				result.AutoDestroy();
				result.Delay(bombAnimDelay);
			}));
			yield return new WaitForSeconds(bombAnimDelay * 0.1f);
			targetTeam.battleField.shaker.Shake(10f, 0.2f);
		}
		foreach (UnitView enemyUnit in battleController.GetUnitsByTeam(targetTeam))
		{
			int damage = GetNextDamage();
			enemyUnit.TakeDamage(damage);
			if (damage > 0)
			{
				enemyUnit.LocalDamagePerRound += abilityState.SecondaryBoostValue;
			}
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.5f);
	}
}
