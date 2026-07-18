using System.Collections;
using UnityEngine;

public class UnitItemUpgradeViewController : MonoBehaviour
{
	public float glowSize = 0.75f;

	[SerializeField]
	private tk2dSpineAnimation partsUpgrade;

	[SerializeField]
	private tk2dSpineAnimation glowAnim;

	[SerializeField]
	private UnitInfoView unitView;

	[SerializeField]
	private AnimationCurve growthCurve;

	[SerializeField]
	private AnimationCurve shrinkCurve;

	[SerializeField]
	private CameraShake cameraShake;

	[SerializeField]
	private CameraShake abilityShake;

	[SerializeField]
	private UpgradePartRequirementController[] partObjects;

	private EventUnitBoostDataModel boostModel;

	public float animLength = 1f;

	public bool force = true;

	public float timmmer = 0.15f;

	[ContextMenu("Test Upgrade (HA!)")]
	public void TestUpgrade()
	{
		UserUnit userUnit = null;
		foreach (UserUnit value in UserProfile.player.unitInventory.Values)
		{
			if (value.metadataId == unitView.stored.id)
			{
				userUnit = value;
				break;
			}
		}
		PromotionLocalData previousUnitData = new PromotionLocalData(userUnit.AssetBundleID, userUnit.StartingHealth, userUnit.level, userUnit.partialLevel, userUnit.RollValues, userUnit.RollTypes, userUnit.GetUpgradePrice(), userUnit.UnitDataModel.GetLevel(userUnit.level - 1));
		int[] array = new int[userUnit.RollValues.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = userUnit.RollValues[i] + 2;
		}
		PromotionLocalData currentUnitData = new PromotionLocalData(userUnit.AssetBundleID, userUnit.StartingHealth + 2, userUnit.level + 1, 0, array, userUnit.RollTypes, userUnit.GetUpgradePrice(), userUnit.UnitDataModel.GetLevel(userUnit.level));
		StartCoroutine(PlayUpgradeEffect(previousUnitData, currentUnitData, userUnit, true));
	}

	public IEnumerator PlayUpgradeEffect(PromotionLocalData previousUnitData, PromotionLocalData currentUnitData, UserUnit unit, bool test = false)
	{
		if (test)
		{
			for (int i = partObjects.Length - 1; i >= 0; i--)
			{
				partObjects[i].priceLabel.gameObject.SetActive(true);
				partObjects[i].SetSprites(true);
			}
			yield return new WaitForSeconds(1f);
		}
		partsUpgrade.transform.position = unitView.transform.position;
		bool animating = true;
		AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		float timer = timmmer;
		Vector3[] originals = new Vector3[partObjects.Length];
		for (int i2 = partObjects.Length - 1; i2 >= 0; i2--)
		{
			originals[i2] = partObjects[i2].priceLabel.transform.localPosition;
			yield return StartCoroutine(partObjects[i2].requirementView.PlayUpgradeAnimation(timer));
			partObjects[i2].SetSprites(false);
			StartCoroutine(MoveToPositionAndScale(partObjects[i2].priceLabel.transform, unitView.transform.position, Vector3.one * 0.25f + Vector3.forward, timer, curve));
		}
		while (animating)
		{
			float storedZ = unitView.transform.position.z;
			unitView.unitProxy.Prefab.transform.position = new Vector3(partsUpgrade.Skeleton.skeleton.Bones[1].WorldX, partsUpgrade.Skeleton.skeleton.Bones[1].WorldY, storedZ);
			yield return new WaitForEndOfFrame();
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				animating = false;
			}
		}
		unitView.unitProxy.Prefab.transform.localPosition = Vector3.zero;
		yield return StartCoroutine(PlayPromoteEffect(previousUnitData, currentUnitData, unit));
		for (int i3 = partObjects.Length - 1; i3 >= 0; i3--)
		{
			partObjects[i3].priceLabel.transform.localPosition = originals[i3];
			partObjects[i3].priceLabel.transform.localScale = Vector3.one;
			partObjects[i3].gameObject.SetActive(true);
		}
	}

	public IEnumerator MoveToPositionAndScale(Transform toMove, Vector3 position, Vector3 scale, float length, AnimationCurve curve)
	{
		Vector3 start = toMove.position;
		for (float time = length; time >= 0f; time -= Time.deltaTime)
		{
			toMove.position = Vector3.Lerp(start, position, curve.Evaluate(1f - time / length));
			toMove.localScale = Vector3.Lerp(Vector3.one, scale, shrinkCurve.Evaluate(1f - time / length));
			yield return new WaitForEndOfFrame();
		}
		toMove.position = position;
		toMove.localScale = scale;
		toMove.gameObject.SetActive(false);
	}

	public IEnumerator PlayPromoteEffect(PromotionLocalData previousUnitData, PromotionLocalData currentUnitData, UserUnit unit)
	{
		EventDataModel currentEvent = UserProfile.player.GetActiveEvent();
		if (currentEvent != null)
		{
			boostModel = EventUnitBoostDataModel.FindUnitBoost(unit.UnitDataModel.id, unit.level, currentEvent.id);
		}
		UnitPartialLevelDataModel.PartialLevel previousPartialLevel = UnitPartialLevelDataModel.SetupPartialLevel(unit.metadataId, unit.level, previousUnitData.partialLevel);
		UnitPartialLevelDataModel.PartialLevel currentPartialLevel = UnitPartialLevelDataModel.SetupPartialLevel(unit.metadataId, unit.level, currentUnitData.partialLevel);
		if (previousUnitData == null || currentUnitData == null)
		{
			yield break;
		}
		if (previousUnitData.health != currentUnitData.health)
		{
			yield return StartCoroutine(PromoteHealth(currentUnitData.health));
		}
		for (int i = 0; i < previousUnitData.dieValues.Length; i++)
		{
			if (previousUnitData.dieValues[i] != currentUnitData.dieValues[i] || previousUnitData.dieFaceTypes[i] != currentUnitData.dieFaceTypes[i])
			{
				AudioTrigger.GachaRareRevealed.Play();
				yield return StartCoroutine(PromoteDieFace(i, currentUnitData.dieFaceTypes, currentUnitData.dieValues));
			}
		}
		if (previousPartialLevel.special1BoostA != currentPartialLevel.special1BoostA || previousPartialLevel.special1BoostB != currentPartialLevel.special1BoostB)
		{
			yield return StartCoroutine(PromoteAbility(unit, currentUnitData.level - 1, currentPartialLevel));
		}
	}

	[ContextMenu("Test Ability")]
	public void TestAbilityhAnim()
	{
		StartCoroutine(PromoteAbility(null, 1, null));
	}

	private IEnumerator PromoteAbility(UserUnit unit, int level, UnitPartialLevelDataModel.PartialLevel partialLevel)
	{
		if (!(abilityShake == null))
		{
			abilityShake.Shake();
			yield return new WaitForSeconds(0.5f);
			if (unit != null)
			{
				unitView.SetUnitAbilityDescription(unit.UnitDataModel, level, boostModel, partialLevel);
			}
			while (abilityShake.Intensity > 0f)
			{
				yield return new WaitForEndOfFrame();
			}
		}
	}

	[ContextMenu("Test Health")]
	public void TestHealthAnim()
	{
		unitView.SetUnitHP(20);
		StartCoroutine(PromoteHealth(35));
	}

	private IEnumerator PromoteHealth(int healthValue)
	{
		Transform healthT = unitView.HealthLabel.transform;
		tk2dTextMesh healthTxt = unitView.HealthLabel.GetComponent<tk2dTextMesh>();
		glowAnim.transform.position = healthT.position;
		float length = animLength;
		float time = length;
		bool healthSet = false;
		glowAnim.gameObject.SetActive(true);
		glowAnim.loop = false;
		while (time > 0f)
		{
			healthT.localScale = Vector3.one + 0.65f * Vector3.one * growthCurve.Evaluate(1f - time / length);
			glowAnim.transform.localScale = glowSize * Vector3.one * growthCurve.Evaluate(1f - time / length);
			time -= Time.deltaTime;
			if (time < 0.5f && !healthSet)
			{
				healthSet = true;
				unitView.SetUnitHP(healthValue);
			}
			yield return new WaitForEndOfFrame();
		}
		cameraShake.Shake();
		glowAnim.gameObject.SetActive(false);
	}

	[ContextMenu("Test DieFace")]
	public void TestDieFaceAnim()
	{
		unitView.SetDieFaces(new DieFaceType[5]
		{
			DieFaceType.DirectDamage,
			DieFaceType.Initiative,
			DieFaceType.DirectDamage,
			DieFaceType.DirectDamage,
			DieFaceType.DirectDamage
		}, new int[5] { 3, 3, 3, 3, 3 }, null);
		StartCoroutine(PromoteDieFace(1, new DieFaceType[5]
		{
			DieFaceType.DirectDamage,
			DieFaceType.Initiative,
			DieFaceType.DirectDamage,
			DieFaceType.DirectDamage,
			DieFaceType.DirectDamage
		}, new int[5] { 10, 10, 10, 10, 10 }));
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
		switch (dieFaceTypes[dieFaceIndex])
		{
		case DieFaceType.DirectDamage:
		case DieFaceType.ArmourPiercing:
		case DieFaceType.AcidStrike:
			AudioTrigger.HighDamageResult.Play();
			break;
		case DieFaceType.Initiative:
			AudioTrigger.HighFirstStrikeResult.Play();
			break;
		case DieFaceType.Special:
			AudioTrigger.SpecialResult.Play();
			break;
		}
		Transform dieFace = unitView.DieFaces[dieFaceIndex].transform;
		tk2dTextMesh dieFaceTxt = unitView.DieValues[dieFaceIndex];
		glowAnim.transform.position = dieFace.position;
		float length = animLength;
		float time = length;
		bool dieSet = false;
		glowAnim.gameObject.SetActive(true);
		glowAnim.loop = false;
		while (time > 0f)
		{
			dieFace.localScale = Vector3.one + 0.65f * Vector3.one * growthCurve.Evaluate(1f - time / length);
			glowAnim.transform.localScale = glowSize * Vector3.one * growthCurve.Evaluate(1f - time / length);
			time -= Time.deltaTime;
			if (time < length * 0.5f && !dieSet)
			{
				dieSet = true;
				dieFaceTxt.text = dieFaceValues[dieFaceIndex].ToString();
				unitView.SetDieFace(dieFaceIndex, dieFaceTypes, dieFaceValues, boosted);
			}
			yield return new WaitForEndOfFrame();
		}
		dieFace.localScale = Vector3.one;
		cameraShake.Shake();
		glowAnim.gameObject.SetActive(false);
	}
}
