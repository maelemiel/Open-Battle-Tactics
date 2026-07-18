using System.Collections.Generic;

public class AiHandlerDataModel : BaseDataModel
{
	public string handler;

	public static AiHandlerDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AiHandlerDataModel>(id.ToString());
	}

	public static AiHandlerDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AiHandlerDataModel>(id);
	}

	public static List<AiHandlerDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AiHandlerDataModel>();
	}
}
