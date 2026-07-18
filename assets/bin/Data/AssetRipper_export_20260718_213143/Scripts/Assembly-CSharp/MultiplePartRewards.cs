using UnityEngine;

public class MultiplePartRewards : RewardLabelItemView
{
	public PrefabProxy mainPrefabProxy;

	public tk2dSprite mainNonProxySprite;

	public tk2dTextMesh mainNonProxyCount;

	public tk2dSprite secondaryNonProxySprite;

	public tk2dTextMesh unitNameLabel;

	public PrefabProxy mainAdditionalPart;

	public tk2dTextMesh mainAdditionalPartCount;

	public ItemDisplay[] additionalParts;

	public GameObject displayLabel;

	public GameObject displayTitle;

	protected override void SetupItem(ItemCollectionDataModel allItems, ItemCollectionDataModel.Item itemData)
	{
		switch (itemData.itemType)
		{
		case UserInventory.ItemType.Parts:
			if ((bool)mainPrefabProxy)
			{
				StartCoroutine(mainPrefabProxy.ChangeAssetCoroutine(itemData.Part.AssetLinkage));
			}
			if ((bool)unitNameLabel)
			{
				if (itemData.amount > 1)
				{
					unitNameLabel.text = itemData.Part.Name + " x " + itemData.amount.ToString("N0");
				}
				else
				{
					unitNameLabel.text = itemData.Part.Name;
				}
			}
			mainNonProxySprite.gameObject.SetActive(false);
			break;
		case UserInventory.ItemType.Energy:
		case UserInventory.ItemType.Coins:
		case UserInventory.ItemType.PremiumCurrency:
			mainNonProxySprite.gameObject.SetActive(true);
			mainNonProxySprite.SetSprite(itemData.itemType.GetIconName());
			if ((bool)unitNameLabel)
			{
				unitNameLabel.text = itemData.itemType.GetLocalizedName();
				mainNonProxyCount.text = item.amount.ToString("N0");
			}
			break;
		default:
			Log.Warning("MultiplePartRewards Unsupported type: " + itemData.itemType);
			break;
		}
		allItems.items.Remove(allItems.items[0]);
		if ((bool)mainAdditionalPart && allItems.items.Count > 0)
		{
			switch (allItems.items[0].itemType)
			{
			case UserInventory.ItemType.Parts:
				if ((bool)mainAdditionalPart)
				{
					StartCoroutine(mainAdditionalPart.ChangeAssetCoroutine(allItems.items[0].Part.AssetLinkage));
				}
				if ((bool)mainAdditionalPartCount)
				{
					if (allItems.items[0].amount > 1)
					{
						mainAdditionalPartCount.text = allItems.items[0].Part.Name + " x " + allItems.items[0].amount.ToString("N0");
					}
					else
					{
						mainAdditionalPartCount.text = allItems.items[0].Part.Name;
					}
				}
				secondaryNonProxySprite.gameObject.SetActive(false);
				break;
			case UserInventory.ItemType.Energy:
			case UserInventory.ItemType.Coins:
			case UserInventory.ItemType.PremiumCurrency:
				secondaryNonProxySprite.gameObject.SetActive(true);
				secondaryNonProxySprite.SetSprite(allItems.items[0].itemType.GetIconName());
				if ((bool)mainAdditionalPartCount)
				{
					if (allItems.items[0].amount > 1)
					{
						mainAdditionalPartCount.text = allItems.items[0].itemType.GetLocalizedName() + " x " + allItems.items[0].amount.ToString("N0");
					}
					else
					{
						mainAdditionalPartCount.text = allItems.items[0].itemType.GetLocalizedName();
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
}
