using System.Collections.Generic;

public class UnitPartsDataModel : BaseDataModel, IPartMetadata
{
	public int amount;

	public int dropMax;

	public int dropMin;

	public int dropRate;

	public int partType;

	public int unitId;

	public string Name
	{
		get
		{
			return Type.Name;
		}
	}

	public UnitPartTypesDataModel Type
	{
		get
		{
			return UnitPartTypesDataModel.GetSingle(partType);
		}
	}

	public string ID
	{
		get
		{
			return partType.ToString();
		}
	}

	public int PartType
	{
		get
		{
			return partType;
		}
	}

	public int DropChance
	{
		get
		{
			return dropRate;
		}
	}

	public int DropMin
	{
		get
		{
			return dropMin;
		}
	}

	public int DropMax
	{
		get
		{
			return dropMax;
		}
	}

	public static UnitPartsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitPartsDataModel>(id.ToString());
	}

	public static UnitPartsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitPartsDataModel>(id);
	}

	public static List<UnitPartsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitPartsDataModel>();
	}

	public static List<UnitPartsDataModel> GetAllAssociations(string partType)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitPartsDataModel>(" WHERE part_type = " + partType);
	}
}
