using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUnitPartsController : MonoBehaviour, IUpgradeUnitPartsContoller
{
	private const string PROMOTE_BUTTON_TEXT = "Promote to level {0}!";

	[SerializeField]
	private Transform unitAnchor;

	[SerializeField]
	private Transform unitWithAbilityAnchor;

	[SerializeField]
	private UpgradePartRequirementController[] partRequirementControllers;

	[SerializeField]
	private tk2dUIItem promoteButton;

	[SerializeField]
	private tk2dTextMesh promoteButtonText;

	[SerializeField]
	private tk2dBaseSprite promoteButtonSprite;

	[SerializeField]
	private tk2dBaseSprite disablePromoteButtonSprite;

	[SerializeField]
	private PriceLabelController promotePriceLabelController;

	[SerializeField]
	private PromoteUnitController promotionController;

	[SerializeField]
	private tk2dUIItem maxLeveledButton;

	private UserUnit localUserUnit;

	private bool promotingUnit;

	private Action<UnitDataModel, int, int> unitViewChange;

	private Action<bool> scrapButtonUpdate;

	private Action closeScreen;

	private Action upgradeStart;

	public bool forceLoop = true;

	public void SetupScreen(UserUnit localUserUnit, Action upgradeStart, Action closeScreen, Action<bool> scrapButtonUpdate, Action<UnitDataModel, int, int> unitViewChange)
	{
		this.upgradeStart = upgradeStart;
		this.closeScreen = closeScreen;
		this.localUserUnit = localUserUnit;
		this.unitViewChange = unitViewChange;
		this.scrapButtonUpdate = scrapButtonUpdate;
	}

	public void ToggleScreen(bool toggle)
	{
		if (toggle)
		{
			UpdatePriceLabel();
			base.gameObject.SetActive(true);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void UpdatePriceLabel()
	{
		if (!promotePriceLabelController)
		{
			return;
		}
		List<UnitPartialLevelDataModel> partialLevelsForUnit = UnitPartialLevelDataModel.GetPartialLevelsForUnit(localUserUnit.metadataId, localUserUnit.level);
		int num = 0;
		bool flag = true;
		for (int i = 0; i < partRequirementControllers.Length; i++)
		{
			if (i < partialLevelsForUnit.Count)
			{
				partRequirementControllers[i].gameObject.SetActive(true);
				UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(partialLevelsForUnit[i].requirementPriceId);
				if (priceForID.items[0].Part != null)
				{
					bool flag2 = (localUserUnit.partialLevel >> i + 1) % 2 == 1;
					flag = flag && flag2;
					partRequirementControllers[num].ConfigureWithPrice(priceForID, partialLevelsForUnit[i], localUserUnit.ID, flag2, i);
					partRequirementControllers[num].OnPartsCommitted = OnPartsCommitted;
					num++;
				}
				else
				{
					promotePriceLabelController.ConfigurePriceLabel(priceForID);
				}
			}
			else
			{
				partRequirementControllers[i].gameObject.SetActive(false);
				partRequirementControllers[i].GetComponent<tk2dUIItem>().enabled = false;
			}
		}
		SetPromoteButtonStateInternal(flag);
		if (!Constants.ShowUpgradeCost)
		{
			return;
		}
		if (flag)
		{
			UserPriceDataModel upgradePrice = localUserUnit.GetUpgradePrice();
			if (upgradePrice.items.Count > 0)
			{
				promotePriceLabelController.ConfigurePriceLabel(upgradePrice);
				promotePriceLabelController.gameObject.SetActive(true);
				promoteButtonText.gameObject.SetActive(false);
			}
		}
		else
		{
			promotePriceLabelController.gameObject.SetActive(false);
			promoteButtonText.gameObject.SetActive(true);
		}
	}

	private void OnPartsCommitted(int index)
	{
		ItemCollectionDataModel.Item item = partRequirementControllers[index].priceData.items[0];
		PopupManager.ShowPopup(PopupDataModel.UpgradeUnitPartDetailsPopUp(item.Part, item.amount, index, partRequirementControllers[index].partialData, localUserUnit.id, localUserUnit.partialLevel, null, OnPartsCommittedComplete));
	}

	public void OnPartsCommittedComplete(object input)
	{
		UpgradeUnitPartDetailPopUp.PayloadData data = (UpgradeUnitPartDetailPopUp.PayloadData)((PopupDataModel)input).payload;
		partRequirementControllers[data.index].SetCommitted(delegate
		{
			UnitPartialLevelDataModel.PartialLevel levelIncreased = UnitPartialLevelDataModel.SetupPartialLevel(localUserUnit.metadataId, localUserUnit.level, 1 << data.index + 1);
			UpdatePriceLabel();
			SetPromoteButtonStateInternal(false);
			UpdatePromotePopUpState(levelIncreased);
		});
	}

	private void UpdatePromotePopUpState(UnitPartialLevelDataModel.PartialLevel levelIncreased = null)
	{
		if (levelIncreased != null)
		{
			AudioTrigger.PlayerEquipItem.Play();
			StartCoroutine(promotionController.UnitPromotePartialLevel(localUserUnit, localUserUnit.partialLevel, levelIncreased, UpdatePromoteCallBack));
		}
		else
		{
			UpdatePromoteCallBack();
		}
	}

	private void UpdatePromoteCallBack()
	{
		if (unitViewChange != null)
		{
			unitViewChange(localUserUnit.UnitDataModel, localUserUnit.level, localUserUnit.partialLevel);
		}
		SetPartButtonsState(true);
		bool promoteButtonStateInternal = true;
		for (int i = 0; i < partRequirementControllers.Length; i++)
		{
			if (partRequirementControllers[i].GetComponent<tk2dUIItem>().enabled)
			{
				promoteButtonStateInternal = false;
				break;
			}
		}
		SetPromoteButtonStateInternal(promoteButtonStateInternal);
		promotingUnit = false;
	}

	public void SetPromoteButtonState(bool state)
	{
		if (!state)
		{
			SetPromoteButtonStateInternal(state);
			SetPartButtonsState(state);
		}
		else
		{
			SetPartButtonsState(true);
			UpdatePriceLabel();
		}
	}

	private void SetPromoteButtonStateInternal(bool state)
	{
		if ((bool)promoteButton)
		{
			promoteButton.enabled = state;
			SetPromoteButtonStateView(state);
		}
	}

	private void SetPartButtonsState(bool state)
	{
		List<UnitPartialLevelDataModel> partialLevelsForUnit = UnitPartialLevelDataModel.GetPartialLevelsForUnit(localUserUnit.metadataId, localUserUnit.level);
		for (int i = 0; i < partRequirementControllers.Length; i++)
		{
			if (i < partialLevelsForUnit.Count)
			{
				if (state)
				{
					UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(partialLevelsForUnit[i].requirementPriceId);
					bool flag = (localUserUnit.partialLevel >> i + 1) % 2 == 1;
					partRequirementControllers[i].GetComponent<tk2dUIItem>().enabled = !flag;
				}
				else
				{
					partRequirementControllers[i].GetComponent<tk2dUIItem>().enabled = state;
				}
			}
		}
	}

	private void SetPromoteButtonStateView(bool state)
	{
		if (localUserUnit.IsMaxLevel)
		{
			promoteButton.gameObject.SetActive(false);
			maxLeveledButton.gameObject.SetActive(true);
			maxLeveledButton.enabled = state;
			return;
		}
		promoteButton.gameObject.SetActive(true);
		if ((bool)disablePromoteButtonSprite)
		{
			disablePromoteButtonSprite.gameObject.SetActive(!state);
		}
	}

	private void PromoteUnitButton()
	{
		promotingUnit = true;
		SetPromoteButtonState(false);
		if (upgradeStart != null)
		{
			upgradeStart();
		}
		if (scrapButtonUpdate != null)
		{
			scrapButtonUpdate(false);
		}
		for (int i = 0; i < partRequirementControllers.Length; i++)
		{
			partRequirementControllers[i].GetComponent<tk2dUIItem>().enabled = false;
		}
		promotionController.PromoteUnit(localUserUnit, UnitPromoted, UnitPromotedCancelled);
	}

	private void UnitPromoted()
	{
		if (scrapButtonUpdate != null)
		{
			scrapButtonUpdate(true);
		}
		UpdatePriceLabel();
		UpdatePromotePopUpState();
	}

	private void UnitPromotedCancelled(bool forceClose)
	{
		if (forceClose)
		{
			if (closeScreen != null)
			{
				closeScreen();
			}
			return;
		}
		if (scrapButtonUpdate != null)
		{
			scrapButtonUpdate(true);
		}
		UpdatePromotePopUpState();
	}

	public bool IsPromoting()
	{
		return promotingUnit;
	}

	public void MaxLeveledButton()
	{
		PopupManager.ShowPopup(PopupDataModel.Ok("ui_upgrade_nolevels_title".Localize("No More Levels"), "ui_upgrade_nolevels_desc".Localize("There are currently no more levels for this unit.")));
	}

	public Transform GetUnitAnchor()
	{
		UnitLevelProgressionDataModel currentLevelDataModel = localUserUnit.CurrentLevelDataModel;
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		EventUnitBoostDataModel eventUnitBoostDataModel = null;
		if (activeEvent != null)
		{
			eventUnitBoostDataModel = EventUnitBoostDataModel.FindUnitBoost(currentLevelDataModel.unitId.ToString(), currentLevelDataModel.level, activeEvent.id);
		}
		if (localUserUnit.HasSpecial || eventUnitBoostDataModel != null)
		{
			return unitWithAbilityAnchor;
		}
		return unitAnchor;
	}

	public IEnumerator AnimateControllerIn()
	{
		bool animating = true;
		promoteButtonText.Alpha = 0f;
		ToggleScreen(true);
		SetPromoteButtonState(false);
		int i = 0;
		for (int iMax = partRequirementControllers.Length; i < iMax; i++)
		{
			partRequirementControllers[i].gameObject.SetActive(false);
		}
		EffectInstance effect = GlobalEffectsManager.Create(EffectType.PARTS_COME_IN, GetUnitAnchor().position, base.transform).AutoDestroy();
		StartCoroutine(effect.SpineAnimation.PlayAnimCoroutine("Parts Come In"));
		effect.SpineAnimation.AnimationComplete += delegate
		{
			animating = false;
		};
		StartCoroutine(AnimateAlpha());
		while (animating)
		{
			int i2 = 0;
			for (int iMax2 = partRequirementControllers.Length; i2 < iMax2; i2++)
			{
				float storedZ = partRequirementControllers[i2].transform.position.z;
				if (effect.SpineAnimation.Skeleton.skeleton.Bones[i2 + 1].WorldX != 0f)
				{
					partRequirementControllers[i2].transform.position = new Vector3(effect.SpineAnimation.Skeleton.skeleton.Bones[i2 + 1].WorldX, effect.SpineAnimation.Skeleton.skeleton.Bones[i2 + 1].WorldY, storedZ);
					partRequirementControllers[i2].gameObject.SetActive(true);
				}
			}
			yield return new WaitForEndOfFrame();
		}
		UpdatePriceLabel();
	}

	private IEnumerator AnimateAlpha()
	{
		AnimationCurve easeInOut = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		for (float time = 1f; time > 0f; time -= Time.deltaTime)
		{
			promoteButtonText.Alpha = easeInOut.Evaluate(1f - time);
			yield return new WaitForEndOfFrame();
		}
		promoteButtonText.Alpha = 1f;
	}

	public IEnumerator AnimateControllerOut()
	{
		yield break;
	}
}
