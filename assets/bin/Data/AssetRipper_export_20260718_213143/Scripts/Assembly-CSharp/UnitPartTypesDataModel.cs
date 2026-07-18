using System;
using System.Collections.Generic;
using System.Globalization;

public class UnitPartTypesDataModel : BaseDataModel
{
	private UnitLevelProgressionDataModel _cachedTokenGranter;

	private int _cachedIsToken = -1;

	public int assetLinkageId;

	public int gemValue;

	public string isHidden;

	public string name;

	public int orderBy;

	public int rarity;

	public string Name
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(name);
		}
	}

	public AssetLinkageDataModel AssetLinkage
	{
		get
		{
			return AssetLinkageDataModel.GetSingle(assetLinkageId);
		}
	}

	public int OrderBy
	{
		get
		{
			return orderBy;
		}
	}

	public UnitRarityDataModel Rarity
	{
		get
		{
			return UnitRarityDataModel.GetSingle(rarity);
		}
	}

	public bool IsHidden
	{
		get
		{
			if (isHidden == "no")
			{
				return false;
			}
			if (isHidden == "yes")
			{
				return true;
			}
			DateTime t = DateTime.Parse(isHidden, CultureInfo.InvariantCulture).ToUniversalTime();
			return DateTime.Compare(t, DateTime.UtcNow) <= 0;
		}
	}

	public bool IsToken
	{
		get
		{
			if (_cachedIsToken == -1)
			{
				_cachedIsToken = ((TokenGeneratingUnitProgression != null) ? 1 : 0);
			}
			return _cachedIsToken == 1;
		}
	}

	public UnitLevelProgressionDataModel TokenGeneratingUnitProgression
	{
		get
		{
			if (_cachedIsToken == -1)
			{
				List<UnitLevelProgressionDataModel> all = UnitLevelProgressionDataModel.GetAll();
				for (int i = 0; i < all.Count; i++)
				{
					int onLevelGiftId = all[i].onLevelGiftId;
					if (onLevelGiftId != -1)
					{
						ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(onLevelGiftId);
						if (giftPackage.items.Exists((ItemCollectionDataModel.Item x) => x.itemType == UserInventory.ItemType.Parts && x.itemId.ToString() == id))
						{
							_cachedTokenGranter = all[i];
							_cachedIsToken = 1;
							return _cachedTokenGranter;
						}
					}
				}
				_cachedIsToken = 0;
			}
			return _cachedTokenGranter;
		}
	}

	public UnitDataModel FirstAssociatedUnit
	{
		get
		{
			List<UnitPartsDataModel> allAssociations = UnitPartsDataModel.GetAllAssociations(id);
			if (allAssociations.Count > 0)
			{
				return UnitDataModel.GetSingle(allAssociations[0].unitId);
			}
			return null;
		}
	}

	public UnitDataModel FirstAssociatedPartialLevelUnit
	{
		get
		{
			List<ItemPriceDataModel> partPrices = ItemPriceDataModel.GetPartPrices(id);
			int i = 0;
			for (int count = partPrices.Count; i < count; i++)
			{
				List<UnitPartialLevelDataModel> partialLevelsPrice = UnitPartialLevelDataModel.GetPartialLevelsPrice(partPrices[i].priceId);
				if (partialLevelsPrice.Count > 0)
				{
					return UnitDataModel.GetSingle(partialLevelsPrice[0].unitId);
				}
			}
			return null;
		}
	}

	public static UnitPartTypesDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitPartTypesDataModel>(id.ToString());
	}

	public static UnitPartTypesDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitPartTypesDataModel>(id);
	}

	public static List<UnitPartTypesDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitPartTypesDataModel>();
	}
}
