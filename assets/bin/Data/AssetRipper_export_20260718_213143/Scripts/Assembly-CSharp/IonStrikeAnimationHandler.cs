using System.Collections;
using UnityEngine;

public class IonStrikeAnimationHandler : AbilityAnimationHandler
{
	protected IonStrikeEffect ionStrikeEffect;

	protected Vector3 OFFSET_POSITION_EFFECT = new Vector3(0f, -50f, -5f);

	protected string StrikeText;

	protected override void LoadDependencies()
	{
		StrikeText = "ui_battle_ionstrikeactivated";
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.ION_STRIKE, 1);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState(StrikeText.Localize("Strike Initialized!"));
		}
		CleanupIndicator();
		AudioTrigger.TargetingStarted.Play();
		UnitView targetUnit = target.unitView;
		EffectInstance targetObj = GlobalEffectsManager.Create(EffectType.ION_STRIKE_INDICATOR, targetUnit.TankSpritesTransform.position, targetUnit.TankSpritesTransform);
		ionStrikeEffect = targetObj.gameObject.GetComponent<IonStrikeEffect>();
		if ((bool)ionStrikeEffect)
		{
			ionStrikeEffect.SetTarget(targetUnit.TankSpritesTransform);
			ionStrikeEffect.ActivateEffect();
		}
		yield return new WaitForSeconds(1f);
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToMainState();
		}
	}

	public override IEnumerator JammedAnimation(ServerAbilityState jammerAnimation)
	{
		yield return battleController.StartCoroutine(ionStrikeEffect.JammedAnimation(abilityState.target.unitView.TankSpritesTransform));
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
		AudioTrigger.IonStrikeActivated.Play();
		targetObj = GlobalEffectsManager.Create(EffectType.ION_STRIKE, targetUnit.unitView.TankSpritesTransform.position + OFFSET_POSITION_EFFECT, targetUnit.unitView.TankSpritesTransform);
		itempIonStrikeEffect = targetObj.gameObject.GetComponent<tk2dSpineAnimation>();
		yield return new WaitForSeconds(itempIonStrikeEffect.GetAnimationDuration("Ion Strike Revisions") - 0.7f);
		if ((bool)ionStrikeEffect)
		{
			GlobalEffectsManager.Return(ionStrikeEffect.gameObject);
			ionStrikeEffect = null;
		}
		targetUnit.unitView.TakeDamage(GetNextDamage());
		yield return new WaitForSeconds(0.75f);
		Object.Destroy(targetObj);
	}

	public override IEnumerator DeactivationAnimation()
	{
		CleanupIndicator();
		yield break;
	}

	protected void CleanupIndicator()
	{
		if ((bool)ionStrikeEffect)
		{
			ionStrikeEffect.DeactivateEffect(delegate
			{
				GlobalEffectsManager.Return(ionStrikeEffect.gameObject);
			});
		}
	}
}
