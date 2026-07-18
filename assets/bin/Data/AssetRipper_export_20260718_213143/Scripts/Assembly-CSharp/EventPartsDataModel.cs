using System.Collections.Generic;

public class EventPartsDataModel : BaseDataModel, IPartMetadata
{
	public int dropMax;

	public int dropMin;

	public int dropRate;

	public int eventId;

	public int partType;

	public int unitId;

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

	public static EventPartsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventPartsDataModel>(id.ToString());
	}

	public static EventPartsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventPartsDataModel>(id);
	}

	public static List<EventPartsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EventPartsDataModel>();
	}

	public static EventPartsDataModel FindUnitEventPart(string unitid, string eventid)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingleByQuery<EventPartsDataModel>(" WHERE unit_id = " + unitid + " AND event_id = " + eventid);
	}
}
