using System.Collections.Generic;
using UnityEngine;

public class BlueprintsInspectLockedTankPopUpController : PopupController
{
	[SerializeField]
	private UnitInfoView unitInfoView;

	[SerializeField]
	private PriceLabelController priceLabelController;

	[SerializeField]
	private tk2dTextMesh popUpMessage;

	[SerializeField]
	private UpgradePartRequirementController[] partRequirementControllers;

	protected override void Start()
	{
		base.Start();
		UnitDataModel unitDataModel = (UnitDataModel)model.payload;
		if (unitDataModel != null)
		{
			if ((bool)unitInfoView)
			{
				unitInfoView.ConfigureUnitView(unitDataModel, 1);
			}
			SetupPriceLabel(unitDataModel.GetBuildPrice());
			if ((bool)popUpMessage)
			{
				popUpMessage.color = Color.red;
				popUpMessage.text = "ui_buildpopup_highertier_required".Localize("Higher tier required to build this tank");
			}
		}
	}

	private void SetupPriceLabel(UserPriceDataModel price)
	{
		int num = 0;
		for (int i = 0; i < partRequirementControllers.Length; i++)
		{
			if (i < price.items.Count)
			{
				partRequirementControllers[i].gameObject.SetActive(true);
				if (price.items[i].Part != null)
				{
					List<ItemCollectionDataModel.Item> list = new List<ItemCollectionDataModel.Item>();
					list.Add(price.items[i]);
					UserPriceDataModel priceDataModel = new UserPriceDataModel(list);
					partRequirementControllers[num].ConfigureWithPrice(priceDataModel, null, string.Empty, UserProfile.player.CanAffordItem(price.items[i]), i);
					num++;
				}
			}
			else
			{
				partRequirementControllers[i].gameObject.SetActive(false);
			}
			partRequirementControllers[i].GetComponent<tk2dUIItem>().enabled = false;
		}
	}

	private void GetPartsButtonClicked()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
		ClosePopUpButton();
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
