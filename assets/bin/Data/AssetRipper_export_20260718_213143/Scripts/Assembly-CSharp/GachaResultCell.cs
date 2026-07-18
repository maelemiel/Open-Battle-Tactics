using UnityEngine;

public class GachaResultCell : ScrollableCell
{
	[SerializeField]
	private GachaUnitView gachaUnitView;

	[SerializeField]
	private UnitAnimationController unitAnimationController;

	[SerializeField]
	private PriceLabelController priceLabelController;

	[SerializeField]
	private tk2dTextMesh cellLabel;

	public override void ConfigureCellData()
	{
		if (dataObject != null)
		{
			GachaResultItem gachaResultItem = (GachaResultItem)dataObject;
			if (gachaResultItem != null)
			{
				ConfigureItemView(gachaResultItem.item, gachaResultItem.label);
			}
		}
	}

	private void ConfigureItemView(ItemCollectionDataModel.Item item, string label)
	{
		gachaUnitView.StopAllCoroutines();
		switch (item.itemType)
		{
		case UserInventory.ItemType.Unit:
		{
			int level = 0;
			UnitDataModel single = UnitDataModel.GetSingle(item.itemId);
			if (single == null)
			{
				UnitLevelProgressionDataModel single2 = UnitLevelProgressionDataModel.GetSingle(item.itemId);
				single = UnitDataModel.GetSingle(single2.unitId);
				level = single2.level - 1;
			}
			gachaUnitView.gameObject.SetActive(true);
			gachaUnitView.UpdateGachaUnitInfo(single, level);
			StartCoroutine(unitAnimationController.SetupAnimation());
			priceLabelController.gameObject.SetActive(false);
			break;
		}
		case UserInventory.ItemType.Energy:
		case UserInventory.ItemType.Coins:
		case UserInventory.ItemType.PremiumCurrency:
		case UserInventory.ItemType.Parts:
		case UserInventory.ItemType.EventPoint:
		case UserInventory.ItemType.RaidBossEventPoint:
			gachaUnitView.gameObject.SetActive(false);
			priceLabelController.gameObject.SetActive(true);
			priceLabelController.ConfigurePriceLabel(new ItemCollectionDataModel(item));
			break;
		}
		if ((bool)cellLabel)
		{
			cellLabel.text = label.Localize();
			cellLabel.gameObject.SetActive(!string.IsNullOrEmpty(label));
		}
	}
}
