using System.Collections;

public class TargetingAdvanceAnimationHandler : TargetingAnimationHandler
{
	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.TARGET_STRIKE, 1);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		targetUnit = target.unitView;
		EffectInstance effect = GlobalEffectsManager.Create(EffectType.TARGET_STRIKE, targetUnit.TankSpritesTransform.position, targetUnit.TankSpritesTransform);
		targetIndicator = effect.gameObject.GetComponent<TargetIndicator>();
		if ((bool)targetIndicator)
		{
			targetIndicator.SetTarget(targetUnit, TargetIndicator.TargetIndicatorType.ADVANCED);
		}
		AudioTrigger.TargetingStarted.Play();
		targetUnit.LocalExtraDamage += abilityState.metadata.BoostValue;
		yield break;
	}

	public override IEnumerator DeactivationAnimation()
	{
		UnitView targetUnit = abilityState.target.unitView;
		if ((bool)targetUnit && !targetUnit.LocalIsDead)
		{
			targetUnit.LocalExtraDamage -= abilityState.metadata.BoostValue;
		}
		yield return StartCoroutine(CleanupIndicator(TargetIndicator.TargetIndicatorType.ADVANCED));
	}
}
