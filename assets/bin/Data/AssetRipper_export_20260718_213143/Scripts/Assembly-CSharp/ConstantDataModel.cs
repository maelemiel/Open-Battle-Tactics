using System.Collections.Generic;

public class ConstantDataModel : BaseDataModel
{
	public string key;

	public string value;

	public static ConstantDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ConstantDataModel>(id.ToString());
	}

	public static ConstantDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ConstantDataModel>(id);
	}

	public static List<ConstantDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<ConstantDataModel>();
	}
}
