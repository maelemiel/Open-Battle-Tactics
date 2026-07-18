using System.Collections;
using UnityEngine;

public class AoeDamageAnimationHandler : AbilityAnimationHandler
{
	public int ExplosionCount
	{
		get
		{
			return Mathf.Min(20, abilityState.BoostValue * 3);
		}
	}

	public int MissileCount
	{
		get
		{
			return Mathf.Min(20, abilityState.BoostValue);
		}
	}

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.MISSILE_EXPLOSION, MissileCount);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOMB_EXPLODE, ExplosionCount);
	}

	public override IEnumerator UnitFiringAnimation(UnitState unit, UnitState target)
	{
		int numMissiles = Mathf.Min(abilityState.BoostValue, Constants.MultiStrikeMaxMissileCount);
		for (int i = 0; i < numMissiles; i++)
		{
			UnitView unitView = abilityState.target.unitView;
			UnitWeaponSystem.UnitWeaponData weaponData = unitView.WeaponSystem.GetWeaponDataWithType(UnitWeaponType.Missile);
			if (weaponData != null)
			{
				yield return battleController.StartCoroutine(weaponData.handler.FiringAnimation(abilityState.target.unitView));
				continue;
			}
			string unitID = "N/A";
			if (abilityState.target.UserUnitMetadata != null)
			{
				unitID = abilityState.target.UserUnitMetadata.metadataId;
			}
			Debug.LogWarning("Unit '" + unitID + "' should be using MissileWeaponAnimationHandler, but it is using the default one instead");
			yield return battleController.StartCoroutine(abilityState.target.unitView.PlayWeaponFiringAnimation());
		}
		BattleField enemyBattlefield = unit.team.otherTeam.battleField;
		yield return new WaitForSeconds(0.5f);
		for (int j = 0; j < numMissiles; j++)
		{
			Vector3 randomPosition = enemyBattlefield.unityCamera.ViewportToWorldPoint(new Vector3(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), 0f));
			EffectInstance burstEffect = GlobalEffectsManager.Create(EffectType.MISSILE_EXPLOSION, randomPosition, enemyBattlefield.gameObject).AutoDestroy();
			if (abilityState.team != battleController.playerTeam)
			{
				burstEffect.transform.localScale = new Vector3(0f - burstEffect.transform.localScale.x, burstEffect.transform.localScale.y, burstEffect.transform.localScale.z);
			}
			yield return new WaitForSeconds(0.15f);
			StartCoroutine(PlaySoundWithDelay(AudioTrigger.MissileFiringHit, 0.3f));
			enemyBattlefield.shaker.Shake(2f, 0.1f);
		}
		UnitView[] array = battleController.GetUnitsByTeam(unit.team.otherTeam).ToArray();
		foreach (UnitView enemyUnit in array)
		{
			enemyUnit.TakeDamage(GetNextDamage());
			yield return new WaitForSeconds(0.15f);
		}
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator PlaySoundWithDelay(AudioTrigger trigger, float delay)
	{
		yield return new WaitForSeconds(delay);
		trigger.Play();
	}
}
