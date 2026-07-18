using System.Collections.Generic;

public class AssetLinkageDataModel : BaseDataModel
{
	public string assetName;

	public int bundleId;

	public int priority;

	public static AssetLinkageDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AssetLinkageDataModel>(id.ToString());
	}

	public static AssetLinkageDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AssetLinkageDataModel>(id);
	}

	public static List<AssetLinkageDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AssetLinkageDataModel>();
	}
}
