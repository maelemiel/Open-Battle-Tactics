using System;
using System.Collections.Generic;
using System.Globalization;

public class EventUnitsDataModel : BaseDataModel
{
	public string canBuild;

	public int eventId;

	public int unitId;

	public bool CanBuild
	{
		get
		{
			if (canBuild == "no")
			{
				return false;
			}
			if (canBuild == "yes")
			{
				return true;
			}
			DateTime t = DateTime.Parse(canBuild, CultureInfo.InvariantCulture).ToUniversalTime();
			return DateTime.Compare(t, DateTime.UtcNow) <= 0;
		}
	}

	public static EventUnitsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventUnitsDataModel>(id.ToString());
	}

	public static EventUnitsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventUnitsDataModel>(id);
	}

	public static List<EventUnitsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EventUnitsDataModel>();
	}
}
