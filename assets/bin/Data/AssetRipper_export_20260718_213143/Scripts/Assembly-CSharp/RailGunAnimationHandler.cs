using System.Collections;
using UnityEngine;

public class RailGunAnimationHandler : AbilityAnimationHandler
{
	private const string RAIL_GUN_SHOT_ANIMATION_NAME = "New Rail Gun";

	private RailGunAbility railgunHandler;

	private RailgunEffect railgunEffect;

	private Vector3 railgunBeamOffset = new Vector3(-84f, 20f, 0f);

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.RAILGUN, 1);
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.RAILGUN_HIT, 1);
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.RAILGUN_SHOT, 1);
	}

	public override void Init(BattleController battleController, AbilityState abilityState)
	{
		base.Init(battleController, abilityState);
		railgunHandler = abilityState.handler as RailGunAbility;
	}

	public override IEnumerator UnitFiringAnimation(UnitState unit, UnitState target)
	{
		if (railgunHandler.shouldDeploy || railgunEffect == null)
		{
			yield return battleController.StartCoroutine(DeployAnimation());
		}
		else
		{
			yield return battleController.StartCoroutine(FiringAnimation(target));
		}
	}

	public override IEnumerator DestroyAnimation()
	{
		Object.Destroy(railgunEffect);
		railgunHandler = null;
		yield break;
	}

	private IEnumerator FiringAnimation(UnitState target)
	{
		float directionScale = ((abilityState.team != battleController.playerTeam) ? 1f : (-1f));
		UnitState unitTarget = ((target == null) ? ((UnitState)railgunHandler.target) : target);
		Vector3 offset = new Vector3(railgunBeamOffset.x * directionScale, railgunBeamOffset.y, railgunBeamOffset.z);
		EffectInstance fireRailgunEffect = GlobalEffectsManager.Create(EffectType.RAILGUN_SHOT, railgunEffect.transform.position + offset, railgunEffect.gameObject);
		AudioTrigger.RailgunStarted.Play();
		yield return new WaitForSeconds(1.25f);
		railgunEffect.Recoil();
		yield return new WaitForSeconds(0.2f);
		Object.Destroy(fireRailgunEffect.gameObject);
		yield return battleController.StartCoroutine(ProjectileAnimation(unitTarget.unitView));
		unitTarget.unitView.TakeDamage(GetNextDamage());
		railgunEffect.Idle();
	}

	private IEnumerator ProjectileAnimation(UnitView target)
	{
		Vector3 effectPosition = new Vector3(target.TankSpritesTransform.position.x, target.TankSpritesTransform.position.y, target.TankSpritesTransform.position.z - 1f);
		EffectInstance railgunEnemyHit = GlobalEffectsManager.Create(EffectType.RAILGUN_HIT, effectPosition, target.Team.battleField.gameObject).AutoDestroy();
		float directionScale = ((!abilityState.target.unitView.isEnemy) ? (-1f) : 1f);
		Vector3 currentScale = railgunEnemyHit.transform.localScale;
		railgunEnemyHit.transform.localScale = new Vector3(currentScale.x * directionScale, currentScale.y, currentScale.z);
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator DeployAnimation()
	{
		if (railgunEffect == null)
		{
			CreateEffects();
		}
		railgunEffect.Deploy();
		while (railgunEffect.IsDeploying)
		{
			yield return 0;
		}
		yield return new WaitForSeconds(1f);
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
