using System.Collections.Generic;

public class UnitScrapValueDataModel : BaseDataModel
{
	public int giftId;

	public int rarity;

	public int unitLevel;

	public static UnitScrapValueDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitScrapValueDataModel>(id.ToString());
	}

	public static UnitScrapValueDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitScrapValueDataModel>(id);
	}

	public static List<UnitScrapValueDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitScrapValueDataModel>();
	}
}
