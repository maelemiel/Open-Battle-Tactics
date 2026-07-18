using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfoView : MonoBehaviour
{
	public struct DescTitleHolder
	{
		public List<string> titles;

		public List<string> descs;
	}

	protected const string MAX_LEVEL_STRING = "Max level";

	protected const string DIE_FACE_PREFIX = "DieFace_";

	protected const string SPECIAL_TEXT = "Special: ";

	protected const string HIDDEN_VALUE_CHARACTER = "?";

	private const string UI_ATTACH_POINT_NAME = "dust";

	private readonly Vector3 initialUIPosition = new Vector3(-65f, -81f, 0f);

	[SerializeField]
	protected tk2dTextMesh unitNameLabel;

	[SerializeField]
	protected tk2dTextMesh levelLabel;

	[SerializeField]
	protected tk2dTextMesh unitRarityLabel;

	[SerializeField]
	protected tk2dTextMesh hpLabel;

	[SerializeField]
	protected tk2dTextMesh abilityLabel;

	[SerializeField]
	protected tk2dTextMesh abilityDescriptionLabel;

	[SerializeField]
	protected tk2dSprite[] dieFaces;

	[SerializeField]
	protected tk2dTextMesh[] dieValues;

	public UnitProxy unitProxy;

	[SerializeField]
	protected ChartBarView rarityStarsView;

	[SerializeField]
	protected UnitIcon unitIcon;

	[SerializeField]
	private tk2dSprite upgradeUnitIcon;

	[SerializeField]
	private tk2dTextMesh upgradeUnitText;

	[SerializeField]
	private GameObject upgradeButton;

	[SerializeField]
	private GameObject parentUIGameObject;

	[SerializeField]
	private GameObject arrowsEffect;

	[SerializeField]
	private tk2dSpineAnimation cooldownEffect;

	public bool showTextOnMaxLevel;

	public UnitDataModel stored;

	public tk2dSprite[] DieFaces
	{
		get
		{
			return dieFaces;
		}
	}

	public tk2dTextMesh[] DieValues
	{
		get
		{
			return dieValues;
		}
	}

	public GameObject HealthLabel
	{
		get
		{
			if ((bool)hpLabel)
			{
				return hpLabel.gameObject;
			}
			return null;
		}
	}

	public GameObject UnitGameObject
	{
		get
		{
			if ((bool)unitProxy)
			{
				return unitProxy.gameObject;
			}
			return null;
		}
	}

	public virtual void SetUnitName(string unitName)
	{
		if ((bool)unitNameLabel && !string.IsNullOrEmpty(unitName))
		{
			unitNameLabel.text = unitName;
		}
	}

	public virtual void SetUnitRarityLabel(string unitRarityName)
	{
		if ((bool)unitRarityLabel)
		{
			unitRarityLabel.text = unitRarityName;
		}
	}

	public void SetUnitAbilityDescription(UnitDataModel unit, int level, EventUnitBoostDataModel boost, UnitPartialLevelDataModel.PartialLevel partialLevel)
	{
		UnitLevelProgressionDataModel level2 = unit.GetLevel(level);
		List<UnitSpecialDataModel> list = new List<UnitSpecialDataModel>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		UnitSpecialDataModel unitSpecialDataModel = null;
		UnitSpecialDataModel unitSpecialDataModel2 = null;
		unitSpecialDataModel = UnitSpecialDataModel.GetSingle((partialLevel.special1 != 0) ? partialLevel.special1 : level2.specialId);
		if (unitSpecialDataModel != null && unitSpecialDataModel.Type != null)
		{
			list.Add(unitSpecialDataModel);
			list2.Add(level2.specialBoostValueA + partialLevel.special1BoostA);
			list3.Add(level2.specialBoostValueB + partialLevel.special1BoostB);
		}
		unitSpecialDataModel = UnitSpecialDataModel.GetSingle((partialLevel.special2 != 0) ? partialLevel.special2 : level2.passiveId);
		if (unitSpecialDataModel != null && unitSpecialDataModel.Type != null)
		{
			list.Add(unitSpecialDataModel);
			list2.Add(level2.passiveBoostValueA + partialLevel.special2BoostA);
			list3.Add(level2.passiveBoostValueB + partialLevel.special2BoostB);
		}
		if (boost != null)
		{
			if (boost.ability1Override != 0)
			{
				if (list.Count > 0)
				{
					list[0] = UnitSpecialDataModel.GetSingle(boost.ability1Override);
					List<int> list5;
					List<int> list4 = (list5 = list2);
					int index2;
					int index = (index2 = 0);
					index2 = list5[index2];
					list4[index] = index2 + boost.ability1BoostA;
					List<int> list7;
					List<int> list6 = (list7 = list3);
					int index3 = (index2 = 0);
					index2 = list7[index2];
					list6[index3] = index2 + boost.ability1BoostB;
				}
				else
				{
					list.Add(UnitSpecialDataModel.GetSingle(boost.ability1Override));
					list2.Add(boost.ability1BoostA);
					list3.Add(boost.ability1BoostB);
				}
			}
			if (boost.ability2Override != 0)
			{
				unitSpecialDataModel2 = UnitSpecialDataModel.GetSingle(boost.ability2Override);
				if (list.Count > 1)
				{
					list[1] = UnitSpecialDataModel.GetSingle(boost.ability2Override);
					List<int> list9;
					List<int> list8 = (list9 = list2);
					int index2;
					int index4 = (index2 = 1);
					index2 = list9[index2];
					list8[index4] = index2 + boost.ability2BoostA;
					List<int> list11;
					List<int> list10 = (list11 = list3);
					int index5 = (index2 = 1);
					index2 = list11[index2];
					list10[index5] = index2 + boost.ability2BoostB;
				}
				else
				{
					list.Add(UnitSpecialDataModel.GetSingle(boost.ability2Override));
					list2.Add(boost.ability2BoostA);
					list3.Add(boost.ability2BoostB);
				}
			}
			bool flag = Array.Exists(list.ToArray(), (UnitSpecialDataModel item) => item.ID == "60901");
			if (boost.bonusPointsBoost > 0 && !flag)
			{
				list.Add(UnitSpecialDataModel.GetSingle("60901"));
				list2.Add(boost.bonusPointsBoost);
				list3.Add(0);
			}
		}
		StopCoroutine("TransitionBetweenAbilities");
		string empty = string.Empty;
		string empty2 = string.Empty;
		List<string> list12 = new List<string>();
		List<string> list13 = new List<string>();
		for (int num = 0; num < list.Count; num++)
		{
			list12.Add(UnitSpecialDataModel.GetName(list[num], list2[num], list3[num]));
			list13.Add(UnitSpecialDataModel.GetDescription(list[num], list2[num], list3[num]));
		}
		if ((bool)abilityLabel)
		{
			abilityLabel.text = string.Empty;
		}
		if ((bool)abilityDescriptionLabel)
		{
			abilityDescriptionLabel.text = string.Empty;
		}
		if (list12.Count > 1)
		{
			StartCoroutine("TransitionBetweenAbilities", new DescTitleHolder
			{
				titles = list12,
				descs = list13
			});
		}
		else if (list12.Count == 1)
		{
			empty2 = list12[0];
			empty = list13[0];
			if ((bool)abilityLabel)
			{
				abilityLabel.text = empty2;
			}
			if ((bool)abilityDescriptionLabel)
			{
				abilityDescriptionLabel.text = empty;
			}
		}
	}

	private IEnumerator TransitionBetweenAbilities(DescTitleHolder holder)
	{
		int counter = 0;
		while (true)
		{
			if ((bool)abilityLabel)
			{
				abilityLabel.text = holder.titles[counter];
			}
			if ((bool)abilityDescriptionLabel)
			{
				abilityDescriptionLabel.text = holder.descs[counter];
			}
			SimpleTween.Start(0f, 1f, 1f, delegate(float current)
			{
				if ((bool)abilityLabel)
				{
					abilityLabel.Alpha = current;
				}
				if ((bool)abilityDescriptionLabel)
				{
					abilityDescriptionLabel.Alpha = current;
				}
			});
			yield return new WaitForSeconds(4f);
			SimpleTween.Start(1f, 0f, 1.5f, delegate(float current)
			{
				if ((bool)abilityLabel)
				{
					abilityLabel.Alpha = current;
				}
				if ((bool)abilityDescriptionLabel)
				{
					abilityDescriptionLabel.Alpha = current;
				}
			});
			yield return new WaitForSeconds(1f);
			counter = (counter + 1) % holder.descs.Count;
			if ((bool)abilityLabel)
			{
				abilityLabel.text = holder.titles[counter];
			}
			if ((bool)abilityDescriptionLabel)
			{
				abilityDescriptionLabel.text = holder.descs[counter];
			}
		}
	}

	public virtual IEnumerator SetAssetBundle(int assetBundleID)
	{
		if ((bool)unitProxy)
		{
			unitProxy.gameObject.SetActive(true);
			yield return StartCoroutine(unitProxy.ChangeAssetCoroutine("Prefab.prefab", assetBundleID));
		}
	}

	public virtual void SetUnitLevel(int level, bool isMaxLevel = false)
	{
		if ((bool)levelLabel)
		{
			if (isMaxLevel && showTextOnMaxLevel)
			{
				levelLabel.text = "ui_unitinfo_maxlevel".Localize("Max level");
			}
			else
			{
				levelLabel.text = string.Format("ui_unitinfo_level".Localize("Lvl {0}"), level);
			}
		}
	}

	public void SetUnitHP(int hp)
	{
		if ((bool)hpLabel)
		{
			hpLabel.text = hp.ToString();
		}
	}

	public void SetDieFaces(DieFaceType[] dieFaceTypes, int[] dieFaceValues, EventUnitBoostDataModel boosted)
	{
		if (dieFaces.Length == 0 || dieValues.Length == 0)
		{
			return;
		}
		if (dieFaces != null && dieFaces.Length < dieFaceTypes.Length)
		{
			Log.Error("UnitView dieFaces/dieFaceTypes are not configured correctly", base.gameObject);
		}
		if (dieValues != null && dieValues.Length < dieFaceValues.Length)
		{
			Log.Error("UnitView dieFaces/dieFaceValues are not configured correctly", base.gameObject);
		}
		for (int i = 0; i < dieFaceTypes.Length; i++)
		{
			if (boosted != null)
			{
				if (dieFaceTypes[i] == DieFaceType.DirectDamage && boosted.dieBoostDamage > 0)
				{
					dieFaces[i].SetSprite("DieFace_" + (int)dieFaceTypes[i] + "G");
				}
				else if (dieFaceTypes[i] == DieFaceType.Initiative && boosted.dieBoostInitiative > 0)
				{
					dieFaces[i].SetSprite("DieFace_" + (int)dieFaceTypes[i] + "G");
				}
				else if (dieFaceTypes[i] == DieFaceType.ArmourPiercing && boosted.dieBoostArmourPiercing > 0)
				{
					dieFaces[i].SetSprite("DieFace_" + (int)dieFaceTypes[i] + "G");
				}
				else if (dieFaceTypes[i] == DieFaceType.AcidStrike && boosted.dieBoostAcidStrike > 0)
				{
					dieFaces[i].SetSprite("DieFace_" + (int)dieFaceTypes[i] + "G");
				}
				else if (dieFaceTypes[i] == DieFaceType.Special && (boosted.ability1BoostA > 0 || boosted.ability1BoostB > 0))
				{
					dieFaces[i].SetSprite("DieFace_" + (int)dieFaceTypes[i] + "G");
				}
				else
				{
					dieFaces[i].SetSprite("DieFace_" + (int)dieFaceTypes[i]);
				}
			}
			else
			{
				dieFaces[i].SetSprite("DieFace_" + (int)dieFaceTypes[i]);
			}
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

	public void HideDataValues()
	{
		string text = "0";
		for (int i = 0; i < dieValues.Length; i++)
		{
			text = dieValues[i].text;
			if (!string.IsNullOrEmpty(text) && text != "0")
			{
				dieValues[i].text = "?";
			}
			else
			{
				dieValues[i].text = string.Empty;
			}
		}
		if ((bool)hpLabel)
		{
			hpLabel.text = "??";
		}
	}

	public void SetDieFace(int index, DieFaceType[] dieFaceTypes, int[] dieFaceValues, bool boosted)
	{
		if (dieFaces.Length > index && dieValues.Length > index)
		{
			dieFaces[index].SetSprite("DieFace_" + (int)dieFaceTypes[index] + ((!boosted) ? string.Empty : "G"));
			int num = dieFaceValues[index];
			if (num > 0)
			{
				dieValues[index].text = num.ToString();
			}
			else
			{
				dieValues[index].text = string.Empty;
			}
		}
	}

	public void SetRarity(int rarity)
	{
		if ((bool)rarityStarsView)
		{
			rarityStarsView.SetBarLevel(rarity);
		}
	}

	public void SetUpgradeState(bool state)
	{
		if ((bool)upgradeUnitIcon)
		{
			upgradeUnitIcon.gameObject.SetActive(state);
		}
		if ((bool)upgradeUnitText)
		{
			upgradeUnitText.gameObject.SetActive(state);
		}
		if ((bool)arrowsEffect)
		{
			arrowsEffect.SetActive(state);
		}
		if ((bool)upgradeButton)
		{
			upgradeButton.gameObject.SetActive(state);
		}
	}

	private IEnumerator ChangeUpgradeAlphaValue()
	{
		while (true)
		{
			float currentAlpha = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f;
			if ((bool)upgradeUnitIcon)
			{
				upgradeUnitIcon.Alpha = currentAlpha;
			}
			if ((bool)upgradeUnitText)
			{
				upgradeUnitText.Alpha = currentAlpha;
			}
			yield return null;
		}
	}

	private void SetUnitCooldownEffect(bool state)
	{
		if ((bool)cooldownEffect)
		{
			cooldownEffect.gameObject.SetActive(state);
		}
	}

	public void RepositionUI()
	{
		if ((bool)parentUIGameObject)
		{
			StartCoroutine(InternalRepositionUI());
		}
	}

	private IEnumerator InternalRepositionUI()
	{
		while (!unitProxy.AssetReady)
		{
			yield return null;
		}
		tk2dSprite tankSprite = unitProxy.Prefab.GetComponentInChildren<tk2dSprite>();
		if ((bool)tankSprite)
		{
			tk2dSpriteDefinition.AttachPoint attachPoint = tankSprite.GetAttachPointByName("dust");
			if (attachPoint != null)
			{
				parentUIGameObject.transform.transform.localPosition = attachPoint.position;
			}
		}
	}

	public void RestoreUIPosition()
	{
		if ((bool)parentUIGameObject)
		{
			parentUIGameObject.transform.localPosition = initialUIPosition;
		}
	}

	public virtual void ConfigureUnitView(UnitDataModel unit, int level = -1, int partialLevelBitFlag = 0, bool onCooldown = false)
	{
		bool flag = false;
		int level2 = level;
		if (unit.IsLevelSkin(level))
		{
			flag = true;
			level2 = unit.MaxLevel;
		}
		else if (level == -1 || unit.IsMaxLevel(level))
		{
			flag = true;
			level = unit.MaxLevel;
			level2 = level;
		}
		else if (level == 0)
		{
			level = 1;
			level2 = level;
			Log.Warning("Unit levels should be 1-based", base.gameObject);
		}
		UnitLevelProgressionDataModel level3 = unit.GetLevel(level - 1);
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		EventUnitBoostDataModel eventUnitBoostDataModel = null;
		if (activeEvent != null)
		{
			eventUnitBoostDataModel = EventUnitBoostDataModel.FindUnitBoost(level3.unitId.ToString(), level3.level, activeEvent.id);
		}
		UnitPartialLevelDataModel.PartialLevel partialLevel = UnitPartialLevelDataModel.SetupPartialLevel(unit.id, level, partialLevelBitFlag);
		SetUnitName(unit.name);
		SetUnitLevel(level2, flag);
		SetUnitRarityLabel(level3.RarityModel.Name);
		SetRarity(level3.Rarity - 1);
		SetUnitAbilityDescription(unit, level - 1, eventUnitBoostDataModel, partialLevel);
		SetUnitCooldownEffect(onCooldown);
		if ((bool)unitIcon)
		{
			unitIcon.SetUnitIcon(unit.UnitType);
		}
		if (eventUnitBoostDataModel != null)
		{
			SetUnitHP(level3.hp + eventUnitBoostDataModel.hpBoost + partialLevel.health);
		}
		else
		{
			SetUnitHP(level3.hp + partialLevel.health);
		}
		if (!flag)
		{
			UnitLevelUpRequirementDataModel single = UnitLevelUpRequirementDataModel.GetSingle(level3.levelUpRequirementId);
			SetUpgradeState(UserProfile.player.CanAfford(ItemPriceDataModel.GetPriceForID(single.priceId)));
		}
		else
		{
			SetUpgradeState(false);
		}
		int[] array = new int[level3.RollTypes.Length];
		for (int i = 0; i < level3.RollTypes.Length; i++)
		{
			array[i] = level3.RollValues[i] + partialLevel.diceValues[i];
			if (eventUnitBoostDataModel != null)
			{
				if (level3.RollTypes[i] == DieFaceType.Initiative)
				{
					array[i] += eventUnitBoostDataModel.dieBoostInitiative;
				}
				if (level3.RollTypes[i] == DieFaceType.DirectDamage)
				{
					array[i] += eventUnitBoostDataModel.dieBoostDamage;
				}
				if (level3.RollTypes[i] == DieFaceType.ArmourPiercing)
				{
					array[i] += eventUnitBoostDataModel.dieBoostArmourPiercing;
				}
				if (level3.RollTypes[i] == DieFaceType.AcidStrike)
				{
					array[i] += eventUnitBoostDataModel.dieBoostAcidStrike;
				}
			}
		}
		DieFaceType[] array2 = new DieFaceType[level3.RollTypes.Length];
		for (int j = 0; j < level3.RollTypes.Length; j++)
		{
			array2[j] = level3.RollTypes[j];
			if (partialLevel.diceTypes[j] != DieFaceType.None)
			{
				array2[j] = partialLevel.diceTypes[j];
			}
		}
		SetDieFaces(array2, array, eventUnitBoostDataModel);
		unitProxy.gameObject.SetActive(true);
		StartCoroutine(SetAssetBundle(level3.assetBundleId));
	}

	public void SetState(bool state)
	{
		if (!state)
		{
			unitProxy.ResetProxy();
		}
		base.gameObject.SetActive(state);
	}
}
