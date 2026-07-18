using System.Collections.Generic;

public class UnitRarityDataModel : BaseDataModel
{
	public enum RarityType
	{
		COMMON = 1,
		UNCOMMON = 2,
		RARE = 3,
		SUPERRARE = 4,
		LEGENDARY = 5,
		MAX = 6
	}

	public string keyName;

	public string Name
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(keyName);
		}
	}

	public RarityType Type
	{
		get
		{
			int result;
			if (int.TryParse(id, out result))
			{
				return (RarityType)result;
			}
			return RarityType.COMMON;
		}
	}

	public static UnitRarityDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitRarityDataModel>(id.ToString());
	}

	public static UnitRarityDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitRarityDataModel>(id);
	}

	public static List<UnitRarityDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitRarityDataModel>();
	}
}
