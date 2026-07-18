using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitPartialUpgradeView : MonoBehaviour
{
	protected const string DIE_FACE_PREFIX = "DieFace_";

	protected const string INCREASE_FORMAT = "+{0}";

	[SerializeField]
	protected tk2dTextMesh hpLabel;

	[SerializeField]
	protected tk2dTextMesh hpIncreaseLabel;

	[SerializeField]
	protected tk2dTextMesh abilityLabel;

	[SerializeField]
	protected tk2dTextMesh abilityIncreaseLabel;

	[SerializeField]
	protected tk2dSprite[] dieFaces;

	[SerializeField]
	protected tk2dTextMesh[] dieValues;

	[SerializeField]
	protected tk2dTextMesh[] dieValueIncs;

	[SerializeField]
	protected GameObject dieFacesContainer;

	[SerializeField]
	protected GameObject hpContainer;

	[SerializeField]
	protected GameObject specialContainer;

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
	}

	public void SetDieFaces(DieFaceType[] dieFaceTypes, int[] dieFaceValues, EventUnitBoostDataModel boosted)
	{
		if (dieFaces.Length == 0 || dieValues.Length == 0)
		{
			return;
		}
		if (dieFaces != null && dieFaces.Length < dieFaceTypes.Length)
		{
			Log.Error("UnitPartialUpgradeView dieFaces/dieFaceTypes are not configured correctly", base.gameObject);
		}
		if (dieValues != null && dieValues.Length < dieFaceValues.Length)
		{
			Log.Error("UnitPartialUpgradeView dieFaces/dieFaceValues are not configured correctly", base.gameObject);
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

	public void SetUnitHP(int health)
	{
		hpLabel.text = health.ToString();
		hpLabel.color = Color.red;
	}

	public virtual void ConfigureUnitPartialUpgradeView(UnitDataModel unit, int level, int currentPartialLevelBitFlag, UnitPartialLevelDataModel previewPartialLevel)
	{
		UnitLevelProgressionDataModel level2 = unit.GetLevel(level - 1);
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		EventUnitBoostDataModel eventUnitBoostDataModel = null;
		if (activeEvent != null)
		{
			eventUnitBoostDataModel = EventUnitBoostDataModel.FindUnitBoost(level2.unitId.ToString(), level2.level, activeEvent.id);
		}
		UnitPartialLevelDataModel.PartialLevel partialLevel = UnitPartialLevelDataModel.SetupPartialLevel(unit.id, level, currentPartialLevelBitFlag);
		if (previewPartialLevel.hp > 0)
		{
			dieFacesContainer.SetActive(false);
			hpContainer.SetActive(true);
			specialContainer.SetActive(false);
			if (eventUnitBoostDataModel != null)
			{
				SetUnitHP(level2.hp + eventUnitBoostDataModel.hpBoost + partialLevel.health);
			}
			else
			{
				SetUnitHP(level2.hp + partialLevel.health);
			}
			hpIncreaseLabel.text = string.Format("+{0}", previewPartialLevel.hp);
			return;
		}
		if (previewPartialLevel.specialBoostValueA > 0)
		{
			dieFacesContainer.SetActive(false);
			hpContainer.SetActive(false);
			specialContainer.SetActive(true);
			SetUnitAbilityDescription(unit, level - 1, eventUnitBoostDataModel, partialLevel);
			abilityIncreaseLabel.text = string.Format("+{0}", previewPartialLevel.hp);
			return;
		}
		dieFacesContainer.SetActive(true);
		hpContainer.SetActive(false);
		specialContainer.SetActive(false);
		int[] array = new int[level2.RollTypes.Length];
		for (int i = 0; i < level2.RollTypes.Length; i++)
		{
			array[i] = level2.RollValues[i] + partialLevel.diceValues[i];
			if (eventUnitBoostDataModel != null)
			{
				if (level2.RollTypes[i] == DieFaceType.Initiative)
				{
					array[i] += eventUnitBoostDataModel.dieBoostInitiative;
				}
				if (level2.RollTypes[i] == DieFaceType.DirectDamage)
				{
					array[i] += eventUnitBoostDataModel.dieBoostDamage;
				}
				if (level2.RollTypes[i] == DieFaceType.ArmourPiercing)
				{
					array[i] += eventUnitBoostDataModel.dieBoostArmourPiercing;
				}
				if (level2.RollTypes[i] == DieFaceType.AcidStrike)
				{
					array[i] += eventUnitBoostDataModel.dieBoostAcidStrike;
				}
			}
		}
		DieFaceType[] array2 = new DieFaceType[level2.RollTypes.Length];
		for (int j = 0; j < level2.RollTypes.Length; j++)
		{
			array2[j] = level2.RollTypes[j];
			if (partialLevel.diceTypes[j] != DieFaceType.None)
			{
				array2[j] = partialLevel.diceTypes[j];
			}
		}
		SetDieFaces(array2, array, eventUnitBoostDataModel);
		dieValueIncs[0].text = ((previewPartialLevel.face1Value <= 0) ? string.Empty : string.Format("+{0}", previewPartialLevel.face1Value));
		dieValueIncs[1].text = ((previewPartialLevel.face2Value <= 0) ? string.Empty : string.Format("+{0}", previewPartialLevel.face2Value));
		dieValueIncs[2].text = ((previewPartialLevel.face3Value <= 0) ? string.Empty : string.Format("+{0}", previewPartialLevel.face3Value));
		dieValueIncs[3].text = ((previewPartialLevel.face4Value <= 0) ? string.Empty : string.Format("+{0}", previewPartialLevel.face4Value));
		dieValueIncs[4].text = ((previewPartialLevel.face5Value <= 0) ? string.Empty : string.Format("+{0}", previewPartialLevel.face5Value));
	}
}
