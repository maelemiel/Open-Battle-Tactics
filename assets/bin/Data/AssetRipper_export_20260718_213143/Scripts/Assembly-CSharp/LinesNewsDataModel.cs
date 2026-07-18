using System.Collections.Generic;

public class LinesNewsDataModel : BaseDataModel
{
	public string newsKeyString;

	public static LinesNewsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LinesNewsDataModel>(id.ToString());
	}

	public static LinesNewsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LinesNewsDataModel>(id);
	}

	public static List<LinesNewsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<LinesNewsDataModel>();
	}
}
