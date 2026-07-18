using System.Collections.Generic;

public class ContractDetailsDataModel : BaseDataModel
{
	public int contractId;

	public int dropMax;

	public int dropMin;

	public int dropRate;

	public int itemId;

	public int partType;

	public static ContractDetailsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ContractDetailsDataModel>(id.ToString());
	}

	public static ContractDetailsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ContractDetailsDataModel>(id);
	}

	public static List<ContractDetailsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<ContractDetailsDataModel>();
	}
}
