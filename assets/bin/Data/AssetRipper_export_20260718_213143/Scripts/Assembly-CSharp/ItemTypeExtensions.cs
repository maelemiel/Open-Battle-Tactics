using UnityEngine;

internal static class ItemTypeExtensions
{
	public static string GetServerCurrencyName(this UserInventory.ItemType itemType)
	{
		switch (itemType)
		{
		case UserInventory.ItemType.Coins:
			return "coins";
		case UserInventory.ItemType.PremiumCurrency:
			return "diamonds";
		case UserInventory.ItemType.Energy:
			return "energy";
		default:
			return string.Empty;
		}
	}

	public static Vector3 GetTopBarPosition(this UserInventory.ItemType itemType)
	{
		switch (itemType)
		{
		case UserInventory.ItemType.Coins:
			return TopBarController.instance.CoinsTransform.position;
		case UserInventory.ItemType.PremiumCurrency:
			return TopBarController.instance.ScrapTransform.position;
		case UserInventory.ItemType.Energy:
			return TopBarController.instance.EnergyTransform.position;
		default:
			return new Vector3(0f, 500f, 0f);
		}
	}

	public static bool IsCurrency(this UserInventory.ItemType itemType)
	{
		switch (itemType)
		{
		case UserInventory.ItemType.Coins:
			return true;
		case UserInventory.ItemType.PremiumCurrency:
			return true;
		case UserInventory.ItemType.Energy:
			return true;
		default:
			return false;
		}
	}

	public static string GetLocalizedName(this UserInventory.ItemType itemType)
	{
		ItemDataModel single = ItemDataModel.GetSingle((int)itemType);
		if (single != null)
		{
			return single.keyName.Localize(single.keyName);
		}
		return "<null>";
	}

	public static string GetIconName(this UserInventory.ItemType itemType)
	{
		switch (itemType)
		{
		case UserInventory.ItemType.Coins:
			return "icon_cash";
		case UserInventory.ItemType.PremiumCurrency:
			return "icon_diamond";
		case UserInventory.ItemType.Energy:
			return "icon_energy";
		case UserInventory.ItemType.EventPoint:
			return "icon_event_point";
		case UserInventory.ItemType.RaidBossEventPoint:
			return "icon_raid_boss_event_point";
		case UserInventory.ItemType.VictoryPoint:
			return "icon_pvp_tournament";
		default:
			return string.Empty;
		}
	}

	public static int GetStackSize(this UserInventory.ItemType itemType)
	{
		return CacheManager.GetConstantInt(itemType.ToString() + "_stack_amount", 1);
	}
}
