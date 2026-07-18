public class UserInventory
{
	public enum ItemType
	{
		Item = 1,
		Energy = 2,
		Coins = 4,
		PremiumCurrency = 5,
		Parts = 6,
		SlotIndex = 9,
		Unit = 10,
		EventPoint = 99,
		RaidBossEventPoint = 100,
		VictoryPoint = 101
	}

	private UserProfile user;

	public UserInventory(UserProfile user)
	{
		this.user = user;
	}

	public void SetParts(string partId, int amount)
	{
		user.SetPartCount(partId, amount);
	}

	public void AddParts(string partId, int amount)
	{
		SetParts(partId, GetParts(partId) + amount);
	}

	public void RemoveParts(string partId, int amount)
	{
		AddParts(partId, -amount);
		if (GetParts(partId) >= 0)
		{
		}
	}

	public int GetParts(string partId)
	{
		if (user.parts.ContainsKey(partId))
		{
			return user.parts[partId];
		}
		return 0;
	}

	public int GetItem(ItemType type, int itemId)
	{
		switch (type)
		{
		case ItemType.Coins:
			return user.coins;
		case ItemType.PremiumCurrency:
			return user.gems;
		case ItemType.Energy:
			return user.energy;
		case ItemType.Parts:
			return GetParts(itemId.ToString());
		default:
			return 0;
		}
	}

	public void SetItem(ItemType type, string itemId, int amount)
	{
		switch (type)
		{
		case ItemType.Coins:
			user.coins = amount;
			break;
		case ItemType.PremiumCurrency:
			user.gems = amount;
			break;
		case ItemType.Energy:
			user.energy = amount;
			break;
		case ItemType.Parts:
			SetParts(itemId, amount);
			break;
		}
	}

	public void AddItem(ItemType type, string itemId, int amount)
	{
		int result = 0;
		int.TryParse(itemId, out result);
		SetItem(type, itemId, GetItem(type, result) + amount);
	}

	public void RemoveItem(ItemType type, string itemId, int amount)
	{
		int result = 0;
		int.TryParse(itemId, out result);
		SetItem(type, itemId, GetItem(type, result) - amount);
	}
}
