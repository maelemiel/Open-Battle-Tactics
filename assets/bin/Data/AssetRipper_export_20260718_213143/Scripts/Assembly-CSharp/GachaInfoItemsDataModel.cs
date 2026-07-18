using System.Collections.Generic;

public class GachaInfoItemsDataModel : BaseLocalizationDataModel
{
	public int assetLinkage;

	public int eventAssetLinkageIndex;

	public int gachaPoolId;

	public string Name
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(base.name);
		}
	}

	public string Description
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(base.description);
		}
	}

	public AssetLinkageDataModel AssetLinkage
	{
		get
		{
			return AssetLinkageDataModel.GetSingle(assetLinkage);
		}
	}

	public static GachaInfoItemsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaInfoItemsDataModel>(id.ToString());
	}

	public static GachaInfoItemsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaInfoItemsDataModel>(id);
	}

	public static List<GachaInfoItemsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<GachaInfoItemsDataModel>();
	}
}
