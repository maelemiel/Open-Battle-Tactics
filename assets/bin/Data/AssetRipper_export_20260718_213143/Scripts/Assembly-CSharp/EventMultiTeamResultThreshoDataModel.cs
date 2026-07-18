using System.Collections.Generic;

public class EventMultiTeamResultThreshoDataModel : BaseDataModel
{
	public string spriteName;

	public int thresholdValue;

	public static EventMultiTeamResultThreshoDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventMultiTeamResultThreshoDataModel>(id.ToString());
	}

	public static EventMultiTeamResultThreshoDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventMultiTeamResultThreshoDataModel>(id);
	}

	public static List<EventMultiTeamResultThreshoDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EventMultiTeamResultThreshoDataModel>();
	}
}
