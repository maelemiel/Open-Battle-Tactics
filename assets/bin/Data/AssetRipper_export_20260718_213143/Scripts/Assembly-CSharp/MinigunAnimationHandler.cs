using System.Collections;
using UnityEngine;

public class MinigunAnimationHandler : AbilityAnimationHandler
{
	private const int MINI_GUN_MAX_SHOTS = 3;

	private const string WEAPON_DEPLOY = "Mini Gun Special Deploy";

	private const string WEAPON_FIRE_LOW = "Mini Gun Special Low Fire";

	private const string WEAPON_FIRE_MED = "Mini Gun Special Med Fire";

	private const string WEAPON_FIRE_HIGH = "Mini Gun Special High Fire";

	private const string WEAPON_HIT_1 = "Mini Gun Special Hit 1";

	private const string WEAPON_HIT_2 = "Mini Gun Special Hit 2";

	private const string WEAPON_HIT_3 = "Mini Gun Special Hit 3";

	private static string[] WEAPON_HITS = new string[3] { "Mini Gun Special Hit 1", "Mini Gun Special Hit 2", "Mini Gun Special Hit 3" };

	private MinigunUnitAbility minigunHandler;

	private EffectInstance minigunInstance;

	private int extraShots;

	public override string InBattleName
	{
		get
		{
			if (minigunHandler == null)
			{
				return string.Empty;
			}
			return string.Format("ui_minigunSpecial_plus".Localize("MINIGUN +{0}"), abilityState.BoostValue + (minigunHandler.extraShots - 1) * abilityState.SecondaryBoostValue);
		}
	}

	public override void Init(BattleController battleController, AbilityState abilityState)
	{
		base.Init(battleController, abilityState);
		minigunHandler = abilityState.handler as MinigunUnitAbility;
	}

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.AppendPoolCapacity(EffectType.MINIGUN_WEAPON, 1);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.MINIGUN_HIT, 4);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.Mini_Gun_Deploy);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.Mini_Gun_Low);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.Mini_Gun_Med);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.Mini_Gun_High);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.Mini_Gun_Hit);
	}

	public override IEnumerator UnitFiringAnimation(UnitState unit, UnitState target)
	{
		if (minigunHandler.extraShots <= 1 || minigunInstance == null)
		{
			yield return battleController.StartCoroutine(DeployAnimation());
		}
		yield return battleController.StartCoroutine(FiringAnimation(target));
	}

	public override IEnumerator DestroyAnimation()
	{
		if (minigunInstance != null)
		{
			Object.Destroy(minigunInstance.gameObject);
		}
		minigunHandler = null;
		yield break;
	}

	private IEnumerator FiringAnimation(UnitState target)
	{
		UnitView targetUnit = ((target == null) ? (minigunHandler.target as UnitState) : target).unitView;
		string fireAnim = "Mini Gun Special Low Fire";
		if (extraShots == 2)
		{
			fireAnim = "Mini Gun Special Med Fire";
		}
		else if (extraShots >= 3)
		{
			fireAnim = "Mini Gun Special High Fire";
		}
		StartCoroutine(minigunInstance.SpineAnimation.PlayAnimCoroutine(fireAnim));
		GetShotSoundAudioTrigger(extraShots).Play();
		yield return new WaitForSeconds(1f);
		int currentDamage = GetNextDamage();
		if (targetUnit.LocalHealth > 0)
		{
			DamageTarget(targetUnit, currentDamage);
			yield return new WaitForSeconds(0.5f);
		}
		for (int i = 0; i < extraShots; i++)
		{
			currentDamage = GetNextDamage();
			if (targetUnit.LocalHealth > 0)
			{
				DamageTarget(targetUnit, currentDamage);
				yield return new WaitForSeconds(0.5f);
			}
		}
		extraShots++;
		yield return StartCoroutine(minigunInstance.SpineAnimation.WaitForAnimationComplete());
	}

	private IEnumerator DeployAnimation()
	{
		UnitView parentUnit = abilityState.target.unitView;
		minigunInstance = GlobalEffectsManager.Create(EffectType.MINIGUN_WEAPON, parentUnit.transform.position, parentUnit.GetUnitObject());
		tk2dSpriteDefinition.AttachPoint attachPoint = parentUnit.GetUnitAttachPointByName("minigun");
		if (attachPoint != null)
		{
			minigunInstance.transform.localPosition = Vector3.Scale(attachPoint.position, parentUnit.GetUnitScale());
			minigunInstance.transform.localPosition += Vector3.forward;
			minigunInstance.transform.localEulerAngles = new Vector3(0f, 0f, attachPoint.angle * Mathf.Sign(parentUnit.GetUnitScale().x));
		}
		AudioTrigger.Mini_Gun_Deploy.Play();
		yield return StartCoroutine(minigunInstance.SpineAnimation.PlayAnimCoroutine("Mini Gun Special Deploy"));
	}

	private void DamageTarget(UnitView target, int damage)
	{
		target.TakeDamage(damage);
		AudioTrigger.Mini_Gun_Hit.Play();
		EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.MINIGUN_HIT, target.transform.position, target.transform);
		effectInstance.SpineAnimation.AnimationName = WEAPON_HITS[Random.Range(0, WEAPON_HITS.Length)];
	}

	private AudioTrigger GetShotSoundAudioTrigger(int shotIndex)
	{
		AudioTrigger result = AudioTrigger.NONE;
		switch (shotIndex)
		{
		case 1:
			result = AudioTrigger.Mini_Gun_Low;
			break;
		case 2:
			result = AudioTrigger.Mini_Gun_Med;
			break;
		}
		if (shotIndex >= 3)
		{
			result = AudioTrigger.Mini_Gun_High;
		}
		return result;
	}
}
