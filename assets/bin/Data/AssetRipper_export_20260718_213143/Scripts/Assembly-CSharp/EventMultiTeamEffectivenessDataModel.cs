using System.Collections.Generic;

public class EventMultiTeamEffectivenessDataModel : BaseDataModel
{
	public int eventUnitRankMax;

	public int eventUnitRankMin;

	public int nonEventUnitRankMax;

	public int nonEventUnitRankMin;

	public int postEventUnitMax;

	public int postEventUnitMin;

	public int unitRarity;

	public static EventMultiTeamEffectivenessDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventMultiTeamEffectivenessDataModel>(id.ToString());
	}

	public static EventMultiTeamEffectivenessDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventMultiTeamEffectivenessDataModel>(id);
	}

	public static List<EventMultiTeamEffectivenessDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EventMultiTeamEffectivenessDataModel>();
	}
}
