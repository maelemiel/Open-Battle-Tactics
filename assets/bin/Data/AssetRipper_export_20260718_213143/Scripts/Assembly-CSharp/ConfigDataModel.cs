using System.Collections.Generic;

public class ConfigDataModel : BaseDataModel
{
	public string key;

	public string value;

	public static ConfigDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ConfigDataModel>(id.ToString());
	}

	public static ConfigDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ConfigDataModel>(id);
	}

	public static List<ConfigDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<ConfigDataModel>();
	}
}
