using System.Collections.Generic;

public class HelpRegistersDataModel : BaseDataModel
{
	public string descriptionKey;

	public string name;

	public int orderNumber;

	public string titleKey;

	public int topicId;

	public static HelpRegistersDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<HelpRegistersDataModel>(id.ToString());
	}

	public static HelpRegistersDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<HelpRegistersDataModel>(id);
	}

	public static List<HelpRegistersDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<HelpRegistersDataModel>();
	}
}
