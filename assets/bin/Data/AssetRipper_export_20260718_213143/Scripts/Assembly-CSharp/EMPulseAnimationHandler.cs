using System.Collections;
using UnityEngine;

public class EMPulseAnimationHandler : AbilityAnimationHandler
{
	private EffectInstance targetObj;

	protected IonStrikeEffect ionStrikeEffect;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.EM_PULSE, 1);
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.ION_STRIKE_INDICATOR_BLUE, 1);
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.SHORT_CIRCUIT_HIT, 1);
	}

	public override IEnumerator TeamBeginAttackAnimation(TeamState team)
	{
		UnitView targetUnit = abilityState.target.unitView;
		GlobalEffectsManager.Create(EffectType.EM_PULSE, targetUnit.TankSpritesTransform.position + new Vector3(0f, -60f, -1f), targetUnit.TankSpritesTransform).AutoDestroy();
		if ((bool)targetObj)
		{
			targetObj.Destroy();
		}
		if (team == battleController.playerTeam)
		{
			AudioTrigger.CrowdCheering.Play();
		}
		else
		{
			AudioTrigger.CrowdDisappointed.Play();
		}
		yield return new WaitForSeconds(1.75f);
		targetUnit.LocalPreventReroll = true;
		targetUnit.LocalRoundsUntilRerollEnabled++;
		targetUnit.TakeDamage(GetNextDamage());
		yield return new WaitForSeconds(0.75f);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		UnitView targetUnit = target.unitView;
		targetObj = GlobalEffectsManager.Create(EffectType.ION_STRIKE_INDICATOR_BLUE, targetUnit.TankSpritesTransform.position, targetUnit.TankSpritesTransform);
		ionStrikeEffect = targetObj.gameObject.GetComponent<IonStrikeEffect>();
		if ((bool)ionStrikeEffect)
		{
			ionStrikeEffect.SetTarget(targetUnit.TankSpritesTransform);
			ionStrikeEffect.ActivateEffect();
		}
		yield break;
	}

	public override IEnumerator JammedAnimation(ServerAbilityState jammerAnimation)
	{
		yield return battleController.StartCoroutine(ionStrikeEffect.JammedAnimation(abilityState.target.unitView.TankSpritesTransform));
	}
}
