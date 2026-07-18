using System;
using System.Collections.Generic;

public class UserPriceDataModel : ItemCollectionDataModel
{
	public enum PaymentType
	{
		Normal = 0,
		UsePremiumForDifference = 1
	}

	public UserPriceDataModel(UserInventory.ItemType itemType, int amount)
		: base(itemType, amount)
	{
	}

	public UserPriceDataModel(UserInventory.ItemType itemType, int amount, UserInventory.ItemType itemType2, int amount2)
		: base(new List<Item>
		{
			new Item
			{
				itemType = itemType,
				amount = amount
			},
			new Item
			{
				itemType = itemType2,
				amount = amount2
			}
		})
	{
	}

	public UserPriceDataModel(List<Item> items)
		: base(items)
	{
	}

	public UserPriceDataModel()
	{
	}

	public UserPriceDataModel Multiply(int multiplier)
	{
		List<Item> list = new List<Item>();
		for (int i = 0; i < items.Count; i++)
		{
			Item item = new Item();
			item.itemType = items[i].itemType;
			item.amount = items[i].amount * multiplier;
			item.itemId = items[i].itemId;
			list.Add(item);
		}
		return new UserPriceDataModel(list);
	}

	public static UserPriceDataModel GetPremiumPrice(UserPriceDataModel originalPrice, UserProfile userProfile, int gemToCashExchangeRate)
	{
		int num = 0;
		UserPriceDataModel userPriceDataModel = new UserPriceDataModel();
		for (int i = 0; i < originalPrice.items.Count; i++)
		{
			Item item = originalPrice.items[i];
			if (item.itemType == UserInventory.ItemType.PremiumCurrency)
			{
				num += item.amount;
				continue;
			}
			if (item.itemType != UserInventory.ItemType.Coins && item.itemType != UserInventory.ItemType.Parts)
			{
				userPriceDataModel.AddItem(item);
				continue;
			}
			int val = ((item.itemType != UserInventory.ItemType.Coins) ? userProfile.inventory.GetParts(item.itemId.ToString()) : userProfile.coins);
			int num2 = Math.Min(item.amount, val);
			if (num2 > 0)
			{
				userPriceDataModel.AddItem(item.itemType, item.itemId, num2);
			}
			int num3 = item.amount - num2;
			if (num3 > 0)
			{
				num = ((item.itemType != UserInventory.ItemType.Coins) ? (num + num3 * UnitPartTypesDataModel.GetSingle(item.itemId.ToString()).gemValue) : (num + (int)Math.Ceiling((double)num3 / (double)gemToCashExchangeRate)));
			}
		}
		if (num > 0)
		{
			userPriceDataModel.AddItem(UserInventory.ItemType.PremiumCurrency, 0, num);
		}
		return userPriceDataModel;
	}
}
