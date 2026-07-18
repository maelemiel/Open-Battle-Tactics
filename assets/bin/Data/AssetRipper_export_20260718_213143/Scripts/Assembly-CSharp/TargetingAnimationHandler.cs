using System.Collections;
using UnityEngine;

public class TargetingAnimationHandler : AbilityAnimationHandler
{
	protected readonly string JAMMER_PER_UNIT_ANIMATION = "Wave";

	protected TargetIndicator targetIndicator;

	protected UnitView targetUnit;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.TARGET, 1);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		targetUnit = target.unitView;
		EffectInstance effect = GlobalEffectsManager.Create(EffectType.TARGET, targetUnit.TankSpritesTransform.position, targetUnit.TankSpritesTransform);
		targetIndicator = effect.gameObject.GetComponent<TargetIndicator>();
		if ((bool)targetIndicator)
		{
			targetIndicator.SetTarget(targetUnit, TargetIndicator.TargetIndicatorType.RED);
		}
		AudioTrigger.TargetingStarted.Play();
		yield break;
	}

	public override IEnumerator DeactivationAnimation()
	{
		yield return StartCoroutine(CleanupIndicator(TargetIndicator.TargetIndicatorType.RED));
	}

	public override IEnumerator JammedAnimation(ServerAbilityState jammerAnimation)
	{
		EffectType effectType = EffectType.NONE;
		switch (jammerAnimation.metadata.Type)
		{
		case "jammer":
			effectType = EffectType.JAMMER;
			break;
		case "jammer2":
			effectType = EffectType.JAMMER_2;
			break;
		}
		EffectInstance jammedEffect = GlobalEffectsManager.Create(effectType, targetUnit.gameObject.transform.position, targetUnit.TankSpritesTransform);
		tk2dSpineAnimation spineAnimation = jammedEffect.GetComponent<tk2dSpineAnimation>();
		if ((bool)spineAnimation)
		{
			spineAnimation.AnimationName = JAMMER_PER_UNIT_ANIMATION;
		}
		spineAnimation.transform.SetLocalXScale(-1f);
		battleController.CubeBar.GoToTextState("ui_battle_jammer2used".Localize("Target Jammed!"));
		yield return new WaitForSeconds(0.75f);
		yield return StartCoroutine(DeactivationAnimation());
	}

	protected IEnumerator CleanupIndicator(TargetIndicator.TargetIndicatorType indicatorType)
	{
		if ((bool)targetIndicator)
		{
			battleController.StartCoroutine(targetIndicator.AnimateBack(indicatorType));
			targetIndicator = null;
			yield return new WaitForSeconds(2f);
			battleController.CubeBar.GoToTextState(string.Empty);
		}
	}
}
