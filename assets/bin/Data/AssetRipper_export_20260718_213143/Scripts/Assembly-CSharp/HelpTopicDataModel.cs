using System.Collections.Generic;

public class HelpTopicDataModel : BaseDataModel
{
	public string name;

	public string titleKey;

	public static HelpTopicDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<HelpTopicDataModel>(id.ToString());
	}

	public static HelpTopicDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<HelpTopicDataModel>(id);
	}

	public static List<HelpTopicDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<HelpTopicDataModel>();
	}
}
