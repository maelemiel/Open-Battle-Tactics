using System.Collections.Generic;

public class EventPointsBucketsDataModel : BaseDataModel
{
	public int eventId;

	public int pointsEnd;

	public int pointsStart;

	public static EventPointsBucketsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventPointsBucketsDataModel>(id.ToString());
	}

	public static EventPointsBucketsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventPointsBucketsDataModel>(id);
	}

	public static List<EventPointsBucketsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EventPointsBucketsDataModel>();
	}
}
