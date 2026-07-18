using System.Collections;
using UnityEngine;

public class HotShotAnimationHandler : AbilityAnimationHandler
{
	private HotShotAbility hotShotHandler;

	private float screenShake = 1f;

	private float kickbackAmount = 50f;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOMB_DROP, 1);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.HOTSHOT_FIRE, 1);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.HOTSHOT_HIT, 1);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BURN, 2);
	}

	public override void Init(BattleController battleController, AbilityState abilityState)
	{
		base.Init(battleController, abilityState);
		hotShotHandler = abilityState.handler as HotShotAbility;
	}

	public override IEnumerator UnitFiringAnimation(UnitState unit, UnitState target)
	{
		UnitView unitView = abilityState.target.unitView;
		UnitView targetUnit = ((target == null) ? ((UnitState)hotShotHandler.target) : target).unitView;
		AudioTrigger.BattleCannonAttack.Play();
		unitView.ShakeMyField(screenShake, 0.1f);
		unitView.PlayKickbackAnimation(kickbackAmount);
		EffectInstance fireEffect = null;
		yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.HOTSHOT_FIRE, unitView.TankSpritesTransform.position, unitView.TankSpritesTransform.gameObject, delegate(EffectInstance result)
		{
			fireEffect = result;
		}));
		fireEffect.AutoDestroy();
		fireEffect.transform.Translate(0f, 0f, -20f);
		UnitWeaponSystem.UnitWeaponData weapon = unitView.WeaponSystem.GetCurrentWeaponData(unitView);
		fireEffect.transform.Rotate(Vector3.forward, weapon.angle);
		fireEffect.transform.MultiplyScale(-1f, 1f, 1f);
		Vector3 anchorOffset = weapon.anchorPt;
		if (unitView.isEnemy)
		{
			anchorOffset.x *= -1f;
		}
		fireEffect.transform.Translate(anchorOffset);
		fireEffect.SpineAnimation.AnimationName = fireEffect.SpineAnimation.GetAnimationNames()[0];
		yield return new WaitForSeconds(0.5f);
		float dirScaler = ((!targetUnit.Team.IsEnemy) ? (-1f) : 1f);
		Vector3 bombPosition = targetUnit.TankSpritesTransform.position;
		bombPosition.z -= 1f;
		EffectInstance bombDropAnim = null;
		yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.BOMB_DROP, bombPosition, targetUnit.gameObject, delegate(EffectInstance result)
		{
			bombDropAnim = result;
		}));
		bombDropAnim.AutoDestroy();
		bombDropAnim.transform.localScale = new Vector3(dirScaler, 1f, 1f);
		float bombAnimDelay = bombDropAnim.SpineAnimation.state.Animation.Duration;
		yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.HOTSHOT_HIT, bombPosition, targetUnit.gameObject, delegate(EffectInstance result)
		{
			result.AutoDestroy();
			result.Delay(bombAnimDelay);
		}));
		yield return new WaitForSeconds(bombAnimDelay);
		targetUnit.Team.battleField.shaker.Shake(10f, 0.2f);
		targetUnit.TakeDamage(GetNextDamage());
		targetUnit.LocalDamagePerRound += abilityState.SecondaryBoostValue;
		yield return new WaitForSeconds(1f);
	}
}
