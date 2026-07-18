using System.Collections.Generic;

public class AssetBundlesDataModel : BaseDataModel
{
	public string androidHash;

	public string iphoneHash;

	public string osxeditorHash;

	public int priority;

	public static AssetBundlesDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AssetBundlesDataModel>(id.ToString());
	}

	public static AssetBundlesDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AssetBundlesDataModel>(id);
	}

	public static List<AssetBundlesDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AssetBundlesDataModel>();
	}
}
