using System.Collections.Generic;

public class UnitWeaponAnimDataModel : BaseDataModel
{
	public string animName;

	public static UnitWeaponAnimDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitWeaponAnimDataModel>(id.ToString());
	}

	public static UnitWeaponAnimDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitWeaponAnimDataModel>(id);
	}

	public static List<UnitWeaponAnimDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitWeaponAnimDataModel>();
	}
}
