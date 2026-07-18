using System.Collections;
using UnityEngine;

public class GachaUnitView : MonoBehaviour
{
	private const int SORTING_ORDER_TANK_SPRITE = 11;

	private const string DIE_FACE_PREFIX = "DieFace_";

	[SerializeField]
	private UnitProxy unitProxy;

	[HideInInspector]
	public GachaSceneController controller;

	[SerializeField]
	private tk2dTextMesh unitNameLabel;

	[SerializeField]
	private tk2dTextMesh unitAbilityType;

	[SerializeField]
	private tk2dSprite[] dieFaces;

	[SerializeField]
	private tk2dTextMesh[] dieValues;

	[SerializeField]
	public tk2dSpineAnimation scrapAnimation;

	[SerializeField]
	private ChartBarView rarityStarsView;

	[SerializeField]
	private tk2dTextMesh hpLabel;

	[SerializeField]
	private tk2dSprite attackIcon;

	[SerializeField]
	private UnitIcon unitIcon;

	private tk2dSprite tankSprite;

	public void SetScrapMode()
	{
		if ((bool)scrapAnimation)
		{
			scrapAnimation.gameObject.SetActive(true);
			AutodestroySpineAnimation.Autodestroy(scrapAnimation);
		}
		if ((bool)tankSprite)
		{
			tankSprite.gameObject.SetActive(false);
		}
		if ((bool)attackIcon)
		{
			attackIcon.gameObject.SetActive(false);
		}
		if ((bool)hpLabel)
		{
			hpLabel.transform.parent.gameObject.SetActive(false);
		}
		for (int i = 0; i < dieFaces.Length; i++)
		{
			dieFaces[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < dieValues.Length; j++)
		{
			dieValues[j].gameObject.SetActive(false);
		}
		unitAbilityType.text = string.Empty;
		unitNameLabel.transform.position = unitAbilityType.transform.position;
		if ((bool)rarityStarsView)
		{
			rarityStarsView.gameObject.SetActive(false);
		}
		unitNameLabel.text = "Scrap!";
	}

	public void UpdateGachaUnitInfo(UnitDataModel unit, int level = 0)
	{
		unitNameLabel.text = unit.name;
		UnitSpecialDataModel single = UnitSpecialDataModel.GetSingle(unit.Levels[level].specialId);
		if (single != null)
		{
			if (single.Type == null)
			{
				unitAbilityType.text = string.Empty;
			}
			else if (single != null)
			{
				unitAbilityType.text = UnitSpecialDataModel.GetName(single, unit.Levels[level].specialBoostValueA, unit.Levels[level].specialBoostValueB);
			}
		}
		else
		{
			Debug.LogWarning("The unit has not metadata. This should not ever happen");
			unitAbilityType.text = string.Empty;
		}
		UnitLevelProgressionDataModel unitLevelProgressionDataModel = unit.Levels[level];
		SetDieFaces(unitLevelProgressionDataModel.RollTypes, unitLevelProgressionDataModel.RollValues);
		SetUnitRarity(int.Parse(unitLevelProgressionDataModel.RarityModel.id) - 1);
		SetUnitHealth(unit.Levels[level].hp);
		if ((bool)unitIcon)
		{
			unitIcon.SetUnitIcon(unit.UnitType);
		}
		StartCoroutine(SetAssetBundle(unit.Levels[level].assetBundleId));
	}

	private IEnumerator SetAssetBundle(int assetBundleID)
	{
		if ((bool)unitProxy)
		{
			yield return StartCoroutine(unitProxy.ChangeAssetCoroutine("Prefab.prefab", assetBundleID));
		}
		tankSprite = unitProxy.GetComponentInChildren<tk2dSprite>();
		if ((bool)tankSprite)
		{
			tankSprite.SortingOrder = 11;
			tankSprite.gameObject.layer = unitProxy.gameObject.layer;
		}
	}

	private void SetDieFaces(DieFaceType[] dieFaceTypes, int[] dieFaceValues)
	{
		if (dieFaces != null && dieFaces.Length < dieFaceTypes.Length)
		{
			Log.Error("UnitView dieFaces/dieFaceTypes are not configured correctly", base.gameObject);
		}
		if (dieFaces != null && dieFaces.Length < dieFaceValues.Length)
		{
			Log.Error("UnitView dieFaces/dieFaceValues are not configured correctly", base.gameObject);
		}
		for (int i = 0; i < dieFaceTypes.Length; i++)
		{
			dieFaces[i].SetSprite("DieFace_" + (int)dieFaceTypes[i]);
		}
		int num = 0;
		for (int j = 0; j < dieFaceValues.Length; j++)
		{
			num = dieFaceValues[j];
			if (num > 0)
			{
				dieValues[j].text = num.ToString();
			}
			else
			{
				dieValues[j].text = string.Empty;
			}
		}
	}

	private void SetUnitRarity(int rarity)
	{
		if ((bool)rarityStarsView)
		{
			rarityStarsView.SetBarLevel(rarity);
		}
	}

	private void SetUnitHealth(int health)
	{
		if ((bool)hpLabel)
		{
			hpLabel.text = health.ToString();
		}
	}
}
