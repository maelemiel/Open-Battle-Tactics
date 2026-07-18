using System;
using UnityEngine;

public class MaxSkinUnitCell : ScrollableCell
{
	[SerializeField]
	private UnitInfoView unitInfoView;

	[SerializeField]
	private tk2dTextMesh equipTextMesh;

	[SerializeField]
	private tk2dUIItem button;

	[SerializeField]
	private tk2dBaseSprite disabledButtonSprite;

	[SerializeField]
	private PriceLabelController priceLabelController;

	public Action<UnitLevelProgressionDataModel> PurchaseAction;

	public override void ConfigureCellData()
	{
		SetState(true);
		UnitLevelProgressionDataModel unitLevelProgressionDataModel = (UnitLevelProgressionDataModel)base.DataObject;
		if (unitLevelProgressionDataModel == null)
		{
			return;
		}
		if (unitInfoView != null)
		{
			unitInfoView.gameObject.SetActive(true);
			unitInfoView.ConfigureUnitView(unitLevelProgressionDataModel.UnitDataModel, unitLevelProgressionDataModel.level);
		}
		if (!UserProfile.player.HasUnlockedSkin(unitLevelProgressionDataModel.id) && unitLevelProgressionDataModel.level != unitLevelProgressionDataModel.UnitDataModel.MaxLevel)
		{
			equipTextMesh.text = string.Empty;
			equipTextMesh.Commit();
			if ((bool)priceLabelController)
			{
				priceLabelController.gameObject.SetActive(true);
				priceLabelController.ConfigurePriceLabel(ItemPriceDataModel.GetPriceForID(unitLevelProgressionDataModel.priceId));
			}
		}
		else
		{
			priceLabelController.gameObject.SetActive(false);
			equipTextMesh.text = "ui_unitupgrade_equip".Localize("EQUIP");
			equipTextMesh.Commit();
		}
	}

	public void SetState(bool state)
	{
		button.enabled = state;
		if ((bool)disabledButtonSprite)
		{
			disabledButtonSprite.gameObject.SetActive(!state);
		}
	}

	public void PurchaseSkin()
	{
		PurchaseAction((UnitLevelProgressionDataModel)base.DataObject);
	}

	private void OnDestroy()
	{
		PurchaseAction = null;
	}
}
