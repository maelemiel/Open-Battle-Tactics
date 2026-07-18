using System.Collections;
using UnityEngine;

public class UnitItemPromoteViewController : MonoBehaviour
{
	[SerializeField]
	private tk2dSpineAnimation damageDiceEffect;

	[SerializeField]
	private tk2dSpineAnimation firstStrikeDiceEffect;

	[SerializeField]
	private tk2dSpineAnimation specialDiceEffect;

	[SerializeField]
	private tk2dSpineAnimation unitEffect;

	[SerializeField]
	private tk2dSpineAnimation unitEvolutionEffect;

	[SerializeField]
	private tk2dSpineAnimation healthEffect;

	[SerializeField]
	private Transform unitLevelUpEffectTransform;

	[SerializeField]
	private tk2dSpineAnimation unitLevelUpEffect;

	[SerializeField]
	private PriceLabelController promoteCostLabel;

	[SerializeField]
	private UnitInfoView unitInfoView;

	private EventUnitBoostDataModel boostModel;

	public float promoteUnitTime = 1f;

	public float promoteHealthTime = 1f;

	public float promoteDieFaceTime = 0.5f;

	private void Awake()
	{
		damageDiceEffect.gameObject.SetActive(false);
		firstStrikeDiceEffect.gameObject.SetActive(false);
		specialDiceEffect.gameObject.SetActive(false);
		unitEffect.gameObject.SetActive(false);
		unitEvolutionEffect.gameObject.SetActive(false);
		healthEffect.gameObject.SetActive(false);
		unitLevelUpEffect.gameObject.SetActive(false);
	}

	public IEnumerator PlayPromoteEffect(PromotionLocalData previousUnitData, PromotionLocalData currentUnitData, UserUnit unit)
	{
		EventDataModel currentEvent = UserProfile.player.GetActiveEvent();
		if (currentEvent != null)
		{
			boostModel = EventUnitBoostDataModel.FindUnitBoost(unit.UnitDataModel.id, unit.level, currentEvent.id);
		}
		UnitPartialLevelDataModel.PartialLevel partialLevel = UnitPartialLevelDataModel.SetupPartialLevel(unit.metadataId, unit.level, previousUnitData.partialLevel);
		if (previousUnitData == null || currentUnitData == null)
		{
			yield break;
		}
		if (previousUnitData.assetBundleID != currentUnitData.assetBundleID)
		{
			yield return StartCoroutine(EvolveUnit(currentUnitData.assetBundleID));
			unitInfoView.SetUnitLevel(currentUnitData.level);
		}
		else
		{
			StartCoroutine(PromoteUnit(currentUnitData.level));
		}
		if (previousUnitData.health != currentUnitData.health)
		{
			yield return StartCoroutine(PromoteHealth(currentUnitData.health));
		}
		for (int i = 0; i < previousUnitData.dieValues.Length; i++)
		{
			if (previousUnitData.dieValues[i] != currentUnitData.dieValues[i] || previousUnitData.dieFaceTypes[i] != currentUnitData.dieFaceTypes[i])
			{
				yield return StartCoroutine(PromoteDieFace(i, currentUnitData.dieFaceTypes, currentUnitData.dieValues));
			}
		}
		if (previousUnitData.unitLevelProgressionDataModel != null && currentUnitData.unitLevelProgressionDataModel != null && (previousUnitData.unitLevelProgressionDataModel.specialId != currentUnitData.unitLevelProgressionDataModel.specialId || previousUnitData.unitLevelProgressionDataModel.specialBoostValueA != currentUnitData.unitLevelProgressionDataModel.specialBoostValueA || previousUnitData.unitLevelProgressionDataModel.specialBoostValueB != currentUnitData.unitLevelProgressionDataModel.specialBoostValueB))
		{
			AudioTrigger.GachaSuperRareRevealed.Play();
			yield return StartCoroutine(PromoteUnitSpecial(1f));
		}
		unitInfoView.SetUnitAbilityDescription(unit.UnitDataModel, currentUnitData.level - 1, boostModel, partialLevel);
		yield return new WaitForSeconds(0.5f);
		if ((bool)promoteCostLabel)
		{
			promoteCostLabel.ConfigurePriceLabel(currentUnitData.promotionCost);
		}
	}

	private IEnumerator EvolveUnit(int assetBundleID)
	{
		AudioTrigger.GachaRareRevealed.Play();
		float animationDuration = unitEvolutionEffect.state.Animation.Duration;
		if (animationDuration > promoteUnitTime)
		{
			Log.Warning("Animation time and configured time are inconsistent. Clamping the value", base.gameObject);
			promoteUnitTime = unitEvolutionEffect.state.Animation.Duration;
		}
		yield return StartCoroutine(PlayAnimation(unitInfoView.UnitGameObject.transform.position, unitEvolutionEffect, animationDuration * 0.5f));
		StartCoroutine(unitInfoView.SetAssetBundle(assetBundleID));
		AudioTrigger.SpecialResult.Play();
		yield return new WaitForSeconds(promoteUnitTime - animationDuration * 0.5f);
	}

	private IEnumerator PromoteUnit(int level)
	{
		AudioTrigger.GachaRareRevealed.Play();
		float animationDuration = unitEffect.state.Animation.Duration;
		if (animationDuration > promoteUnitTime)
		{
			Log.Warning("Animation time and configured time are inconsistent. Clamping the value", base.gameObject);
			promoteUnitTime = unitEffect.state.Animation.Duration;
		}
		yield return StartCoroutine(PlayAnimation(unitInfoView.UnitGameObject.transform.position, unitEffect, animationDuration * 0.75f));
		unitInfoView.SetUnitLevel(level);
		yield return new WaitForSeconds(promoteUnitTime - animationDuration * 0.25f);
	}

	private IEnumerator PromoteHealth(int healthValue)
	{
		float animationDuration = healthEffect.state.Animation.Duration;
		if (animationDuration > promoteHealthTime)
		{
			Log.Warning("Animation time and configured time are inconsistent. Clamping the value", base.gameObject);
			promoteHealthTime = healthEffect.state.Animation.Duration;
		}
		unitInfoView.SetUnitHP(healthValue);
		AudioTrigger.HighFirstStrikeResult.Play();
		yield return StartCoroutine(PlayAnimation(unitInfoView.HealthLabel.transform.position, healthEffect, promoteHealthTime));
	}

	private IEnumerator PromoteUnitSpecial(float delay)
	{
		if ((bool)unitLevelUpEffect)
		{
			yield return StartCoroutine(PlayAnimation(unitLevelUpEffectTransform.position, unitLevelUpEffect, delay));
		}
	}

	private IEnumerator PromoteDieFace(int dieFaceIndex, DieFaceType[] dieFaceTypes, int[] dieFaceValues)
	{
		bool boosted = false;
		if (boostModel != null)
		{
			switch (dieFaceTypes[dieFaceIndex])
			{
			case DieFaceType.ArmourPiercing:
				if (boostModel.dieBoostArmourPiercing > 0)
				{
					boosted = true;
				}
				break;
			case DieFaceType.DirectDamage:
				if (boostModel.dieBoostArmourPiercing > 0)
				{
					boosted = true;
				}
				break;
			case DieFaceType.Initiative:
				if (boostModel.dieBoostInitiative > 0)
				{
					boosted = true;
				}
				break;
			}
		}
		unitInfoView.SetDieFace(dieFaceIndex, dieFaceTypes, dieFaceValues, boosted);
		tk2dSpineAnimation dieFaceAnimation = null;
		switch (dieFaceTypes[dieFaceIndex])
		{
		case DieFaceType.DirectDamage:
		case DieFaceType.ArmourPiercing:
		case DieFaceType.AcidStrike:
			dieFaceAnimation = damageDiceEffect;
			AudioTrigger.HighDamageResult.Play();
			break;
		case DieFaceType.Initiative:
			dieFaceAnimation = firstStrikeDiceEffect;
			AudioTrigger.HighFirstStrikeResult.Play();
			break;
		case DieFaceType.Special:
			dieFaceAnimation = specialDiceEffect;
			AudioTrigger.SpecialResult.Play();
			break;
		}
		if ((bool)dieFaceAnimation)
		{
			float animationDuration = dieFaceAnimation.state.Animation.Duration;
			if (animationDuration > promoteDieFaceTime)
			{
				Log.Warning("Animation time and configured time are inconsistent. Clamping the value", base.gameObject);
				promoteDieFaceTime = dieFaceAnimation.state.Animation.Duration;
			}
			yield return StartCoroutine(PlayAnimation(unitInfoView.DieFaces[dieFaceIndex].transform.position, dieFaceAnimation, promoteDieFaceTime));
		}
	}

	private IEnumerator PlayAnimation(Vector3 position, tk2dSpineAnimation spineAnimation, float delayTime)
	{
		Log.DebugTag("Playing: " + spineAnimation.name, null, "UNITITempromoteview");
		if ((bool)spineAnimation)
		{
			spineAnimation.gameObject.transform.position = position;
			spineAnimation.gameObject.SetActive(true);
			spineAnimation.state.Time = 0f;
			spineAnimation.AnimationComplete += DeactivateAnimation;
			yield return new WaitForSeconds(delayTime);
		}
	}

	private void DeactivateAnimation(tk2dSpineAnimation spineAnimation)
	{
		spineAnimation.AnimationComplete -= DeactivateAnimation;
		spineAnimation.gameObject.SetActive(false);
	}
}
