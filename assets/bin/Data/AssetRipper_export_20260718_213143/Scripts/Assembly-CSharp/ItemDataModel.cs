using System.Collections.Generic;

public class ItemDataModel : BaseDataModel
{
	public string keyName;

	public string keyNameSingular;

	public static ItemDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ItemDataModel>(id.ToString());
	}

	public static ItemDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ItemDataModel>(id);
	}

	public static List<ItemDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<ItemDataModel>();
	}
}
