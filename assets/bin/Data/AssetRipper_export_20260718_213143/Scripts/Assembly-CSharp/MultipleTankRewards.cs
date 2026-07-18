using UnityEngine;

public class MultipleTankRewards : RewardLabelItemView
{
	public UnitInfoView mainUnitInfoView;

	public tk2dTextMesh unitNameLabel;

	public tk2dTextMesh unitAbilityLabel;

	public UnitProxy additionalTank;

	public PrefabProxy additionalPart;

	public tk2dTextMesh additionalPartAmount;

	public tk2dSprite secondaryNonProxySprite;

	public ItemDisplay[] additionalParts;

	public GameObject displayLabel;

	public GameObject displayTitle;

	public BoxCollider addtionalTankCollider;

	private ItemCollectionDataModel items;

	private ItemCollectionDataModel.Item mainItem;

	private UnitDataModel additionalTankData;

	protected override void SetupItem(ItemCollectionDataModel allItems, ItemCollectionDataModel.Item itemData)
	{
		items = allItems;
		mainItem = itemData;
		addtionalTankCollider.enabled = false;
		if ((bool)mainUnitInfoView)
		{
			if (itemData.Unit == null)
			{
				Log.Warning(string.Concat("MultipleTankRewards SetupItem Unit1 with unit of: ", itemData.Unit, " is null"));
			}
			else
			{
				if ((bool)unitNameLabel)
				{
					unitNameLabel.text = itemData.Unit.UnitDataModel.name;
				}
				if ((bool)unitAbilityLabel)
				{
					UnitSpecialDataModel single = UnitSpecialDataModel.GetSingle(itemData.Unit.specialId);
					unitAbilityLabel.text = UnitSpecialDataModel.GetName(single, itemData.Unit.specialBoostValueA, itemData.Unit.specialBoostValueB);
				}
				mainUnitInfoView.ConfigureUnitView(itemData.Unit.UnitDataModel, itemData.Unit.level);
			}
		}
		allItems.items.Remove(allItems.items[0]);
		if (allItems.items.Count > 0)
		{
			switch (allItems.items[0].itemType)
			{
			case UserInventory.ItemType.Unit:
				if ((bool)additionalTank && allItems.items[0].Unit != null)
				{
					addtionalTankCollider.enabled = true;
					additionalTankData = allItems.items[0].Unit.UnitDataModel;
					StartCoroutine(additionalTank.ChangeAssetCoroutine(allItems.items[0].Unit.assetBundleId));
					additionalPartAmount.text = string.Empty;
					secondaryNonProxySprite.gameObject.SetActive(false);
				}
				break;
			case UserInventory.ItemType.Parts:
				if ((bool)additionalPart && allItems.items[0].Part != null)
				{
					StartCoroutine(additionalPart.ChangeAssetCoroutine(allItems.items[0].Part.AssetLinkage));
					if (allItems.items[0].amount > 1)
					{
						additionalPartAmount.text = allItems.items[0].Part.Name + " x " + allItems.items[0].amount.ToString("N0");
					}
					else
					{
						additionalPartAmount.text = allItems.items[0].Part.Name;
					}
					secondaryNonProxySprite.gameObject.SetActive(false);
				}
				break;
			case UserInventory.ItemType.Energy:
			case UserInventory.ItemType.Coins:
			case UserInventory.ItemType.PremiumCurrency:
				secondaryNonProxySprite.gameObject.SetActive(true);
				secondaryNonProxySprite.SetSprite(allItems.items[0].itemType.GetIconName());
				if ((bool)additionalPartAmount)
				{
					if (allItems.items[0].amount > 1)
					{
						additionalPartAmount.text = allItems.items[0].itemType.GetLocalizedName() + " x " + allItems.items[0].amount.ToString("N0");
					}
					else
					{
						additionalPartAmount.text = allItems.items[0].itemType.GetLocalizedName();
					}
				}
				break;
			default:
				Log.Warning("MultiplePartRewards Unsupported type: " + allItems.items[0].itemType);
				break;
			}
			allItems.items.Remove(allItems.items[0]);
		}
		if (allItems.items.Count == 0)
		{
			displayLabel.SetActive(false);
			displayTitle.SetActive(false);
			return;
		}
		for (int i = 0; i < allItems.items.Count; i++)
		{
			additionalParts[i].gameObject.SetActive(true);
			additionalParts[i].SetupItem(allItems.items[i], allItems.items[i].amount);
		}
	}

	public void ShowTankDetails()
	{
		PopupManager.ShowPopup(PopupDataModel.InspectUnitPopUp(mainItem.Unit.UnitDataModel, delegate
		{
		}));
	}

	public void ShowAdditionalTankDetails()
	{
		if (additionalTankData != null)
		{
			PopupManager.ShowPopup(PopupDataModel.InspectUnitPopUp(additionalTankData, delegate
			{
			}));
		}
	}
}
