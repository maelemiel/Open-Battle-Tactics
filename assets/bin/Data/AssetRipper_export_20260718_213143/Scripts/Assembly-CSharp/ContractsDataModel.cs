using System.Collections.Generic;

public class ContractsDataModel : BaseLocalizationDataModel
{
	public int assetLinkageId;

	public int contractDuration;

	public int contractId;

	public int contractPool;

	public int contractRarity;

	public int contractWeight;

	public AssetLinkageDataModel AssetLinkage
	{
		get
		{
			return AssetLinkageDataModel.GetSingle(assetLinkageId);
		}
	}

	public static ContractsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ContractsDataModel>(id.ToString());
	}

	public static ContractsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ContractsDataModel>(id);
	}

	public static List<ContractsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<ContractsDataModel>();
	}
}
