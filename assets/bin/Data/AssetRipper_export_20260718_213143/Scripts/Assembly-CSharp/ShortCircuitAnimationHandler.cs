using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class ShortCircuitAnimationHandler : AbilityAnimationHandler
{
	private ShortCircuitAbility shortCircuitHandler;

	private RailgunEffect railgunEffect;

	private EffectInstance railgunProjectile;

	private float kickBackAmount = 10f;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.RAILGUN, 1);
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.SHORT_CIRCUIT_HIT, 1);
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.SHORT_CIRCUIT_PROJECTILE, 1);
	}

	public override void Init(BattleController battleController, AbilityState abilityState)
	{
		base.Init(battleController, abilityState);
		shortCircuitHandler = abilityState.handler as ShortCircuitAbility;
	}

	public override IEnumerator UnitFiringAnimation(UnitState unit, UnitState target)
	{
		if (railgunEffect == null)
		{
			CreateEffects();
		}
		yield return battleController.StartCoroutine(FiringAnimation(target));
	}

	private IEnumerator FiringAnimation(UnitState target)
	{
		railgunEffect.Chargeup();
		yield return new WaitForSeconds(0.5f);
		UnitState targetUnit = ((target == null) ? ((UnitState)shortCircuitHandler.target) : target);
		yield return battleController.StartCoroutine(ProjectileAnimation(targetUnit.unitView));
		targetUnit.unitView.TakeDamage(GetNextDamage());
		targetUnit.unitView.LocalPreventReroll = true;
		targetUnit.unitView.LocalRoundsUntilRerollEnabled++;
		railgunEffect.Idle();
	}

	private IEnumerator ProjectileAnimation(UnitView target)
	{
		float directionScaler = abilityState.team.battleField.directionScaler;
		Vector3 projectileStartPos = railgunEffect.transform.position;
		UnitView parentUnit = abilityState.target.unitView;
		tk2dSpriteDefinition.AttachPoint attachPoint = parentUnit.GetUnitAttachPointByName("cannon");
		if (attachPoint != null)
		{
			projectileStartPos += attachPoint.position;
			projectileStartPos.z = 0f;
		}
		parentUnit.PlayKickbackAnimation(kickBackAmount);
		railgunProjectile = GlobalEffectsManager.Create(EffectType.SHORT_CIRCUIT_PROJECTILE, projectileStartPos, abilityState.team.battleField.gameObject);
		Vector3 scale = railgunProjectile.transform.localScale;
		railgunProjectile.transform.localScale = new Vector3(scale.x * directionScaler, scale.y, scale.z);
		railgunProjectile.transform.position = projectileStartPos;
		UnitView target2 = default(UnitView);
		SimpleTween.Start(0f, 1f, 1.5f, EaseType.Linear, delegate(float val)
		{
			val *= 0.3f;
			if (val >= 0.15f)
			{
				val += 0.7f;
			}
			railgunProjectile.transform.position = Vector3.Lerp(projectileStartPos, target2.transform.position, val);
			railgunProjectile.gameObject.layer = ((!((double)val < 0.5)) ? target2.gameObject.layer : abilityState.team.battleField.gameObject.layer);
		});
		yield return new WaitForSeconds(1.5f);
		GlobalEffectsManager.Return(railgunProjectile);
		Vector3 effectPosition = new Vector3(target.TankSpritesTransform.position.x, target.TankSpritesTransform.position.y, target.TankSpritesTransform.position.z - 1f);
		GlobalEffectsManager.Create(EffectType.SHORT_CIRCUIT_HIT, effectPosition, target.Team.battleField.gameObject).AutoDestroy();
		yield return new WaitForSeconds(0.7f);
	}

	private void CreateEffects()
	{
		UnitView unitView = abilityState.target.unitView;
		EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.RAILGUN, unitView.transform.position, unitView.GetUnitObject());
		railgunEffect = effectInstance.GetComponent<RailgunEffect>();
		tk2dSpriteDefinition.AttachPoint unitAttachPointByName = unitView.GetUnitAttachPointByName("railgun_attachment");
		if (unitAttachPointByName != null)
		{
			effectInstance.transform.localPosition = Vector3.Scale(unitAttachPointByName.position, unitView.GetUnitScale());
			effectInstance.transform.localEulerAngles = new Vector3(0f, 0f, unitAttachPointByName.angle * Mathf.Sign(unitView.GetUnitScale().x));
		}
		effectInstance.transform.Translate(0f, 0f, -1f);
	}
}
