using System.Collections;
using UnityEngine;

public class SmallIonStrikeAnimationHandler : IonStrikeAnimationHandler
{
	protected override void LoadDependencies()
	{
		StrikeText = "ui_battle_smallstrikeactivated";
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.SMALL_ION_STRIKE, 1);
	}

	public override IEnumerator TeamBeginAttackAnimation(TeamState team)
	{
		UnitState targetUnit = abilityState.target;
		ionStrikeEffect.ExplosionEffect();
		if (team == battleController.playerTeam)
		{
			AudioTrigger.CrowdCheering.Play();
		}
		else
		{
			AudioTrigger.CrowdDisappointed.Play();
		}
		tk2dSpineAnimation itempIonStrikeEffect = null;
		EffectInstance targetObj = null;
		targetObj = GlobalEffectsManager.Create(EffectType.SMALL_ION_STRIKE, targetUnit.unitView.TankSpritesTransform.position + OFFSET_POSITION_EFFECT, targetUnit.unitView.TankSpritesTransform);
		itempIonStrikeEffect = targetObj.gameObject.GetComponent<tk2dSpineAnimation>();
		AudioTrigger.SmallIonStrikeActivated.Play();
		yield return new WaitForSeconds(itempIonStrikeEffect.GetAnimationDuration("Ion Strike") - 1f);
		AudioTrigger.MissileFiringHit.Play();
		if ((bool)ionStrikeEffect)
		{
			GlobalEffectsManager.Return(ionStrikeEffect.gameObject);
			ionStrikeEffect = null;
		}
		targetUnit.unitView.TakeDamage(GetNextDamage());
		yield return new WaitForSeconds(0.75f);
		Object.Destroy(targetObj);
	}
}
