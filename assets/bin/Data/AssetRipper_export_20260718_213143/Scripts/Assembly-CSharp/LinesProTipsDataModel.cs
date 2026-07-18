using System.Collections.Generic;

public class LinesProTipsDataModel : BaseDataModel
{
	public string proTipsKeyString;

	public static LinesProTipsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LinesProTipsDataModel>(id.ToString());
	}

	public static LinesProTipsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LinesProTipsDataModel>(id);
	}

	public static List<LinesProTipsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<LinesProTipsDataModel>();
	}
}
